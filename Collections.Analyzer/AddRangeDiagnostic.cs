using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Collections.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AddRangeDiagnostic : DiagnosticAnalyzer
    {
        private static readonly IReadOnlyCollection<string> Methods =
            new HashSet<string>(new[] {nameof(Enumerable.ToArray), nameof(Enumerable.ToList)});

        internal static readonly DiagnosticDescriptor AddRangeRule = new(
            "CI0003",
            Resources.CI0003_Title,
            Resources.CI0003_Title,
            Categories.Performance,
            DiagnosticSeverity.Warning,
            true
        );

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            if (invocationExpression.Expression is MemberAccessExpressionSyntax memberAccessExpression && memberAccessExpression.Name.ToString() == nameof(List<object>.AddRange) &&
                invocationExpression.ArgumentList.Arguments.ElementAtOrDefault(0)?.Expression is
                    InvocationExpressionSyntax identifier)
            {
                if (context.SemanticModel.GetSymbolInfo(identifier).Symbol is IMethodSymbol redundantMethod &&
                    Methods.Contains(redundantMethod.Name))
                {
                    context.ReportDiagnostic(Diagnostic.Create(AddRangeRule, identifier.GetLocation(),
                        identifier.ToString()));
                }
            }
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(AddRangeRule);
    }
}