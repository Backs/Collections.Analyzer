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

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync();
            var diagnostic = context.Diagnostics.First();
            var objectCreationExpression = (ObjectCreationExpressionSyntax)root!.FindNode(context.Span);

            context.RegisterCodeFix(
                CodeAction.Create(
                    Resources.SetActualCapacity,
                    token => FixAsync(context.Document, objectCreationExpression, token),
                    "Set capacity"
                ),
                diagnostic
            );
        }

        private static async Task<Document> FixAsync(Document document,
            ObjectCreationExpressionSyntax objectCreationExpression,
            CancellationToken cancellationToken)
        {
            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var capacity = objectCreationExpression.Initializer!.Expressions.Count;
            var genericNameSyntax = (GenericNameSyntax) objectCreationExpression.Type;

            var newObject = ObjectCreationExpression(
                    GenericName(genericNameSyntax.Identifier)
                        .WithTypeArgumentList(genericNameSyntax.TypeArgumentList))
                .WithArgumentList(
                    ArgumentList(
                        SingletonSeparatedList(
                            Argument(
                                LiteralExpression(
                                    SyntaxKind.NumericLiteralExpression,
                                    Literal(capacity))))))
                .WithInitializer(objectCreationExpression.Initializer);

            var newRoot = oldRoot!.ReplaceNode(objectCreationExpression, newObject);

            return document.WithSyntaxRoot(newRoot);
        }


        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }
    }
}