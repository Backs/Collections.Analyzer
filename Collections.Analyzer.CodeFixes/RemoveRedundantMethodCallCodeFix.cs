using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Collections.Analyzer.Diagnostics.CI0001;
using Collections.Analyzer.Diagnostics.CI0002;
using Collections.Analyzer.Diagnostics.CI0003;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Collections.Analyzer.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveRedundantMethodCallCodeFix))]
    [Shared]
    public class RemoveRedundantMethodCallCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
            StringToArrayDiagnostic.RedundantStringToArrayRule.Id,
            ArrayToArrayDiagnostic.RedundantArrayToArrayRule.Id,
            EnumerableToArrayOnReturnDiagnostic.RedundantEnumerableToArrayRule.Id,
            AddRangeDiagnostic.AddRangeRule.Id,
            AssignEnumerableDiagnostic.AssignEnumerableRule.Id,
            ConstructorDiagnostic.ConstructorRule.Id,
            ObjectInitializerDiagnostic.RedundantArrayToArrayRule.Id,
            EnumerableToArrayOnReturnDiagnostic.RedundantEnumerableToArrayRule.Id,
            StringJoinToArrayDiagnostic.StringJoinToArrayRule.Id);

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