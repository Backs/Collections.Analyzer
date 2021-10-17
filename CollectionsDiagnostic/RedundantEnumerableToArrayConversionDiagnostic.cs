namespace CollectionsDiagnostic
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
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
    public sealed class RedundantEnumerableToArrayConversionDiagnostic : DiagnosticAnalyzer
    {
        private static readonly IReadOnlyCollection<string> Methods =
        new HashSet<string>(new[] { nameof(Enumerable.ToArray), nameof(Enumerable.ToList) });

        internal static readonly DiagnosticDescriptor RedundantEnumerableToArrayRule = new(
            "CI0003",
            "Redundant enumerable conversion",
            "Redundant enumerable conversion",
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
            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            var methodSymbol = context.SemanticModel.GetSymbolInfo(invocationExpression).Symbol as IMethodSymbol;

            if (invocationExpression.Parent is ReturnStatementSyntax returnStatementSyntax
                && methodSymbol != null && invocationExpression.Expression is MemberAccessExpressionSyntax
                {
                    Expression: IdentifierNameSyntax identifier
                }
                && Methods.Contains(methodSymbol.Name)
                && context.SemanticModel.GetTypeInfo(identifier).Type.AllInterfaces.Any(o => o.Name == nameof(IEnumerable))
                && returnStatementSyntax.Parent?.Parent is MethodDeclarationSyntax methodDeclarationSyntax
                && context.SemanticModel.GetTypeInfo(methodDeclarationSyntax.ReturnType).Type?.Name == nameof(IEnumerable)
            )
            {
                context.ReportDiagnostic(Diagnostic.Create(RedundantEnumerableToArrayRule, invocationExpression.GetLocation(),
                    methodSymbol.ToString()));
            }
        }
    }
}