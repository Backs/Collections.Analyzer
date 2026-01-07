using System;
using System.Collections;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Collections.Analyzer.Diagnostics.CI0009;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Collections.Analyzer.CodeFixes;

using static SyntaxFactory;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SetListCapacityFromLoopCodeFix))]
public class SetListCapacityFromLoopCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ListLoopDiagnostic.Rule.Id);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var objectCreation = root!.FindNode(context.Span).FirstAncestorOrSelf<BaseObjectCreationExpressionSyntax>();

        if (objectCreation == null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                title: Resources.SetCapacity,
                createChangedDocument: c => FixAsync(context.Document, objectCreation, c),
                equivalenceKey: nameof(SetListCapacityFromLoopCodeFix)),
            context.Diagnostics.First());
    }

    private static async Task<Document> FixAsync(Document document, BaseObjectCreationExpressionSyntax objectCreation,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

        var declarator = objectCreation.FirstAncestorOrSelf<VariableDeclaratorSyntax>();
        if (declarator == null)
            return document;

        var localDeclaration = declarator.FirstAncestorOrSelf<LocalDeclarationStatementSyntax>();
        if (localDeclaration is not { Parent: BlockSyntax block })
            return document;

        var startIndex = block.Statements.IndexOf(localDeclaration);
        if (startIndex == -1)
            return document;

        StatementSyntax? loopStatement = null;
        for (var i = startIndex + 1; i < block.Statements.Count; i++)
        {
            var temp = block.Statements[i];
            if (temp is ForStatementSyntax or ForEachStatementSyntax)
            {
                loopStatement = temp;
                break;
            }
        }

        if (loopStatement == null) return document;

        ExpressionSyntax? capacityExpression = null;

        if (loopStatement is ForStatementSyntax forStatement)
        {
            // extract "array.Length" from "i < array.Length"
            if (forStatement.Condition is BinaryExpressionSyntax binary &&
                binary.IsKind(SyntaxKind.LessThanExpression))
            {
                capacityExpression = binary.Right;
            }
        }
        else if (loopStatement is ForEachStatementSyntax foreachStatement)
        {
            var collectionExpr = foreachStatement.Expression;
            var type = semanticModel!.GetTypeInfo(collectionExpr).Type;

            if (type != null)
            {
                string? propertyName = null;
                if (type.TypeKind == TypeKind.Array || HasProperty(type, nameof(Array.Length)))
                    propertyName = nameof(Array.Length);
                else if (HasProperty(type, nameof(ICollection.Count)))
                    propertyName = nameof(ICollection.Count);

                if (propertyName != null)
                {
                    capacityExpression = MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        collectionExpr.WithoutTrivia(),
                        IdentifierName(propertyName));
                }
            }
        }

        if (capacityExpression == null)
        {
            return document;
        }

        var newArgList = ArgumentList(SingletonSeparatedList(Argument(capacityExpression)));

        var newObjectCreation = objectCreation.WithArgumentList(newArgList);

        var newRoot = root!.ReplaceNode(objectCreation, newObjectCreation);
        return document.WithSyntaxRoot(newRoot);
    }

    private static bool HasProperty(ITypeSymbol type, string propertyName)
    {
        return type.GetMembers(propertyName).OfType<IPropertySymbol>().Any();
    }
}