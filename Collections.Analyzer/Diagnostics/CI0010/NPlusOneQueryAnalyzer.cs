using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Collections.Analyzer.Diagnostics.CI0010
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NPlusOneQueryAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CI0010"; // Замените на нужный ID в проекте
        
        private static readonly LocalizableString Title = "Potential N+1 Query Problem";
        private static readonly LocalizableString MessageFormat = "Method '{0}' is called inside a loop and may cause an N+1 query problem";
        private static readonly LocalizableString Description = "Avoid calling data access methods inside a loop based on the loop variable, as this indicates an N+1 query performance issue.";
        private const string Category = "Performance";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            
            // Регистрируем анализ на циклы foreach, for и кортежные foreach
            context.RegisterSyntaxNodeAction(AnalyzeLoopNode, 
                SyntaxKind.ForEachStatement, 
                SyntaxKind.ForStatement, 
                SyntaxKind.ForEachVariableStatement);
        }

        private void AnalyzeLoopNode(SyntaxNodeAnalysisContext context)
        {
            var loopNode = context.Node;

            // 1. Игнорируем тестовые методы
            var enclosingMethodSymbol = context.SemanticModel.GetEnclosingSymbol(loopNode.SpanStart) as IMethodSymbol;
            if (enclosingMethodSymbol != null && IsTestMethod(enclosingMethodSymbol))
            {
                return;
            }

            // 2. Получаем переменные цикла (поддерживает кортежи)
            var loopVariables = GetLoopVariableSymbols(loopNode, context.SemanticModel);
            if (loopVariables.Count == 0) return;

            var invocationExpressions = loopNode.DescendantNodes().OfType<InvocationExpressionSyntax>();

            foreach (var invocation in invocationExpressions)
            {
                var methodSymbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
                if (methodSymbol == null) continue;

                // 3. Ищем любую из переменных цикла в аргументах вызываемого метода
                var usesLoopVariable = false;
                foreach (var argument in invocation.ArgumentList.Arguments)
                {
                    var dataFlowAnalysis = context.SemanticModel.AnalyzeDataFlow(argument.Expression);
                    if (!dataFlowAnalysis.Succeeded) continue;

                    if (dataFlowAnalysis.ReadInside.Any(symbol => 
                        loopVariables.Contains(symbol, SymbolEqualityComparer.Default)))
                    {
                        usesLoopVariable = true;
                        break;
                    }
                }

                if (!usesLoopVariable) continue;

                // 4. ПРАВИЛО 1: Тип класса или интерфейса должен иметь целевой суффикс
                bool isDataAccessType = false;
                var containingType = methodSymbol.ContainingType;

                if (containingType != null)
                {
                    // Проверяем само имя типа
                    if (IsDataAccessTypeName(containingType.Name))
                    {
                        isDataAccessType = true;
                    }
                    // Проверяем все реализуемые интерфейсы
                    else if (containingType.AllInterfaces.Any(i => IsDataAccessTypeName(i.Name)))
                    {
                        isDataAccessType = true;
                    }
                }

                if (!isDataAccessType) continue;

                // 5. ПРАВИЛО 2: Методы Read, Find, Get (включая асинхронные версии)
                var methodName = methodSymbol.Name;

                var isTargetMethod = methodName.StartsWith("Read") || 
                                     methodName.StartsWith("Find") || 
                                     methodName.StartsWith("Get");

                if (!isTargetMethod) continue;

                // 6. Репортим ошибку (ПРАВИЛО 4 - внутри цикла)
                var diagnostic = Diagnostic.Create(Rule, invocation.GetLocation(), methodSymbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool IsDataAccessTypeName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return false;

            return typeName.EndsWith("Repository") ||
                   typeName.EndsWith("Reader") ||
                   typeName.EndsWith("Writer") ||
                   typeName.EndsWith("Handler");
        }

        private static IReadOnlyCollection<ISymbol> GetLoopVariableSymbols(SyntaxNode loopNode, SemanticModel semanticModel)
        {
            var symbols = new HashSet<ISymbol>(SymbolEqualityComparer.Default);

            switch (loopNode)
            {
                case ForEachStatementSyntax forEachStatement:
                {
                    var symbol = semanticModel.GetDeclaredSymbol(forEachStatement);
                    if (symbol != null) symbols.Add(symbol);
                    break;
                }
                case ForStatementSyntax forStatement:
                {
                    var variableDeclarator = forStatement.Declaration?.Variables.FirstOrDefault();
                    if (variableDeclarator != null)
                    {
                        var symbol = semanticModel.GetDeclaredSymbol(variableDeclarator);
                        if (symbol != null) symbols.Add(symbol);
                    }

                    break;
                }
                case ForEachVariableStatementSyntax forEachVariableStatement:
                {
                    // Поддержка деконструкции: foreach (var (a, b) in items)
                    if (forEachVariableStatement.Variable is DeclarationExpressionSyntax declarationExpression)
                    {
                        // Рекурсивно собираем все SingleVariableDesignationSyntax
                        var designations = declarationExpression.Designation
                            .DescendantNodesAndSelf()
                            .OfType<SingleVariableDesignationSyntax>();

                        symbols.UnionWith(designations.Select(designation => semanticModel.GetDeclaredSymbol(designation)).OfType<ISymbol>());
                    }

                    break;
                }
            }

            return symbols;
        }

        private static bool IsTestMethod(IMethodSymbol methodSymbol)
        {
            if (HasTestAttribute(methodSymbol.GetAttributes())) return true;

            return methodSymbol.ContainingType != null && 
                   HasTestAttribute(methodSymbol.ContainingType.GetAttributes());
        }

        private static bool HasTestAttribute(ImmutableArray<AttributeData> attributes)
        {
            foreach (var attribute in attributes)
            {
                var attributeName = attribute.AttributeClass?.Name;
                if (string.IsNullOrEmpty(attributeName)) continue;

                if (attributeName is "FactAttribute" or "TheoryAttribute" or "TestAttribute" or "TestCaseAttribute" or "TestFixtureAttribute" or "TestMethodAttribute" or "TestClassAttribute")
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}