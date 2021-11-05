namespace CollectionsDiagnostic
{
    using System;
    using System.Collections.Generic;
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
    public class ListFromStringDiagnostic : DiagnosticAnalyzer
    {
        private static readonly IReadOnlyCollection<string> Methods =
        new HashSet<string>(new[] { nameof(Enumerable.ToArray), nameof(Enumerable.ToList) });

        internal static readonly DiagnosticDescriptor ListFromStringRule = new(
            "CI0004",
            Resources.CI0004_Title,
            Resources.CI0004_Title,
            Categories.Performance,
            DiagnosticSeverity.Warning,
            true
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(ListFromStringRule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ObjectCreationExpression);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var objectCreationExpression = (ObjectCreationExpressionSyntax)context.Node;

            if (objectCreationExpression.Type is GenericNameSyntax genericName
                && genericName.Identifier.ToString() == "List"
                && genericName.TypeArgumentList.Arguments.FirstOrDefault()?.ToString() == "char")
            {
                if (objectCreationExpression.ArgumentList?.Arguments.FirstOrDefault()?.Expression is InvocationExpressionSyntax
                    invocationExpression
                    && context.SemanticModel.GetSymbolInfo(invocationExpression).Symbol is IMethodSymbol method)
                {
                    CheckRedundantStringConversion(context, method, invocationExpression);
                }
                else if (objectCreationExpression.ArgumentList?.Arguments.FirstOrDefault()?.Expression is IdentifierNameSyntax identifier
                && context.SemanticModel.GetTypeInfo(identifier).Type?.Name == nameof(String))
                {
                    context.ReportDiagnostic(Diagnostic.Create(ListFromStringRule, identifier.GetLocation(),
                        identifier.ToString()));
                }
            }
        }

        private static void CheckRedundantStringConversion(SyntaxNodeAnalysisContext context, IMethodSymbol methodSymbol,
            InvocationExpressionSyntax invocationExpression)
        {
            if (StringExtensions.IsLinqMethodCalledOnString(context, invocationExpression, methodSymbol, Methods))
            {
                context.ReportDiagnostic(Diagnostic.Create(ListFromStringRule, invocationExpression.GetLocation(),
                    methodSymbol.ToString()));
            }
            else if (StringExtensions.IsLinqMethodCalledOnMethod(context, invocationExpression, methodSymbol, Methods))
            {
                context.ReportDiagnostic(Diagnostic.Create(ListFromStringRule, invocationExpression.GetLocation(),
                    methodSymbol.ToString()));
            }
        }
    }
}