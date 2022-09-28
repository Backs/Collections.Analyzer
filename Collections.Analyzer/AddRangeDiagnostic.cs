using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Collections.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AddRangeDiagnostic : DiagnosticAnalyzer
    {
        internal static readonly DiagnosticDescriptor AddRangeRule = new(
            "CI0003",
            Resources.CI0003_Title,
            Resources.CI0003_Title,
            Categories.Performance,
            DiagnosticSeverity.Warning,
            true
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(AddRangeRule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax) context.Node;

            if (!ExpressionExtensions.IsRedundantMethod(context, invocationExpression)) return;

            if (invocationExpression.Parent is ArgumentSyntax argument &&
                argument.Parent is ArgumentListSyntax argumentList &&
                argumentList.Parent is InvocationExpressionSyntax identifier &&
                identifier.Expression is MemberAccessExpressionSyntax memberAccessExpression &&
                memberAccessExpression.Name.ToString() == nameof(List<object>.AddRange))
                context.ReportDiagnostic(Diagnostic.Create(AddRangeRule, invocationExpression.GetLocation(),
                    invocationExpression.ToString()));
        }
    }
}