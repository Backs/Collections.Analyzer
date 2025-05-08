using System.Threading.Tasks;
using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0006;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace Tests.ListTests;

[TestFixture]
public class ListCapacityTests : CSharpCodeFixTest<
    ListInitializerDiagnostic, SetListCapacityCodeFix, DefaultVerifier>
{
    [Test]
    public Task ListCapacityTest()
    {
        var code = ResourceReader.ReadFromFile("ListInitializer1.txt");

        return ListCapacityVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0006").WithSpan(9, 25, 9, 51));
    }

    [Test]
    public Task ListLessCapacityTest()
    {
        var code = ResourceReader.ReadFromFile("ListInitializer2.txt");

        return ListCapacityVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0006").WithSpan(9, 25, 9, 54));
    }

    [Test]
    [TestCase("ListInitializerBefore1.txt", "ListInitializerAfter1.txt")]
    [TestCase("ListInitializerBefore2.txt", "ListInitializerAfter2.txt")]
    public Task CodeFixesTest(string before, string after)
    {
        var code = ResourceReader.ReadFromFile(before);
        var fixedCode = ResourceReader.ReadFromFile(after);

        return ListCapacityVerifier.VerifyCodeFixAsync(code, fixedCode);
    }
}