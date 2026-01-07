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

            context.RegisterSyntaxNodeAction(Analyze,
                SyntaxKind.ObjectCreationExpression,
                SyntaxKind.ImplicitObjectCreationExpression);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var objectCreation = (BaseObjectCreationExpressionSyntax)context.Node;

            if (objectCreation.Initializer == null || objectCreation.Initializer.Expressions.Count == 0)
            {
                return;
            }

            var typeInfo = context.SemanticModel.GetTypeInfo(objectCreation);

            if (typeInfo.Type is not INamedTypeSymbol type)
                return;

            var listTypeSymbol = context.Compilation.GetTypeByMetadataName("System.Collections.Generic.List`1");

            if (!SymbolEqualityComparer.Default.Equals(type.OriginalDefinition, listTypeSymbol))
            {
                return;
            }

            if (objectCreation.ArgumentList == null || objectCreation.ArgumentList.Arguments.Count == 0)
            {
                context.ReportDiagnostic(Diagnostic.Create(InitializeListWithCapacityRule,
                    objectCreation.GetLocation(), Resources.CI0006_Title));
            }
            else if (HasInsufficientCapacity(context, objectCreation))
            {
                context.ReportDiagnostic(Diagnostic.Create(InitializeListWithCapacityRule,
                    objectCreation.GetLocation(), Resources.NotEnoughCapacity));
            }
        }

        private static bool HasInsufficientCapacity(SyntaxNodeAnalysisContext context,
            BaseObjectCreationExpressionSyntax objectCreation)
        {
            if (context.SemanticModel.GetSymbolInfo(objectCreation).Symbol is not IMethodSymbol symbol ||
                symbol.Parameters.Length != 1 ||
                symbol.Parameters[0].Type.SpecialType != SpecialType.System_Int32)
            {
                return false;
            }

            var argument = objectCreation.ArgumentList!.Arguments[0];
            var constantValue = context.SemanticModel.GetConstantValue(argument.Expression);

            if (!constantValue.HasValue || constantValue.Value is not int capacity)
            {
                return false;
            }

            return capacity < objectCreation.Initializer!.Expressions.Count;
        }
    }
}