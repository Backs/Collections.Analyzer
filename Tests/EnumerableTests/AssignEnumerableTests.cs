using System.Threading.Tasks;
using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0003;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace Tests.EnumerableTests;

[TestFixture]
public class AssignEnumerableTests : CSharpCodeFixTest<
    AssignEnumerableDiagnostic,
    RemoveRedundantMethodCallCodeFix, DefaultVerifier>
{
    [Test]
    public Task AssignEnumerable1Test()
    {
        var code = ResourceReader.ReadFromFile("AssignEnumerable1.txt");

        return AssignEnumerableVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0003").WithSpan(11, 39, 11, 59));
    }
        
    [Test]
    public Task AssignEnumerable2Test()
    {
        var code = ResourceReader.ReadFromFile("AssignEnumerable2.txt");

        return AssignEnumerableVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0003").WithSpan(10, 39, 10, 59));
    }

    [Test]
    [TestCase("AssignEnumerableBefore1.txt", "AssignEnumerableAfter1.txt")]
    [TestCase("AssignEnumerableBefore2.txt", "AssignEnumerableAfter2.txt")]
    public Task CodeFixesTest(string before, string after)
    {
        var code = ResourceReader.ReadFromFile(before);
        var fixedCode = ResourceReader.ReadFromFile(after);

        return AssignEnumerableVerifier.VerifyCodeFixAsync(code, fixedCode);
    }
}