using System.Threading.Tasks;
using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0007;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace Tests.ConcurrentStackIsEmptyTests;

[TestFixture]
public class IsEmptyTests: CSharpCodeFixTest<
    ConcurrentCollectionIsEmptyDiagnostic, ReplaceAnyWithIsEmptyCodeFix, DefaultVerifier>
{
    [Test]
    public Task LocalVariableIsEmptyTest()
    {
        var code = ResourceReader.ReadFromFile("ConcurrentStack1.cs");

        return ConcurrentStackIsEmptyVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0007").WithSpan(12, 20, 12, 29));
    }
    
    [Test]
    public Task ReturnValueIsEmptyTest()
    {
        var code = ResourceReader.ReadFromFile("ConcurrentStack2.cs");

        return ConcurrentStackIsEmptyVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0007").WithSpan(10, 20, 10, 34));
    }

    [Test]
    public Task IsEmptyNoWarnTest()
    {
        var code = ResourceReader.ReadFromFile("ConcurrentStack3.cs");

        return ConcurrentStackIsEmptyVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.EmptyDiagnosticResults);
    }

    [Test]
    [TestCase("ConcurrentStackBefore1.cs", "ConcurrentStackAfter1.cs")]
    [TestCase("ConcurrentStackBefore2.cs", "ConcurrentStackAfter2.cs")]
    [TestCase("ConcurrentStackBefore3.cs", "ConcurrentStackAfter3.cs")]
    [TestCase("ConcurrentStackBefore4.cs", "ConcurrentStackAfter4.cs")]
    public Task CodeFixesTest(string before, string after)
    {
        var code = ResourceReader.ReadFromFile(before);
        var fixedCode = ResourceReader.ReadFromFile(after);

        return ConcurrentStackIsEmptyVerifier.VerifyCodeFixAsync(code, fixedCode);
    }
}