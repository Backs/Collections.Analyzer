namespace CollectionsDiagnostic
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
    
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveToArrayCodeFix)), Shared]
    public class RemoveToArrayCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
            ForeachStringToArrayDiagnostic.ToCharArrayErrorRule.Id, 
            ForeachStringToArrayDiagnostic.ToArrayErrorRule.Id,
            LinqToArrayDiagnostic.ToArrayErrorRule.Id,
            LinqToArrayDiagnostic.ToCharArrayErrorRule.Id);

        public override FixAllProvider GetFixAllProvider() => null;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync();
            var diagnostic = context.Diagnostics.First();
            var invocationExpressionSyntax = (InvocationExpressionSyntax)root.FindNode(context.Span);

            var name = (invocationExpressionSyntax.Expression as MemberAccessExpressionSyntax)?.Name.Identifier;
            
            var title = $"Удалить вызов метода {name}";

            context.RegisterCodeFix(
                CodeAction.Create(
                    title,
                    token => FixAsync(context.Document, invocationExpressionSyntax, token),
                    title
                ),
                diagnostic
            );
        }

        private static async Task<Document> FixAsync(Document document, InvocationExpressionSyntax originalInvocationExpression,
            CancellationToken cancellationToken)
        {
            var name = ((originalInvocationExpression.Expression as MemberAccessExpressionSyntax)?.Expression as IdentifierNameSyntax)?.Identifier.Text;

            if (name == null)
            {
                return document;
            }

            var modifiedInvocationExpression = IdentifierName(name);

            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = oldRoot.ReplaceNode(originalInvocationExpression, modifiedInvocationExpression);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}