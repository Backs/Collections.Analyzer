using System.Threading.Tasks;
using Collections.Analyzer.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;

namespace Tests;

[TestFixture]
public class AnalyzerConfigHelperTests
{
    private class TestAnalyzerConfigOptions : AnalyzerConfigOptions
    {
        public override bool TryGetValue(string key, out string? value)
        {
            value = null;
            return false;
        }
    }

    private class TestAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
    {
        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => new TestAnalyzerConfigOptions();
        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => new TestAnalyzerConfigOptions();
        public override AnalyzerConfigOptions GlobalOptions => new TestAnalyzerConfigOptions();
    }

    [Test]
    public void GetConfig_ParallelCalls_ShouldNotThrow()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText("class C {}");
        var options = new AnalyzerOptions(default, new TestAnalyzerConfigOptionsProvider());

        Parallel.For(0, 1000, _ =>
        {
            AnalyzerConfigHelper.GetConfig(options, syntaxTree, _ => 1);
        });
    }

    [Test]
    public void GetConfig_DifferentTypes_ShouldNotConflict()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText("class C {}");
        var options = new AnalyzerOptions(default, new TestAnalyzerConfigOptionsProvider());

        var result1 = AnalyzerConfigHelper.GetConfig(options, syntaxTree, _ => 1);
        var result2 = AnalyzerConfigHelper.GetConfig(options, syntaxTree, _ => "string");

        Assert.That(result1, Is.EqualTo(1));
        Assert.That(result2, Is.EqualTo("string"));
    }
}
