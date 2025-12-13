using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Collections.Analyzer.Diagnostics.CI0008;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ArrayContainsToHashSetDiagnostic : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        "CI0008",
        Resources.CI0008_Title,
        Resources.CI0008_MessageFormat,
        Categories.Performance,
        DiagnosticSeverity.Warning,
        true,
        Resources.CI0008_Description
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSyntaxNodeAction(AnalyzeLocalDeclaration, SyntaxKind.LocalDeclarationStatement);
        context.RegisterSyntaxNodeAction(AnalyzeFieldDeclaration, SyntaxKind.FieldDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzePropertyDeclaration, SyntaxKind.PropertyDeclaration);
    }

    private static void AnalyzeLocalDeclaration(SyntaxNodeAnalysisContext context)
    {
        var localDeclaration = (LocalDeclarationStatementSyntax)context.Node;

        foreach (var variable in localDeclaration.Declaration.Variables)
        {
            AnalyzeVariable(context, variable, localDeclaration.Declaration.Type);
        }
    }

    private static void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
    {
        var fieldDeclaration = (FieldDeclarationSyntax)context.Node;

        foreach (var variable in fieldDeclaration.Declaration.Variables)
        {
            AnalyzeVariable(context, variable, fieldDeclaration.Declaration.Type);
        }
    }
    
    private static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
    {
        var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

        var typeInfo = context.SemanticModel.GetTypeInfo(propertyDeclaration.Type);
        if (typeInfo.Type is not IArrayTypeSymbol arrayType)
            return;

        var initializer = propertyDeclaration.Initializer;
        if (initializer == null)
            return;

        var arrayInitializer = FindArrayInitializer(initializer);
        if (arrayInitializer == null)
            return;

        var arraySize = CountArrayElements(arrayInitializer);
        if (arraySize <= 1)
            return;

        var propertySymbol = context.SemanticModel.GetDeclaredSymbol(propertyDeclaration);
        if (propertySymbol == null)
            return;

        var classDeclaration = propertyDeclaration.FirstAncestorOrSelf<ClassDeclarationSyntax>();
        if (classDeclaration == null)
            return;

        var usageAnalysis = AnalyzeVariableUsage(classDeclaration, propertySymbol, context);

        if (usageAnalysis.ShouldWarn)
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                propertyDeclaration.Identifier.GetLocation(),
                propertySymbol.Name,
                arrayType.ElementType.ToDisplayString()
            );
            context.ReportDiagnostic(diagnostic);
        }
    }
    
    private static InitializerExpressionSyntax? FindArrayInitializer(EqualsValueClauseSyntax equalsValue)
    {
        if (equalsValue.Value is ImplicitArrayCreationExpressionSyntax implicitArray)
            return implicitArray.Initializer;

        if (equalsValue.Value is ArrayCreationExpressionSyntax arrayCreation)
            return arrayCreation.Initializer;

        if (equalsValue.Value is InitializerExpressionSyntax initializerExpression)
            return initializerExpression;

        return null;
    }

    private static void AnalyzeVariable(
        SyntaxNodeAnalysisContext context,
        VariableDeclaratorSyntax variable,
        TypeSyntax typeSyntax)
    {
        var typeInfo = context.SemanticModel.GetTypeInfo(typeSyntax);
        if (typeInfo.Type is not IArrayTypeSymbol arrayType)
            return;

        var initializer = FindArrayInitializer(variable);
        if (initializer == null)
            return;

        var arraySize = CountArrayElements(initializer);
        if (arraySize <= 1)
            return;

        var variableSymbol = context.SemanticModel.GetDeclaredSymbol(variable);
        if (variableSymbol == null)
            return;

        var scope = GetAnalysisScope(variable);
        if (scope == null)
            return;

        var usageAnalysis = AnalyzeVariableUsage(scope, variableSymbol, context);

        if (usageAnalysis.ShouldWarn)
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                variable.Identifier.GetLocation(),
                variableSymbol.Name,
                arrayType.ElementType.ToDisplayString()
            );
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static SyntaxNode? GetAnalysisScope(SyntaxNode node)
    {
        // Для локальных переменных - метод
        var method = node.FirstAncestorOrSelf<BaseMethodDeclarationSyntax>();
        if (method != null)
            return method;

        // Для полей - весь класс
        var classDeclaration = node.FirstAncestorOrSelf<ClassDeclarationSyntax>();
        if (classDeclaration != null)
            return classDeclaration;

        return null;
    }

    private static UsageAnalysis AnalyzeVariableUsage(
        SyntaxNode scope,
        ISymbol variableSymbol,
        SyntaxNodeAnalysisContext context)
    {
        var result = new UsageAnalysis();
        var descendantNodes = scope.DescendantNodes().ToList();

        foreach (var node in descendantNodes)
        {
            if (node is not IdentifierNameSyntax identifier)
                continue;

            var symbol = context.SemanticModel.GetSymbolInfo(identifier).Symbol;
            if (!SymbolEqualityComparer.Default.Equals(symbol, variableSymbol))
                continue;

            var parent = identifier.Parent;

            // check Contains
            if (!result.HasContainsCall && parent is MemberAccessExpressionSyntax { Parent: InvocationExpressionSyntax invocation })
            {
                var methodSymbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
                if (methodSymbol?.Name == nameof(Enumerable.Contains))
                {
                    result.HasContainsCall = true;
                }
            }

            if (parent is ElementAccessExpressionSyntax elementAccess)
            {
                // Проверяем, это чтение или запись
                if (elementAccess.Parent is AssignmentExpressionSyntax assignment &&
                    assignment.Left == elementAccess)
                {
                    result.HasModification = true;
                }
                else
                {
                    result.HasIndexAccess = true;
                }
            }
        }

        return result;
    }

    private static InitializerExpressionSyntax? FindArrayInitializer(VariableDeclaratorSyntax variable)
    {
        if (variable.Initializer?.Value is ImplicitArrayCreationExpressionSyntax implicitArray)
            return implicitArray.Initializer;

        if (variable.Initializer?.Value is ArrayCreationExpressionSyntax arrayCreation)
            return arrayCreation.Initializer;

        if (variable.Initializer?.Value is InitializerExpressionSyntax initializerExpression)
            return initializerExpression;

        return null;
    }
    
    private static int CountArrayElements(InitializerExpressionSyntax initializer)
    {
        return initializer.Expressions.Count;
    }

    private class UsageAnalysis
    {
        public bool HasContainsCall { get; set; }
        public bool HasIndexAccess { get; set; }
        public bool HasModification { get; set; }
        public bool ShouldWarn => HasContainsCall && !HasIndexAccess && !HasModification;
    }
}