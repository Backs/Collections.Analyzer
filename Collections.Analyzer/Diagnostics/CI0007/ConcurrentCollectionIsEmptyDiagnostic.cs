using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Collections.Analyzer.Diagnostics.CI0007;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ConcurrentCollectionIsEmptyDiagnostic : DiagnosticAnalyzer
{
    internal static readonly DiagnosticDescriptor ReplaceAnyWithIsEmptyRule = new(
        "CI0007",
        Resources.CI0007_Title,
        Resources.CI0007_Title,
        Categories.Performance,
        DiagnosticSeverity.Warning,
        true
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(ReplaceAnyWithIsEmptyRule);

    private static readonly HashSet<string> ConcurrentTypes = new(new[]
    {
        "ConcurrentDictionary",
        "ConcurrentBag",
        "ConcurrentQueue",
        "ConcurrentStack"
    });

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var invocationExpression = (InvocationExpressionSyntax)context.Node;

        var redundantMethod = context.SemanticModel.GetSymbolInfo(invocationExpression).Symbol as IMethodSymbol;
        if (redundantMethod is not { Name: nameof(Enumerable.Any) }
            || redundantMethod.ContainingType.Name != nameof(Enumerable)
            || redundantMethod.Parameters.Length != 0)
            return;

        if (invocationExpression.Expression is MemberAccessExpressionSyntax
            {
                Expression: IdentifierNameSyntax identifier
            } && context.SemanticModel.GetTypeInfo(identifier).Type != null
              && ConcurrentTypes.Contains(context.SemanticModel.GetTypeInfo(identifier).Type.Name)
           )
        {
            context.ReportDiagnostic(Diagnostic.Create(ReplaceAnyWithIsEmptyRule,
                invocationExpression.GetLocation(),
                redundantMethod.ToString()));
        }
        else if (invocationExpression.Expression is MemberAccessExpressionSyntax
                 {
                     Expression: InvocationExpressionSyntax invocationExpressionSyntax
                 }
                 && context.SemanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol is IMethodSymbol ms
                 && ConcurrentTypes.Contains(ms.ReturnType.Name))
        {
            context.ReportDiagnostic(Diagnostic.Create(ReplaceAnyWithIsEmptyRule,
                invocationExpression.GetLocation(),
                redundantMethod.ToString()));
        }
    }
}