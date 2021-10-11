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
    public class RedundantStringToArrayConversionDiagnostic : DiagnosticAnalyzer
    {
        private static readonly IReadOnlyCollection<string> Methods = new HashSet<string>(new[] { "ToArray", "ToList" });

        internal static readonly DiagnosticDescriptor RedundantStringToArrayRule = new(
            "CI0001",
            Resources.CI0001_Title,
            Resources.CI0001_Title,
            Categories.Performance,
            DiagnosticSeverity.Warning,
            true
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(RedundantStringToArrayRule);

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

            if (invocationExpression.Parent is ForEachStatementSyntax && methodSymbol != null)
            {
                if (StringExtensions.IsTypeMethodCalled(methodSymbol))
                {
                    context.ReportDiagnostic(Diagnostic.Create(RedundantStringToArrayRule, invocationExpression.GetLocation(),
                        methodSymbol.ToString()));
                }
                else if (LinqExtensions.IsLinqMethodCalled(context, invocationExpression, methodSymbol, "String", Methods))
                {
                    context.ReportDiagnostic(Diagnostic.Create(RedundantStringToArrayRule, invocationExpression.GetLocation(),
                        methodSymbol.ToString()));
                }
            }
            else if (invocationExpression.Parent is MemberAccessExpressionSyntax parentExpression
                     && methodSymbol != null
                     && context.SemanticModel.GetSymbolInfo(parentExpression).Symbol?.ContainingType.Name == "Enumerable")
            {
                if (StringExtensions.IsTypeMethodCalled(methodSymbol))
                {
                    context.ReportDiagnostic(Diagnostic.Create(RedundantStringToArrayRule, invocationExpression.GetLocation(),
                        methodSymbol.ToString()));
                }
                else if (LinqExtensions.IsLinqMethodCalled(context, invocationExpression, methodSymbol, "String", Methods))
                {
                    context.ReportDiagnostic(Diagnostic.Create(RedundantStringToArrayRule, invocationExpression.GetLocation(),
                        methodSymbol.ToString()));
                }
            }
        }
    }
}