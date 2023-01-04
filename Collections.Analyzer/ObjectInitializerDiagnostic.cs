using System.Collections;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Collections.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ObjectInitializerDiagnostic : DiagnosticAnalyzer
    {
        internal static readonly DiagnosticDescriptor RedundantArrayToArrayRule = new(
            "CI0003",
            Resources.CI0003_Title,
            Resources.CI0003_Title,
            Categories.Performance,
            DiagnosticSeverity.Warning,
            true
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(RedundantArrayToArrayRule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ObjectInitializerExpression);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var initializerExpressionSyntax = (InitializerExpressionSyntax) context.Node;

            foreach (var expression in initializerExpressionSyntax.Expressions.OfType<AssignmentExpressionSyntax>())
            {
                if (expression.Right is InvocationExpressionSyntax invocationExpression &&
                    ExpressionExtensions.IsRedundantMethod(context, invocationExpression)
                    && expression.Left is IdentifierNameSyntax identifier)
                {
                    if (context.SemanticModel.GetTypeInfo(identifier).Type!.AllInterfaces.Any(o =>
                            o.Name == nameof(IEnumerable)))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(RedundantArrayToArrayRule, invocationExpression.GetLocation(),
                            invocationExpression.ToString()));
                    }
                }
            }
        }
    }
}