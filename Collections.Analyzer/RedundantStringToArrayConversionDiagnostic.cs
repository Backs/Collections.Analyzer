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
    public sealed class RedundantStringToArrayConversionDiagnostic : DiagnosticAnalyzer
    {
        private static readonly IReadOnlyCollection<string> Methods =
            new HashSet<string>(new[] {nameof(Enumerable.ToArray), nameof(Enumerable.ToList)});

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
            var invocationExpression = (InvocationExpressionSyntax) context.Node;

            var redundantMethod = context.SemanticModel.GetSymbolInfo(invocationExpression).Symbol as IMethodSymbol;

            if (redundantMethod == null) return;

            if (invocationExpression.Parent is ForEachStatementSyntax)
                CheckRedundantStringConversion(context, redundantMethod, invocationExpression);
            else if (invocationExpression.Parent is MemberAccessExpressionSyntax parentExpression
                     && context.SemanticModel.GetSymbolInfo(parentExpression).Symbol?.ContainingType.Name ==
                     nameof(Enumerable))
                CheckRedundantStringConversion(context, redundantMethod, invocationExpression);
        }

        private static void CheckRedundantStringConversion(SyntaxNodeAnalysisContext context,
            IMethodSymbol methodSymbol,
            InvocationExpressionSyntax invocationExpression)
        {
            if (StringExtensions.IsTypeMethodCalled(methodSymbol))
                context.ReportDiagnostic(Diagnostic.Create(RedundantStringToArrayRule,
                    invocationExpression.GetLocation(),
                    methodSymbol.ToString()));
            else if (StringExtensions.IsLinqMethodCalledOnString(context, invocationExpression, methodSymbol, Methods))
                context.ReportDiagnostic(Diagnostic.Create(RedundantStringToArrayRule,
                    invocationExpression.GetLocation(),
                    methodSymbol.ToString()));
            else if (StringExtensions.IsLinqMethodCalledOnMethod(context, invocationExpression, methodSymbol, Methods))
                context.ReportDiagnostic(Diagnostic.Create(RedundantStringToArrayRule,
                    invocationExpression.GetLocation(),
                    methodSymbol.ToString()));
        }
    }
}