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
using Microsoft.CodeAnalysis.Formatting;

namespace Collections.Analyzer.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ArrayContainsToHashSetCodeFix)), Shared]
public class ArrayContainsToHashSetCodeFix : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create("CI0008");

    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
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
                    title: Resources.CI0008_CodeFixTitle,
                    createChangedDocument: c => ReplaceArrayWithHashSetInVariable(context.Document, variableDeclarator, c),
                    equivalenceKey: nameof(ArrayContainsToHashSetCodeFix)),
                diagnostic);
            return;
        }

        // properties
        var propertyDeclaration = token.Value.Parent?.FirstAncestorOrSelf<PropertyDeclarationSyntax>();
        if (propertyDeclaration != null)
        {
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Resources.CI0008_CodeFixTitle,
                    createChangedDocument: c => ReplaceArrayWithHashSetInProperty(context.Document, propertyDeclaration, c),
                    equivalenceKey: nameof(ArrayContainsToHashSetCodeFix)),
                diagnostic);
        }
    }

    private static async Task<Document> ReplaceArrayWithHashSetInVariable(
        Document document,
        VariableDeclaratorSyntax variableDeclarator,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken);
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken);

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
        
        if (variableDeclaration.Type.IsVar && !isFieldDeclaration)
        {
            // user var
            newType = variableDeclaration.Type;
        }
        else
        {
            newType = CreateHashSetType(elementTypeName);
        }
        
        var newInitializer = CreateHashSetInitializer(variableDeclarator.Initializer, elementTypeName);
        if (newInitializer == null)
            return document;
        
        var newVariableDeclarator = variableDeclarator.WithInitializer(newInitializer);
        
        var newVariableDeclaration = variableDeclaration
            .WithType(newType)
            .WithVariables(
                SyntaxFactory.SingletonSeparatedList(newVariableDeclarator)
            )
            .WithAdditionalAnnotations(Formatter.Annotation);

        var newRoot = root.ReplaceNode(variableDeclaration, newVariableDeclaration);

        newRoot = AddUsingDirective(newRoot);

        return document.WithSyntaxRoot(newRoot);
    }

    private static async Task<Document> ReplaceArrayWithHashSetInProperty(
        Document document,
        PropertyDeclarationSyntax propertyDeclaration,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

        if (root == null || semanticModel == null)
            return document;

        // Получаем тип элементов массива
        var typeInfo = semanticModel.GetTypeInfo(propertyDeclaration.Type, cancellationToken);
        if (typeInfo.Type is not IArrayTypeSymbol arrayType)
            return document;

        var elementType = arrayType.ElementType;
        var elementTypeName = elementType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

        var newType = CreateHashSetType(elementTypeName);

        // Создаём новый инициализатор
        var newInitializer = CreateHashSetInitializer(propertyDeclaration.Initializer, elementTypeName);
        if (newInitializer == null)
            return document;

        // Создаём новое объявление свойства
        var newPropertyDeclaration = propertyDeclaration
            .WithType(newType)
            .WithInitializer(newInitializer)
            .WithAdditionalAnnotations(Formatter.Annotation);

        // Заменяем в дереве
        var newRoot = root.ReplaceNode(propertyDeclaration, newPropertyDeclaration);

        // Добавляем using System.Collections.Generic
        newRoot = AddUsingDirective(newRoot);

        return document.WithSyntaxRoot(newRoot);
    }

    private static EqualsValueClauseSyntax? CreateHashSetInitializer(
        EqualsValueClauseSyntax? originalInitializer,
        string elementTypeName)
    {
        if (originalInitializer?.Value == null)
            return null;
        
        var hashSetType = CreateHashSetType(elementTypeName);

        ExpressionSyntax hashSetCreation;
        
        if (originalInitializer.Value is InitializerExpressionSyntax initializerExpression)
        {
            // Case: int[] arr = { 1, 2, 3 };
            // Transform to: HashSet<int> arr = new HashSet<int> { 1, 2, 3 };
            hashSetCreation = SyntaxFactory.ObjectCreationExpression(hashSetType)
                .WithInitializer(initializerExpression)
                .WithAdditionalAnnotations(Formatter.Annotation);
        }
        else
        {
            // Case: int[] arr = new int[] { 1, 2, 3 }; or var arr = new[] { 1, 2, 3 };
            // Transform to: HashSet<int> arr = new HashSet<int>(new int[] { 1, 2, 3 });
            hashSetCreation = SyntaxFactory.ObjectCreationExpression(hashSetType)
                .WithArgumentList(
                    SyntaxFactory.ArgumentList(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Argument(originalInitializer.Value)
                        )
                    )
                )
                .WithAdditionalAnnotations(Formatter.Annotation);
        }

        return SyntaxFactory.EqualsValueClause(hashSetCreation)
            .WithAdditionalAnnotations(Formatter.Annotation);
    }
    
    private static GenericNameSyntax CreateHashSetType(string elementTypeName)
    {
        return SyntaxFactory.GenericName(
            SyntaxFactory.Identifier("HashSet"),
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

        var hasUsing = compilationUnit.Usings.Any(u =>
            u.Name?.ToString() == "System.Collections.Generic");

        if (hasUsing)
            return root;

        var systemCollectionsGeneric = SyntaxFactory.UsingDirective(
            SyntaxFactory.QualifiedName(
                SyntaxFactory.QualifiedName(
                    SyntaxFactory.IdentifierName("System"),
                    SyntaxFactory.IdentifierName("Collections")
                ),
                SyntaxFactory.IdentifierName("Generic")
            )
        );

        return compilationUnit.AddUsings(systemCollectionsGeneric);
    }
}
