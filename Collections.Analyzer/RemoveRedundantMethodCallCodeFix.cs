using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Collections.Analyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveRedundantMethodCallCodeFix))]
    [Shared]
    public class RemoveRedundantMethodCallCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
            RedundantStringToArrayConversionDiagnostic.RedundantStringToArrayRule.Id,
            RedundantArrayToArrayConversionDiagnostic.RedundantArrayToArrayRule.Id,
            RedundantEnumerableToArrayConversionDiagnostic.RedundantEnumerableToArrayRule.Id);

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync();
            var diagnostic = context.Diagnostics.First();
            var invocationExpressionSyntax = root!.FindNode(context.Span) as InvocationExpressionSyntax;
            if (invocationExpressionSyntax == null)
            {
                var arg = root.FindNode(context.Span) as ArgumentSyntax;
                invocationExpressionSyntax = arg?.Expression as InvocationExpressionSyntax;
            }

            var name = (invocationExpressionSyntax?.Expression as MemberAccessExpressionSyntax)?.Name.Identifier;

            var title = string.Format(Resources.RemoveRedundantCall, name);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title,
                    token => FixAsync(context.Document, invocationExpressionSyntax, token),
                    title
                ),
                diagnostic
            );
        }

        private static async Task<Document> FixAsync(Document document,
            InvocationExpressionSyntax? originalInvocationExpression,
            CancellationToken cancellationToken)
        {
            var expression = (originalInvocationExpression?.Expression as MemberAccessExpressionSyntax)?.Expression;

            if (expression == null) return document;

            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = oldRoot!.ReplaceNode(originalInvocationExpression!, expression);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}