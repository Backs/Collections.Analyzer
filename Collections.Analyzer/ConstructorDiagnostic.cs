using System.Collections;
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
    public class ConstructorDiagnostic : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(ConstructorRule);

        private static readonly IReadOnlyCollection<string> Methods =
            new HashSet<string>(new[] {nameof(Enumerable.ToArray), nameof(Enumerable.ToList)});

        internal static readonly DiagnosticDescriptor ConstructorRule = new(
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

            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ObjectCreationExpression);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var objectCreationExpression = (ObjectCreationExpressionSyntax) context.Node;

            if (objectCreationExpression.ArgumentList?.Arguments == null)
            {
                return;
            }

            foreach (var argument in objectCreationExpression.ArgumentList.Arguments)
            {
                if (argument.Expression is not InvocationExpressionSyntax invocationExpression)
                {
                    continue;
                }

                var redundantMethod = context.SemanticModel.GetSymbolInfo(invocationExpression).Symbol as IMethodSymbol;

                if (redundantMethod == null || !Methods.Contains(redundantMethod.Name))
                {
                    continue;
                }

                if (invocationExpression.Expression is MemberAccessExpressionSyntax
                    {
                        Expression: IdentifierNameSyntax identifier
                    }
                    && context.SemanticModel.GetTypeInfo(identifier).Type!.AllInterfaces.Any(o =>
                        o.Name == nameof(IEnumerable))
                   )
                {
                    context.ReportDiagnostic(Diagnostic.Create(ConstructorRule, invocationExpression.GetLocation(),
                        redundantMethod.ToString()));
                }
                else if (invocationExpression.Expression is MemberAccessExpressionSyntax
                         {
                             Expression: InvocationExpressionSyntax invocationExpressionSyntax
                         }
                         && context.SemanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol is IMethodSymbol ms
                         && ms.ReturnType.AllInterfaces.Any(o => o.Name == nameof(IEnumerable)))
                {
                    context.ReportDiagnostic(Diagnostic.Create(ConstructorRule, invocationExpression.GetLocation(),
                        redundantMethod.ToString()));
                }
            }
        }
    }
}