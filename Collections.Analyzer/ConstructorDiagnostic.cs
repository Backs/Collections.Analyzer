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
    public class ConstructorDiagnostic : DiagnosticAnalyzer
    {
        internal static readonly DiagnosticDescriptor ConstructorRule = new(
            "CI0003",
            Resources.CI0003_Title,
            Resources.CI0003_Title,
            Categories.Performance,
            DiagnosticSeverity.Warning,
            true
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(ConstructorRule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ObjectCreationExpression);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var objectCreationExpression = (ObjectCreationExpressionSyntax) context.Node;

            if (objectCreationExpression.ArgumentList?.Arguments == null) return;

            foreach (var argument in objectCreationExpression.ArgumentList.Arguments)
            {
                if (argument.Expression is not InvocationExpressionSyntax invocationExpression) continue;

                if (!ExpressionExtensions.IsRedundantMethod(context, invocationExpression)) continue;

                if (ExpressionIsIEnumerable(context, invocationExpression))
                    context.ReportDiagnostic(Diagnostic.Create(ConstructorRule, invocationExpression.GetLocation(),
                        invocationExpression.ToString()));
            }
        }

        private static bool ExpressionIsIEnumerable(SyntaxNodeAnalysisContext context,
            InvocationExpressionSyntax invocationExpression)
        {
            // list.ToArray()
            var isVariable = invocationExpression.Expression is MemberAccessExpressionSyntax
                             {
                                 Expression: IdentifierNameSyntax identifier
                             }
                             && context.SemanticModel.GetTypeInfo(identifier).Type!.AllInterfaces.Any(o =>
                                 o.Name == nameof(IEnumerable));

            if (isVariable)
                return true;

            // GetList().ToArray()
            var isMethod = invocationExpression.Expression is MemberAccessExpressionSyntax
                           {
                               Expression: InvocationExpressionSyntax invocationExpressionSyntax
                           }
                           && context.SemanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol is IMethodSymbol ms
                           && ms.ReturnType.AllInterfaces.Any(o => o.Name == nameof(IEnumerable));

            return isMethod;
        }
    }
}