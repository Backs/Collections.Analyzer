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
    public sealed class RedundantEnumerableToArrayConversionDiagnostic : DiagnosticAnalyzer
    {
        internal static readonly DiagnosticDescriptor RedundantEnumerableToArrayRule = new(
            "CI0003",
            Resources.CI0003_Title,
            Resources.CI0003_Title,
            Categories.Performance,
            DiagnosticSeverity.Warning,
            true
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(RedundantEnumerableToArrayRule);

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

            if (invocationExpression.Parent is not ReturnStatementSyntax) return;

            if (invocationExpression.Expression is MemberAccessExpressionSyntax
                {
                    Expression: IdentifierNameSyntax identifier
                }
                && context.SemanticModel.GetTypeInfo(identifier).Type!.AllInterfaces.Any(o =>
                    o.Name == nameof(IEnumerable))
                && FindMethodDeclarationSyntax(invocationExpression) is { } methodDeclarationSyntax
                && context.SemanticModel.GetTypeInfo(methodDeclarationSyntax.ReturnType).Type?.Name ==
                nameof(IEnumerable)
               )
                context.ReportDiagnostic(Diagnostic.Create(RedundantEnumerableToArrayRule,
                    invocationExpression.GetLocation(),
                    invocationExpression.ToString()));
            else if (invocationExpression.Expression is MemberAccessExpressionSyntax
                     {
                         Expression: InvocationExpressionSyntax invocationExpressionSyntax
                     }
                     && FindMethodDeclarationSyntax(invocationExpressionSyntax) is { } methodDeclaration
                     && context.SemanticModel.GetTypeInfo(methodDeclaration.ReturnType).Type?.Name ==
                     nameof(IEnumerable)
                     && context.SemanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol is IMethodSymbol ms
                     && ms.ReturnType.AllInterfaces.Any(o => o.Name == nameof(IEnumerable)))
                context.ReportDiagnostic(Diagnostic.Create(RedundantEnumerableToArrayRule,
                    invocationExpression.GetLocation(),
                    invocationExpression.ToString()));
        }

        private static MethodDeclarationSyntax? FindMethodDeclarationSyntax(SyntaxNode? node)
        {
            while (node is not null and not MethodDeclarationSyntax) node = node.Parent;

            return node as MethodDeclarationSyntax;
        }
    }
}