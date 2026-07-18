using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Collections.Analyzer.Diagnostics.CI0010;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DictionaryLookupDiagnostic : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    internal static readonly DiagnosticDescriptor Rule = new(
        "CI0010",
        Resources.CI0010_Title,
        Resources.CI0010_MessageFormat,
        Categories.Performance,
        DiagnosticSeverity.Warning,
        true
    );

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ForEachStatement, SyntaxKind.ForStatement, SyntaxKind.InvocationExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is InvocationExpressionSyntax invocationNode)
        {
            AnalyzeInvocation(context, invocationNode);
            return;
        }

        var loopBody = context.Node switch
        {
            ForEachStatementSyntax foreachStatement => foreachStatement.Statement,
            ForStatementSyntax forStatement => forStatement.Statement,
            _ => null
        };

        if (loopBody == null) return;

        var loopVariables = GetLoopVariables(context.Node);
        if (loopVariables.Length == 0) return;

        var invocations = loopBody.DescendantNodes()
            .OfType<InvocationExpressionSyntax>();

        foreach (var invocation in invocations)
        {
            AnalyzeInvocationWithVariables(context, invocation, context.Node, loopVariables);
        }
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation)
    {
        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess) return;

        var methodName = memberAccess.Name.Identifier.Text;
        if (methodName is not (
            nameof(Enumerable.Select) or
            nameof(Enumerable.Where) or
            nameof(Enumerable.Any) or
            nameof(Enumerable.All) or
            nameof(Enumerable.Count) or
            nameof(Enumerable.First) or
            nameof(Enumerable.FirstOrDefault) or
            nameof(Enumerable.Single) or
            nameof(Enumerable.SingleOrDefault)))
            return;

        if (invocation.ArgumentList.Arguments.Count == 0) 
            return;
        var argument = invocation.ArgumentList.Arguments[0];

        if (argument.Expression is not LambdaExpressionSyntax lambda) 
            return;

        var loopVariables = GetLambdaParameters(lambda);
        if (loopVariables.Length == 0) 
            return;

        var body = lambda is SimpleLambdaExpressionSyntax simple ? simple.Body : (lambda as ParenthesizedLambdaExpressionSyntax)?.Body;
        if (body == null) 
            return;

        var innerInvocations = body.DescendantNodesAndSelf()
            .OfType<InvocationExpressionSyntax>();

        foreach (var innerInvocation in innerInvocations)
        {
            AnalyzeInvocationWithVariables(context, innerInvocation, invocation, loopVariables);
        }
    }

    private static void AnalyzeInvocationWithVariables(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, SyntaxNode scopeNode, string[] loopVariables)
    {
        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess) return;

        var methodName = memberAccess.Name.Identifier.Text;
        if (methodName is not (
            nameof(Enumerable.First) or 
            nameof(Enumerable.FirstOrDefault) or 
            nameof(Enumerable.Single) or 
            nameof(Enumerable.SingleOrDefault) or 
            nameof(Enumerable.Any))) return;

        if (invocation.ArgumentList.Arguments.Count != 1) return;
        var argument = invocation.ArgumentList.Arguments[0];

        if (argument.Expression is not LambdaExpressionSyntax lambda) return;

        if (!IsSearchingByLoopVariable(lambda, context.SemanticModel, scopeNode, loopVariables)) return;

        // Check if the collection is defined outside the loop
        var collectionExpression = memberAccess.Expression;
        if (!IsDefinedOutside(context, collectionExpression, scopeNode)) return;

        var collectionName = collectionExpression.ToString();
        context.ReportDiagnostic(Diagnostic.Create(Rule, invocation.GetLocation(), collectionName));
    }

    private static string[] GetLambdaParameters(LambdaExpressionSyntax lambda)
    {
        if (lambda is SimpleLambdaExpressionSyntax simple)
        {
            return new[] { simple.Parameter.Identifier.Text };
        }

        if (lambda is ParenthesizedLambdaExpressionSyntax parenthesized)
        {
            return parenthesized.ParameterList.Parameters.Select(p => p.Identifier.Text).ToArray();
        }

        return System.Array.Empty<string>();
    }


    private static string[] GetLoopVariables(SyntaxNode loop)
    {
        return loop switch
        {
            ForEachStatementSyntax foreachStatement => new[] { foreachStatement.Identifier.Text },
            ForStatementSyntax { Declaration: not null } forStatement => forStatement.Declaration.Variables
                .Select(v => v.Identifier.Text)
                .ToArray(),
            _ => System.Array.Empty<string>()
        };
    }

    private static bool IsSearchingByLoopVariable(LambdaExpressionSyntax lambda, SemanticModel semanticModel, SyntaxNode loop, string[] loopVariables)
    {
        // lambda is something like x => x.Id == key
        var expression = lambda is SimpleLambdaExpressionSyntax simple ? simple.Body : (lambda as ParenthesizedLambdaExpressionSyntax)?.Body;

        if (expression is not BinaryExpressionSyntax binary) return false;
        if (!binary.IsKind(SyntaxKind.EqualsExpression)) return false;

        // Check if one side is a property access on lambda parameter and another side is a loop variable
        return (IsPropertyAccessOnParameter(lambda, binary.Left) && IsLoopVariableOrMember(binary.Right, semanticModel, loop, loopVariables)) ||
               (IsPropertyAccessOnParameter(lambda, binary.Right) && IsLoopVariableOrMember(binary.Left, semanticModel, loop, loopVariables));
    }

    private static bool IsPropertyAccessOnParameter(LambdaExpressionSyntax lambda, ExpressionSyntax expression)
    {
        var parameterName = lambda switch
        {
            SimpleLambdaExpressionSyntax simple => simple.Parameter.Identifier.Text,
            ParenthesizedLambdaExpressionSyntax paren => paren.ParameterList.Parameters.FirstOrDefault()?.Identifier.Text,
            _ => null
        };

        if (parameterName == null) return false;

        if (expression is MemberAccessExpressionSyntax memberAccess)
        {
            return memberAccess.Expression is IdentifierNameSyntax id && id.Identifier.Text == parameterName;
        }

        if (expression is IdentifierNameSyntax identifier)
        {
            return identifier.Identifier.Text == parameterName;
        }

        return false;
    }

    private static bool IsLoopVariableOrMember(ExpressionSyntax expression, SemanticModel semanticModel, SyntaxNode loop, string[] loopVariables)
    {
        if (expression is IdentifierNameSyntax id)
        {
            var name = id.Identifier.Text;
            if (loopVariables.Contains(name)) 
                return true;

            // Check if it's a local variable defined inside the loop
            var symbol = semanticModel.GetSymbolInfo(id).Symbol;
            if (symbol is ILocalSymbol localSymbol)
            {
                var loopSpan = loop.GetLocation().SourceSpan;
                var isInsideLoop = localSymbol.Locations.Any(loc => loc.IsInSource && loopSpan.Contains(loc.SourceSpan));

                if (isInsideLoop)
                {
                    var declarator = localSymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as VariableDeclaratorSyntax;
                    if (declarator?.Initializer != null)
                    {
                        return IsLoopVariableOrMember(declarator.Initializer.Value, semanticModel, loop, loopVariables);
                    }
                }
            }
        }

        if (expression is BinaryExpressionSyntax binary)
        {
            return IsLoopVariableOrMember(binary.Left, semanticModel, loop, loopVariables) || IsLoopVariableOrMember(binary.Right, semanticModel, loop, loopVariables);
        }

        if (expression is MemberAccessExpressionSyntax memberAccess)
        {
            // Support both key and keys[i]
            if (IsLoopVariableOrMember(memberAccess.Expression, semanticModel, loop, loopVariables)) 
                return true;
        }

        if (expression is ElementAccessExpressionSyntax elementAccess)
        {
             if (IsLoopVariableOrMember(elementAccess.Expression, semanticModel, loop, loopVariables)) 
                 return true;
             
             // Check if indexer uses loop variable: keys[i]
             foreach(var arg in elementAccess.ArgumentList.Arguments)
             {
                 if (IsLoopVariableOrMember(arg.Expression, semanticModel, loop, loopVariables)) return true;
             }
        }

        if (expression is ParenthesizedExpressionSyntax parenthesized)
        {
            return IsLoopVariableOrMember(parenthesized.Expression, semanticModel, loop, loopVariables);
        }

        return false;
    }

    private static bool IsDefinedOutside(SyntaxNodeAnalysisContext context, ExpressionSyntax expression, SyntaxNode loop)
    {
        if (expression is MemberAccessExpressionSyntax or InvocationExpressionSyntax)
        {
            return true;
        }

        var symbolInfo = context.SemanticModel.GetSymbolInfo(expression);
        var symbol = symbolInfo.Symbol ?? symbolInfo.CandidateSymbols.FirstOrDefault();
        
        if (symbol == null)
        {
            // If we can't resolve the symbol, assume it's defined outside
            return true;
        }

        var loopLocation = loop.GetLocation().SourceSpan;
        foreach (var location in symbol.Locations)
        {
            if (location.IsInSource && !loopLocation.Contains(location.SourceSpan))
            {
                return true;
            }
        }

        // If the symbol is a parameter of the containing method, it's defined outside the loop
        return symbol.Kind == SymbolKind.Parameter;
    }
}
