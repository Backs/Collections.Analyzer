using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Collections.Analyzer.Diagnostics.CI0006;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Collections.Analyzer.CodeFixes
{
    using static SyntaxFactory;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SetListCapacityCodeFix))]
    [Shared]
    public class SetListCapacityCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
            ListInitializerDiagnostic.InitializeListWithCapacityRule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var span = context.Span;

            var objectCreation = root!.FindNode(span).FirstAncestorOrSelf<BaseObjectCreationExpressionSyntax>();

            if (objectCreation == null)
                return;

            context.RegisterCodeFix(
                CodeAction.Create(
                    Resources.SetActualCapacity,
                    token => FixAsync(context.Document, objectCreation, token),
                    nameof(SetListCapacityCodeFix)
                ),
                diagnostic
            );
        }

        private static async Task<Document> FixAsync(Document document,
            BaseObjectCreationExpressionSyntax objectCreation,
            CancellationToken cancellationToken)
        {
            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            
            var count = objectCreation.Initializer?.Expressions.Count ?? 0;

            var newArgumentList = ArgumentList(
                SingletonSeparatedList(
                    Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(count)))));

            BaseObjectCreationExpressionSyntax newObjectCreation;

            switch (objectCreation)
            {
                case ImplicitObjectCreationExpressionSyntax implicitCreation:
                    newObjectCreation = implicitCreation.WithArgumentList(newArgumentList);
                    break;
                case ObjectCreationExpressionSyntax explicitCreation:
                    newObjectCreation = explicitCreation.WithArgumentList(newArgumentList);
                    break;
                default:
                    return document;
            }

            var newRoot = oldRoot!.ReplaceNode(objectCreation, newObjectCreation);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
