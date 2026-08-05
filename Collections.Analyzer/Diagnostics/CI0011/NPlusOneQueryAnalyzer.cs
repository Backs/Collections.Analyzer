using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Collections.Analyzer.Diagnostics.CI0011;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NPlusOneQueryAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    internal static readonly DiagnosticDescriptor Rule = new(
        "CI0011",
        Resources.CI0011_Title,
        Resources.CI0011_MessageFormat,
        Categories.Performance,
        DiagnosticSeverity.Warning,
        true,
        Resources.CI0011_Description
    );

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeLoopNode,
            SyntaxKind.ForEachStatement,
            SyntaxKind.ForStatement,
            SyntaxKind.ForEachVariableStatement);
        context.RegisterSyntaxNodeAction(AnalyzeLinqInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeLinqInvocation(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not InvocationExpressionSyntax invocation) return;
        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess) return;

        var methodName = memberAccess.Name.Identifier.Text;
        if (methodName is not ("Select" or "Where" or "Any" or "All" or "Count" or "First" or "FirstOrDefault" or "Single" or "SingleOrDefault")) return;

        if (invocation.ArgumentList.Arguments.Count == 0) return;
        var argument = invocation.ArgumentList.Arguments[0];

        if (argument.Expression is not LambdaExpressionSyntax lambda) return;

        var body = lambda switch
        {
            SimpleLambdaExpressionSyntax simple => simple.Body,
            ParenthesizedLambdaExpressionSyntax parenthesized => parenthesized.Body,
            _ => null
        };

        if (body == null) return;

        var lambdaParameters = GetLambdaParameters(lambda, context.SemanticModel);
        if (lambdaParameters.Count == 0) return;

        var innerInvocations = body.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>();
        foreach (var innerInvocation in innerInvocations)
        {
            AnalyzeInvocation(context, innerInvocation, lambdaParameters);
        }
    }

    private static IReadOnlyCollection<ISymbol> GetLambdaParameters(LambdaExpressionSyntax lambda, SemanticModel semanticModel)
    {
        var parameters = lambda switch
        {
            SimpleLambdaExpressionSyntax simple => new[] { simple.Parameter },
            ParenthesizedLambdaExpressionSyntax parenthesized => parenthesized.ParameterList.Parameters.ToArray(),
            _ => System.Array.Empty<ParameterSyntax>()
        };

        return parameters
            .Select(p => semanticModel.GetDeclaredSymbol(p))
            .OfType<ISymbol>()
            .ToList();
    }

    private static void AnalyzeLoopNode(SyntaxNodeAnalysisContext context)
    {
        var loopNode = context.Node;

        var enclosingMethodSymbol = context.SemanticModel.GetEnclosingSymbol(loopNode.SpanStart) as IMethodSymbol;
        if (enclosingMethodSymbol != null && IsTestMethod(enclosingMethodSymbol))
        {
            return;
        }

        var loopVariables = GetLoopVariableSymbols(loopNode, context.SemanticModel);
        if (loopVariables.Count == 0)
        {
            return;
        }

        var invocations = loopNode.DescendantNodes().OfType<InvocationExpressionSyntax>();
        foreach (var invocation in invocations)
        {
            AnalyzeInvocation(context, invocation, loopVariables);
        }
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, IReadOnlyCollection<ISymbol> loopVariables)
    {
        var methodSymbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
        if (methodSymbol == null)
        {
            return;
        }

        if (!UsesLoopVariable(context, invocation, loopVariables))
        {
            return;
        }

        if (IsDataAccessMethod(methodSymbol))
        {
            var diagnostic = Diagnostic.Create(Rule, invocation.GetLocation(), methodSymbol.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static bool UsesLoopVariable(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, IReadOnlyCollection<ISymbol> loopVariables)
    {
        foreach (var argument in invocation.ArgumentList.Arguments)
        {
            var dataFlowAnalysis = context.SemanticModel.AnalyzeDataFlow(argument.Expression);
            if (!dataFlowAnalysis.Succeeded)
            {
                continue;
            }

            if (dataFlowAnalysis.ReadInside.Any(symbol => loopVariables.Contains(symbol, SymbolEqualityComparer.Default)))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsDataAccessMethod(IMethodSymbol methodSymbol)
    {
        if (!IsDataAccessType(methodSymbol.ContainingType))
        {
            return false;
        }

        var methodName = methodSymbol.Name;
        return methodName.StartsWith("Read") ||
               methodName.StartsWith("Find") ||
               methodName.StartsWith("Get") ||
               methodName.Contains("TryRead") ||
               methodName.Contains("TryGet") ||
               methodName.Contains("TryFind");
    }

    private static bool IsDataAccessType(INamedTypeSymbol? type)
    {
        if (type == null)
        {
            return false;
        }

        if (IsDataAccessTypeName(type.Name))
        {
            return true;
        }

        return type.AllInterfaces.Any(i => IsDataAccessTypeName(i.Name));
    }

    private static bool IsDataAccessTypeName(string typeName)
    {
        if (string.IsNullOrEmpty(typeName))
        {
            return false;
        }

        return typeName.Contains("Repository") ||
               typeName.Contains("Reader") ||
               typeName.Contains("Writer") ||
               typeName.Contains("Handler");
    }

    private static IReadOnlyCollection<ISymbol> GetLoopVariableSymbols(SyntaxNode loopNode, SemanticModel semanticModel)
    {
        var symbols = new HashSet<ISymbol>(SymbolEqualityComparer.Default);

        switch (loopNode)
        {
            case ForEachStatementSyntax forEachStatement:
                var symbol = semanticModel.GetDeclaredSymbol(forEachStatement);
                if (symbol != null) symbols.Add(symbol);
                break;

            case ForStatementSyntax forStatement:
                var variableDeclarator = forStatement.Declaration?.Variables.FirstOrDefault();
                if (variableDeclarator != null)
                {
                    var variableSymbol = semanticModel.GetDeclaredSymbol(variableDeclarator);
                    if (variableSymbol != null) symbols.Add(variableSymbol);
                }
                break;

            case ForEachVariableStatementSyntax forEachVariableStatement:
                if (forEachVariableStatement.Variable is DeclarationExpressionSyntax declarationExpression)
                {
                    var designations = declarationExpression.Designation
                        .DescendantNodesAndSelf()
                        .OfType<SingleVariableDesignationSyntax>();

                    foreach (var designation in designations)
                    {
                        var declaredSymbol = semanticModel.GetDeclaredSymbol(designation);
                        if (declaredSymbol != null) symbols.Add(declaredSymbol);
                    }
                }
                break;
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
        return attributes.Any(attribute =>
        {
            var name = attribute.AttributeClass?.Name;
            return name is "FactAttribute" or "TheoryAttribute" or "TestAttribute" or "TestCaseAttribute" or "TestFixtureAttribute" or "TestMethodAttribute" or "TestClassAttribute";
        });
    }
}
