using System.Collections;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Collections.Analyzer.Diagnostics.CI0003
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class AssignEnumerableDiagnostic : DiagnosticAnalyzer
    {
        internal static readonly DiagnosticDescriptor AssignEnumerableRule = new(
            "CI0003",
            Resources.CI0003_Title,
            Resources.CI0003_Title,
            Categories.Performance,
            DiagnosticSeverity.Warning,
            true
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(AssignEnumerableRule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.VariableDeclaration);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var declarationExpression = (VariableDeclarationSyntax) context.Node;

            foreach (var variable in declarationExpression.Variables)
            {
                if (context.SemanticModel.GetDeclaredSymbol(variable) is ILocalSymbol localSymbol &&
                    localSymbol.Type.Name == nameof(IEnumerable)
                    && variable.Initializer?.Value is InvocationExpressionSyntax invocationExpression &&
                    ExpressionExtensions.IsRedundantMethod(context, invocationExpression))
                {
                    context.ReportDiagnostic(Diagnostic.Create(AssignEnumerableRule, invocationExpression.GetLocation(),
                        invocationExpression.ToString()));
                }
            }
        }
    }
}