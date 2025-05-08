using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Collections.Analyzer.Diagnostics.CI0007;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Collections.Analyzer.CodeFixes;

using static SyntaxFactory;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ReplaceAnyWithIsEmptyCodeFix))]
[Shared]
public class ReplaceAnyWithIsEmptyCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
        ConcurrentCollectionIsEmptyDiagnostic.ReplaceAnyWithIsEmptyRule.Id);

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync();
        var diagnostic = context.Diagnostics.First();

        var syntaxNode = root!.FindNode(context.Span) as InvocationExpressionSyntax;
        if (syntaxNode?.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax) return;

        var title = Resources.ReplaceWithIsEmpty;

        context.RegisterCodeFix(
            CodeAction.Create(
                title,
                token => FixAsync(context.Document, memberAccessExpressionSyntax, token),
                title
            ),
            diagnostic
        );
    }

    private static async Task<Document> FixAsync(Document document, MemberAccessExpressionSyntax originalExpression,
        CancellationToken cancellationToken)
    {
        var newExpression = PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                originalExpression.Expression,
                IdentifierName(nameof(ConcurrentDictionary<object, object>.IsEmpty)))
            )
            .NormalizeWhitespace();

        var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
        var newRoot = oldRoot!.ReplaceNode(originalExpression.Parent, newExpression);

        return document.WithSyntaxRoot(newRoot);
    }

    public override FixAllProvider GetFixAllProvider()
    {
        return WellKnownFixAllProviders.BatchFixer;
    }
}