using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Collections.Analyzer.Diagnostics.CI0006
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ListInitializerDiagnostic : DiagnosticAnalyzer
    {
        internal static readonly DiagnosticDescriptor InitializeListWithCapacityRule = new(
            "CI0006",
            Resources.CI0006_Title,
            "{0}",
            Categories.Performance,
            DiagnosticSeverity.Warning,
            true
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(InitializeListWithCapacityRule);

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
                && objectCreationExpression.Initializer?.Expressions.Count > 0)
            {
                if (objectCreationExpression.ArgumentList == null ||
                    objectCreationExpression.ArgumentList?.Arguments.Count == 0)
                {
                    context.ReportDiagnostic(Diagnostic.Create(InitializeListWithCapacityRule,
                        objectCreationExpression.GetLocation(), Resources.CI0006_Title));
                }
                else if (LessCapacity(context, objectCreationExpression))
                {
                    context.ReportDiagnostic(Diagnostic.Create(InitializeListWithCapacityRule,
                        objectCreationExpression.GetLocation(), Resources.NotEnoughCapacity));
                }
            }
        }

        private static bool LessCapacity(SyntaxNodeAnalysisContext context,
            BaseObjectCreationExpressionSyntax objectCreationExpression)
        {
            var argument = objectCreationExpression.ArgumentList!.Arguments.First();
            if (argument.Expression is not LiteralExpressionSyntax) return false;

            var capacity = context.SemanticModel.GetConstantValue(argument.Expression).Value as int?;

            return capacity < objectCreationExpression.Initializer?.Expressions.Count;
        }
    }
}