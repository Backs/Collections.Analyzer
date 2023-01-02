using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Collections.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StringJoinToArrayDiagnostic : DiagnosticAnalyzer
    {
        internal static readonly DiagnosticDescriptor StringJoinToArrayRule = new(
            "CI0003",
            Resources.CI0003_Title,
            Resources.CI0003_Title,
            Categories.Performance,
            DiagnosticSeverity.Warning,
            true
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(StringJoinToArrayRule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax) context.Node;

            if (invocationExpression.Expression is MemberAccessExpressionSyntax
                {
                    Expression: PredefinedTypeSyntax
                } memberAccessExpression && memberAccessExpression.Name.ToString() == nameof(string.Join) &&
                invocationExpression.ArgumentList.Arguments.ElementAtOrDefault(1)?.Expression is
                    InvocationExpressionSyntax identifier)
                if (ExpressionExtensions.IsRedundantMethod(context, identifier))
                    context.ReportDiagnostic(Diagnostic.Create(StringJoinToArrayRule, identifier.GetLocation(),
                        identifier.ToString()));
        }
    }
}