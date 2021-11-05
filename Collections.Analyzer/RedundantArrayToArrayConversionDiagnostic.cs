namespace CollectionsDiagnostic
{
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Diagnostic = Microsoft.CodeAnalysis.Diagnostic;
    using DiagnosticDescriptor = Microsoft.CodeAnalysis.DiagnosticDescriptor;
    using DiagnosticSeverity = Microsoft.CodeAnalysis.DiagnosticSeverity;
    using IMethodSymbol = Microsoft.CodeAnalysis.IMethodSymbol;
    using LanguageNames = Microsoft.CodeAnalysis.LanguageNames;
    using SyntaxKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class RedundantArrayToArrayConversionDiagnostic : DiagnosticAnalyzer
    {
        internal static readonly DiagnosticDescriptor RedundantArrayToArrayRule = new(
            "CI0002",
            Resources.CI0002_Title,
            Resources.CI0002_Title,
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

            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            var redundantMethod = context.SemanticModel.GetSymbolInfo(invocationExpression).Symbol as IMethodSymbol;
            if (redundantMethod is not { Name: nameof(Enumerable.ToArray) }
                || redundantMethod.ContainingType.Name != nameof(Enumerable))
            {
                return;
            }

            if (invocationExpression.Expression is MemberAccessExpressionSyntax
                {
                    Expression: IdentifierNameSyntax identifier
                }
                && context.SemanticModel.GetTypeInfo(identifier).Type?.TypeKind == TypeKind.Array)
            {
                context.ReportDiagnostic(Diagnostic.Create(RedundantArrayToArrayRule, invocationExpression.GetLocation(),
                    redundantMethod.ToString()));
            }
            else if (invocationExpression.Expression is MemberAccessExpressionSyntax
                     {
                         Expression: InvocationExpressionSyntax invocationExpressionSyntax
                     }
                     && context.SemanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol is IMethodSymbol ms
                     && ms.ReturnType.TypeKind == TypeKind.Array)
            {
                context.ReportDiagnostic(Diagnostic.Create(RedundantArrayToArrayRule, invocationExpression.GetLocation(),
                    redundantMethod.ToString()));
            }
        }
    }
}