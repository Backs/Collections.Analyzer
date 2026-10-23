using System.Linq;
using Collections.Analyzer.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace Tests;

[TestFixture]
public class ExpressionExtensionsTests
{
    [Test]
    public void FindCollectionInitializer_CollectionExpression_ShouldReturnCollectionExpression()
    {
        var tree = CSharpSyntaxTree.ParseText("class C { int[] arr = [1, 2, 3]; }");
        var root = tree.GetRoot();
        var variable = root.DescendantNodes().OfType<VariableDeclaratorSyntax>().First();

        var initializer = ExpressionExtensions.FindCollectionInitializer(variable.Initializer);

        Assert.That(initializer, Is.InstanceOf<CollectionExpressionSyntax>());
        Assert.That(ExpressionExtensions.GetCollectionElementCount(initializer), Is.EqualTo(3));
    }

    [Test]
    public void FindCollectionInitializer_ImplicitArray_ShouldReturnInitializer()
    {
        var tree = CSharpSyntaxTree.ParseText("class C { int[] arr = new[] { 1, 2, 3, 4 }; }");
        var root = tree.GetRoot();
        var variable = root.DescendantNodes().OfType<VariableDeclaratorSyntax>().First();

        var initializer = ExpressionExtensions.FindCollectionInitializer(variable.Initializer);

        Assert.That(initializer, Is.InstanceOf<InitializerExpressionSyntax>());
        Assert.That(ExpressionExtensions.GetCollectionElementCount(initializer), Is.EqualTo(4));
    }

    [Test]
    public void FindCollectionInitializer_ExplicitArray_ShouldReturnInitializer()
    {
        var tree = CSharpSyntaxTree.ParseText("class C { int[] arr = new int[] { 1, 2 }; }");
        var root = tree.GetRoot();
        var variable = root.DescendantNodes().OfType<VariableDeclaratorSyntax>().First();

        var initializer = ExpressionExtensions.FindCollectionInitializer(variable.Initializer);

        Assert.That(initializer, Is.InstanceOf<InitializerExpressionSyntax>());
        Assert.That(ExpressionExtensions.GetCollectionElementCount(initializer), Is.EqualTo(2));
    }

    [Test]
    public void FindCollectionInitializer_DirectInitializer_ShouldReturnInitializer()
    {
        var tree = CSharpSyntaxTree.ParseText("class C { int[] arr = { 1, 2, 3, 4, 5 }; }");
        var root = tree.GetRoot();
        var variable = root.DescendantNodes().OfType<VariableDeclaratorSyntax>().First();

        var initializer = ExpressionExtensions.FindCollectionInitializer(variable.Initializer);

        Assert.That(initializer, Is.InstanceOf<InitializerExpressionSyntax>());
        Assert.That(ExpressionExtensions.GetCollectionElementCount(initializer), Is.EqualTo(5));
    }

    [Test]
    public void FindCollectionInitializer_NullOrNonCollection_ShouldReturnNullAndZero()
    {
        var tree = CSharpSyntaxTree.ParseText("class C { int x = 42; }");
        var root = tree.GetRoot();
        var variable = root.DescendantNodes().OfType<VariableDeclaratorSyntax>().First();

        var initializer = ExpressionExtensions.FindCollectionInitializer(variable.Initializer);

        Assert.That(initializer, Is.Null);
        Assert.That(ExpressionExtensions.GetCollectionElementCount(initializer), Is.EqualTo(0));
    }
}
