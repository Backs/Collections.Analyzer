using System;
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
    public sealed class RedundantToArrayLengthDiagnostic : DiagnosticAnalyzer
    {
        internal static readonly DiagnosticDescriptor RedundantToArrayLengthRule = new(
            "CI0005",
            Resources.CI0005_Title,
            Resources.CI0005_Title,
            Categories.Performance,
            DiagnosticSeverity.Warning,
            true
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(RedundantToArrayLengthRule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax) context.Node;

            if (context.SemanticModel.GetSymbolInfo(invocationExpression).Symbol is not IMethodSymbol redundantMethod)
                return;

            if (invocationExpression.Parent is not MemberAccessExpressionSyntax parent) return;

            switch (redundantMethod.Name)
            {
                case nameof(Enumerable.ToArray) when parent.Name.Identifier.ToString() == nameof(Array.Length):
                case nameof(Enumerable.ToList) when parent.Name.Identifier.ToString() == nameof(List<object>.Count):
                    context.ReportDiagnostic(Diagnostic.Create(RedundantToArrayLengthRule,
                        invocationExpression.Parent.GetLocation(),
                        redundantMethod.ToString()));
                    break;
            }
        }
    }
}