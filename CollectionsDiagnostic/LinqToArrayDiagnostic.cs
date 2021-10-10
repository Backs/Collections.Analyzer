namespace CollectionsDiagnostic
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
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
    public class LinqToArrayDiagnostic : DiagnosticAnalyzer
    {
        internal static readonly DiagnosticDescriptor ToArrayErrorRule = new(
            "CI0003",
            Resources.CI0003_Title,
            Resources.CI0003_Title,
            Categories.Performance,
            DiagnosticSeverity.Warning,
            true
        );

        internal static readonly DiagnosticDescriptor ToCharArrayErrorRule = new(
            "CI0004",
            Resources.CI0004_Title,
            Resources.CI0004_Title,
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

        private static readonly IReadOnlyCollection<string> methods = new HashSet<string>(new[] { "ToArray", "ToList" });
        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            var symbol = context.SemanticModel.GetSymbolInfo(invocationExpression).Symbol;

            if (invocationExpression.Parent is MemberAccessExpressionSyntax parentExpression
                && context.SemanticModel.GetSymbolInfo(parentExpression).Symbol?.ContainingType.Name == "Enumerable"
                && symbol is IMethodSymbol methodSymbol)
            {
                if (StringExtensions.IsTypeMethodCalled(methodSymbol))
                {
                    context.ReportDiagnostic(Diagnostic.Create(ToCharArrayErrorRule, invocationExpression.GetLocation(),
                        methodSymbol.ToString()));
                }
                else if (LinqExtensions.IsLinqMethodCalled(context, invocationExpression, methodSymbol, "String", methods))
                {
                    context.ReportDiagnostic(Diagnostic.Create(ToArrayErrorRule, invocationExpression.GetLocation(),
                        methodSymbol.ToString()));
                }
            }
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(ToArrayErrorRule, ToCharArrayErrorRule);
    }
}