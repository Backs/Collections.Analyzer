using System.Collections.Frozen;
using System.Collections.Generic;
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

    private static readonly FrozenSet<string> TargetMethods = new HashSet<string>()
    {
        nameof(Enumerable.First),
        nameof(Enumerable.FirstOrDefault),
        nameof(Enumerable.Single),
        nameof(Enumerable.SingleOrDefault),
        nameof(Enumerable.Any)
    }.ToFrozenSet();

    private static readonly FrozenSet<string> LinqScopeMethods = new HashSet<string>()
    {
        nameof(Enumerable.Select),
        nameof(Enumerable.Where),
        nameof(Enumerable.Any),
        nameof(Enumerable.All),
        nameof(Enumerable.Count),
        nameof(Enumerable.First),
        nameof(Enumerable.FirstOrDefault),
        nameof(Enumerable.Single),
        nameof(Enumerable.SingleOrDefault)
    }.ToFrozenSet();

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSyntaxNodeAction(AnalyzeLoop, SyntaxKind.ForEachStatement, SyntaxKind.ForStatement);
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeLoop(SyntaxNodeAnalysisContext context)
    {
        var loop = context.Node;
        var loopBody = loop switch
        {
            ForEachStatementSyntax foreachStatement => foreachStatement.Statement,
            ForStatementSyntax forStatement => forStatement.Statement,
            _ => null
        };

        if (loopBody == null) return;

        var loopVariables = GetLoopVariables(loop);
        if (loopVariables.Length == 0) return;

        AnalyzeInvocations(context, loopBody.DescendantNodes().OfType<InvocationExpressionSyntax>(), loop, loopVariables);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not InvocationExpressionSyntax invocation) return;
        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess) return;

        var methodName = memberAccess.Name.Identifier.Text;
        if (!LinqScopeMethods.Contains(methodName)) return;

        if (invocation.ArgumentList.Arguments.Count == 0) return;
        var argument = invocation.ArgumentList.Arguments[0];

        if (argument.Expression is not LambdaExpressionSyntax lambda) return;

        var loopVariables = GetLambdaParameters(lambda);
        if (loopVariables.Length == 0) return;

        var body = lambda is SimpleLambdaExpressionSyntax simple ? simple.Body : (lambda as ParenthesizedLambdaExpressionSyntax)?.Body;
        if (body == null) return;

        AnalyzeInvocations(context, body.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>(), invocation, loopVariables);
    }

    private static void AnalyzeInvocations(SyntaxNodeAnalysisContext context, IEnumerable<InvocationExpressionSyntax> invocations, SyntaxNode scopeNode, string[] loopVariables)
    {
        foreach (var invocation in invocations)
        {
            if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess) continue;

            if (!TargetMethods.Contains(memberAccess.Name.Identifier.Text)) continue;

            if (invocation.ArgumentList.Arguments.Count != 1) continue;
            var argument = invocation.ArgumentList.Arguments[0];

            if (argument.Expression is not LambdaExpressionSyntax lambda) continue;

            if (!IsSearchingByLoopVariable(lambda, context.SemanticModel, scopeNode, loopVariables)) continue;

            var collectionExpression = memberAccess.Expression;
            if (collectionExpression is not IdentifierNameSyntax) continue;

            if (!IsDefinedOutside(context, collectionExpression, scopeNode)) continue;

            var typeInfo = context.SemanticModel.GetTypeInfo(collectionExpression);
            if (typeInfo.Type != null && IsIQueryable(typeInfo.Type)) continue;

            context.ReportDiagnostic(Diagnostic.Create(Rule, invocation.GetLocation(), collectionExpression.ToString()));
        }
    }

    private static bool IsIQueryable(ITypeSymbol type)
    {
        return type.Name == nameof(IQueryable) || type.AllInterfaces.Any(i => i.Name == nameof(IQueryable));
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
        var parameters = GetLambdaParameters(lambda);
        if (parameters.Length == 0) return false;
        var primaryParameter = parameters[0];

        if (expression is MemberAccessExpressionSyntax memberAccess)
        {
            return memberAccess.Expression is IdentifierNameSyntax id && id.Identifier.Text == primaryParameter;
        }

        return expression is IdentifierNameSyntax identifier && identifier.Identifier.Text == primaryParameter;
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
        var symbol = context.SemanticModel.GetSymbolInfo(expression).Symbol;
        if (symbol == null) return true;

        var loopLocation = loop.GetLocation().SourceSpan;
        return symbol.Locations.Any(location => location.IsInSource && !loopLocation.Contains(location.SourceSpan)) ||
               symbol.Kind == SymbolKind.Parameter;
    }
}
