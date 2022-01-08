namespace Collections.Analyzer
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ReplaceWithCountCodeFix)), Shared]
    public class ReplaceWithCountCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
            RedundantToArrayLengthDiagnostic.RedundantToArrayLengthRule.Id);

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync();
            var diagnostic = context.Diagnostics.First();

            var syntaxNode = root!.FindNode(context.Span) as MemberAccessExpressionSyntax;
            if (syntaxNode?.Expression is not InvocationExpressionSyntax invocationExpressionSyntax)
            {
                return;
            }

            var title = Resources.ReplaceWithCountCall;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title,
                    token => FixAsync(context.Document, invocationExpressionSyntax, token),
                    title
                ),
                diagnostic
            );
        }

        private static async Task<Document> FixAsync(Document document, InvocationExpressionSyntax originalExpression,
            CancellationToken cancellationToken)
        {
            var internalExpression = (originalExpression.Expression as MemberAccessExpressionSyntax)?.Expression;

            if (internalExpression == null || originalExpression.Parent == null)
            {
                return document;
            }

            var newExpression = InvocationExpression(
                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                    internalExpression,
                    IdentifierName(nameof(Enumerable.Count))));

            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = oldRoot!.ReplaceNode(originalExpression.Parent, newExpression);

            return document.WithSyntaxRoot(newRoot);
        }

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;
    }
}