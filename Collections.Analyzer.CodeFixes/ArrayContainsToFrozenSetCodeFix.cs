using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Collections.Analyzer.Diagnostics;
using Collections.Analyzer.Diagnostics.CI0008;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Collections.Analyzer.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ArrayContainsToFrozenSetCodeFix)), Shared]
public class ArrayContainsToFrozenSetCodeFix : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds => [ArrayContainsToHashSetDiagnostic.Rule.Id];

    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
        if (semanticModel == null || !semanticModel.Compilation.SupportsFrozenSet())
            return;

        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var token = root?.FindToken(diagnosticSpan.Start);
        if (!token.HasValue)
            return;

        // fields and local variables
        var variableDeclarator = token.Value.Parent?.FirstAncestorOrSelf<VariableDeclaratorSyntax>();
        if (variableDeclarator != null)
        {
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Resources.CI0008_FrozenSetCodeFixTitle,
                    createChangedDocument: c => ReplaceArrayWithFrozenSetInVariable(context.Document, variableDeclarator, c),
                    equivalenceKey: nameof(ArrayContainsToFrozenSetCodeFix)),
                diagnostic);
            return;
        }

        // properties
        var propertyDeclaration = token.Value.Parent?.FirstAncestorOrSelf<PropertyDeclarationSyntax>();
        if (propertyDeclaration != null)
        {
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Resources.CI0008_FrozenSetCodeFixTitle,
                    createChangedDocument: c => ReplaceArrayWithFrozenSetInProperty(context.Document, propertyDeclaration, c),
                    equivalenceKey: nameof(ArrayContainsToFrozenSetCodeFix)),
                diagnostic);
        }
    }

    private static async Task<Document> ReplaceArrayWithFrozenSetInVariable(
        Document document,
        VariableDeclaratorSyntax variableDeclarator,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

        if (root == null || semanticModel == null)
            return document;

        var variableDeclaration = variableDeclarator.Parent as VariableDeclarationSyntax;
        if (variableDeclaration == null)
            return document;

        var typeInfo = semanticModel.GetTypeInfo(variableDeclaration.Type, cancellationToken);
        if (typeInfo.Type is not IArrayTypeSymbol arrayType)
            return document;

        var elementType = arrayType.ElementType;
        var elementTypeName = elementType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

        // get new type
        TypeSyntax newType;
        var isFieldDeclaration = variableDeclaration.Parent is FieldDeclarationSyntax;

        if (variableDeclaration.Type.IsVar && !isFieldDeclaration && variableDeclarator.Initializer?.Value is not CollectionExpressionSyntax)
        {
            // user var
            newType = variableDeclaration.Type;
        }
        else
        {
            newType = CreateFrozenSetType(elementTypeName);
        }

        var newInitializer = CreateFrozenSetInitializer(variableDeclarator.Initializer, elementTypeName);
        if (newInitializer == null)
            return document;

        var newVariableDeclarator = variableDeclarator.WithInitializer(newInitializer);

        var newVariables = variableDeclaration.Variables.Replace(variableDeclarator, newVariableDeclarator);

        var newVariableDeclaration = variableDeclaration
            .WithType(newType)
            .WithVariables(newVariables)
            .WithAdditionalAnnotations(Formatter.Annotation);

        var newRoot = root.ReplaceNode(variableDeclaration, newVariableDeclaration);

        newRoot = AddUsingDirective(newRoot);

        return document.WithSyntaxRoot(newRoot);
    }

    private static async Task<Document> ReplaceArrayWithFrozenSetInProperty(
        Document document,
        PropertyDeclarationSyntax propertyDeclaration,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

        if (root == null || semanticModel == null)
            return document;

        // Get the array element type
        var typeInfo = semanticModel.GetTypeInfo(propertyDeclaration.Type, cancellationToken);
        if (typeInfo.Type is not IArrayTypeSymbol arrayType)
            return document;

        var elementType = arrayType.ElementType;
        var elementTypeName = elementType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

        var newType = CreateFrozenSetType(elementTypeName);

        // Create a new initializer
        var newInitializer = CreateFrozenSetInitializer(propertyDeclaration.Initializer, elementTypeName);
        if (newInitializer == null)
            return document;

        // Create a new property declaration
        var newPropertyDeclaration = propertyDeclaration
            .WithType(newType)
            .WithInitializer(newInitializer)
            .WithAdditionalAnnotations(Formatter.Annotation);

        var newRoot = root.ReplaceNode(propertyDeclaration, newPropertyDeclaration);

        // Add using System.Collections.Frozen
        newRoot = AddUsingDirective(newRoot);

        return document.WithSyntaxRoot(newRoot);
    }

    private static EqualsValueClauseSyntax? CreateFrozenSetInitializer(
        EqualsValueClauseSyntax? originalInitializer,
        string elementTypeName)
    {
        if (originalInitializer?.Value == null)
            return null;

        ExpressionSyntax frozenSetCreation;

        if (originalInitializer.Value is InitializerExpressionSyntax initializerExpression)
        {
            // Case: int[] arr = { 1, 2, 3 };
            // Transform to: FrozenSet<int> arr = new[] { 1, 2, 3 }.ToFrozenSet();
            var implicitArrayCreation = SyntaxFactory.ImplicitArrayCreationExpression(initializerExpression);
            var memberAccess = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                implicitArrayCreation,
                SyntaxFactory.IdentifierName("ToFrozenSet")
            );
            frozenSetCreation = SyntaxFactory.InvocationExpression(memberAccess)
                .WithAdditionalAnnotations(Formatter.Annotation);
        }
        else if (originalInitializer.Value is CollectionExpressionSyntax collectionExpression)
        {
            if (collectionExpression.Elements.Any(e => e is SpreadElementSyntax))
            {
                // Case: int[] arr = [..other, 1, 2, 3];
                // Transform to: FrozenSet<int> arr = ((int[])[..other, 1, 2, 3]).ToFrozenSet();
                var arrayType = SyntaxFactory.ParseTypeName($"{elementTypeName}[]");
                var castExpression = SyntaxFactory.CastExpression(arrayType, collectionExpression);
                var parenthesized = SyntaxFactory.ParenthesizedExpression(castExpression);
                var memberAccess = SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    parenthesized,
                    SyntaxFactory.IdentifierName("ToFrozenSet")
                );
                frozenSetCreation = SyntaxFactory.InvocationExpression(memberAccess)
                    .WithAdditionalAnnotations(Formatter.Annotation);
            }
            else
            {
                // Case: int[] arr = [ 1, 2, 3 ];
                // Transform to: FrozenSet<int> arr = new[] { 1, 2, 3 }.ToFrozenSet();
                var expressions = collectionExpression.Elements
                    .OfType<ExpressionElementSyntax>()
                    .Select(e => e.Expression);
                var initializer = SyntaxFactory.InitializerExpression(
                    SyntaxKind.ArrayInitializerExpression,
                    SyntaxFactory.SeparatedList(expressions)
                );
                var implicitArrayCreation = SyntaxFactory.ImplicitArrayCreationExpression(initializer);
                var memberAccess = SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    implicitArrayCreation,
                    SyntaxFactory.IdentifierName("ToFrozenSet")
                );
                frozenSetCreation = SyntaxFactory.InvocationExpression(memberAccess)
                    .WithAdditionalAnnotations(Formatter.Annotation);
            }
        }
        else
        {
            // Case: int[] arr = new int[] { 1, 2, 3 }; or var arr = new[] { 1, 2, 3 };
            // Transform to: FrozenSet<int> arr = new int[] { 1, 2, 3 }.ToFrozenSet();
            var memberAccess = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                originalInitializer.Value,
                SyntaxFactory.IdentifierName("ToFrozenSet")
            );
            frozenSetCreation = SyntaxFactory.InvocationExpression(memberAccess)
                .WithAdditionalAnnotations(Formatter.Annotation);
        }

        return SyntaxFactory.EqualsValueClause(frozenSetCreation)
            .WithAdditionalAnnotations(Formatter.Annotation);
    }

    private static GenericNameSyntax CreateFrozenSetType(string elementTypeName)
    {
        return SyntaxFactory.GenericName(
            SyntaxFactory.Identifier("FrozenSet"),
            SyntaxFactory.TypeArgumentList(
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.ParseTypeName(elementTypeName)
                )
            )
        );
    }

    private static SyntaxNode AddUsingDirective(SyntaxNode root)
    {
        if (root is not CompilationUnitSyntax compilationUnit)
            return root;

        var hasUsing = compilationUnit.DescendantNodesAndSelf()
            .OfType<UsingDirectiveSyntax>()
            .Any(u => u.Name?.ToString() == "System.Collections.Frozen");

        if (hasUsing)
            return root;

        var systemCollectionsFrozen = SyntaxFactory.UsingDirective(
            SyntaxFactory.QualifiedName(
                SyntaxFactory.QualifiedName(
                    SyntaxFactory.IdentifierName("System"),
                    SyntaxFactory.IdentifierName("Collections")
                ),
                SyntaxFactory.IdentifierName("Frozen")
            )
        );

        return compilationUnit.AddUsings(systemCollectionsFrozen);
    }
}
