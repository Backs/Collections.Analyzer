using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Collections.Analyzer.Diagnostics.CI0004;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Collections.Analyzer.CodeFixes
{
    using static SyntaxFactory;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddToCharArrayCodeFix))]
    [Shared]
    public class AddToCharArrayCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
            ListFromStringDiagnostic.ListFromStringRule.Id);

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync();
            var diagnostic = context.Diagnostics.First();

            var arg = root!.FindNode(context.Span) as ArgumentSyntax;

            var title = arg?.Expression is IdentifierNameSyntax
                ? Resources.AddToCharArray
                : Resources.ReplaceWithToCharArray;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title,
                    token => FixAsync(context.Document, arg?.Expression, token),
                    title
                ),
                diagnostic
            );
        }

        private static async Task<Document> FixAsync(Document document, ExpressionSyntax? originalExpression,
            CancellationToken cancellationToken)
        {
            var identifier = originalExpression as IdentifierNameSyntax;

            if (identifier == null)
                identifier =
                    ((originalExpression as InvocationExpressionSyntax)?.Expression as MemberAccessExpressionSyntax)
                    ?.Expression as IdentifierNameSyntax;

            if (identifier == null) return document;

            var newExpression = InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(identifier.Identifier),
                    IdentifierName(nameof(string.ToCharArray))));

            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = oldRoot!.ReplaceNode(originalExpression!, newExpression);

            return document.WithSyntaxRoot(newRoot);
        }

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }
    }
}