using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Collections.Analyzer.Diagnostics.CI0003
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

            var constructor = context.SemanticModel.GetSymbolInfo(objectCreationExpression).Symbol as IMethodSymbol;

            if (constructor == null) return;

            var parameters = constructor
                .Parameters;

            for (var i = 0; i < objectCreationExpression.ArgumentList.Arguments.Count; i++)
            {
                var argument = objectCreationExpression.ArgumentList.Arguments[i];
                if (argument.Expression is not InvocationExpressionSyntax invocationExpression) continue;

                if (!ExpressionExtensions.IsRedundantMethod(context, invocationExpression)) continue;

                var callerType = ExpressionExtensions.GetCallerType(context, invocationExpression);
                if (callerType == null)
                    continue;

                var parameter = parameters[i];

                if (SymbolEqualityComparer.Default.Equals(callerType, parameter.Type) ||
                    callerType.AllInterfaces.Contains(parameter.Type))
                {
                    context.ReportDiagnostic(Diagnostic.Create(ConstructorRule, invocationExpression.GetLocation(),
                        invocationExpression.ToString()));
                }
            }
        }
    }
}