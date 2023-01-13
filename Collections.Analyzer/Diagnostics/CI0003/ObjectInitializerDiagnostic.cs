using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Collections.Analyzer.Diagnostics.CI0003
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
                if (expression.Left is IdentifierNameSyntax &&
                    expression.Right is InvocationExpressionSyntax rightInvocationExpression &&
                    IsArrayProperty(context, rightInvocationExpression) &&
                    ExpressionExtensions.IsRedundantMethod(context, rightInvocationExpression)
                   )
                {
                    context.ReportDiagnostic(Diagnostic.Create(RedundantArrayToArrayRule,
                        rightInvocationExpression.GetLocation(),
                        rightInvocationExpression.ToString()));
                }
            }
        }

        private static bool IsArrayProperty(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocationExpression)
        {
            return invocationExpression.Expression is MemberAccessExpressionSyntax
                   {
                       Expression: MemberAccessExpressionSyntax memberAccessExpression
                   } &&
                   context.SemanticModel.GetTypeInfo(memberAccessExpression).Type?.Kind == SymbolKind.ArrayType;
        }
    }
}