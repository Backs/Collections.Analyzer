using System.Threading.Tasks;
using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0007;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace Tests.ConcurrentBagIsEmptyTests;

[TestFixture]
public class IsEmptyTests: CSharpCodeFixTest<
    ConcurrentCollectionIsEmptyDiagnostic, ReplaceAnyWithIsEmptyCodeFix, DefaultVerifier>
{
    [Test]
    public Task LocalVariableIsEmptyTest()
    {
        var code = ResourceReader.ReadFromFile("ConcurrentBag1.cs");

        return ConcurrentBagIsEmptyVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0007").WithSpan(12, 20, 12, 27));
    }
    
    [Test]
    public Task ReturnValueIsEmptyTest()
    {
        var code = ResourceReader.ReadFromFile("ConcurrentBag2.cs");

        return ConcurrentBagIsEmptyVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0007").WithSpan(10, 20, 10, 32));
    }

    [Test]
    public Task IsEmptyNoWarnTest()
    {
        var code = ResourceReader.ReadFromFile("ConcurrentBag3.cs");

        return ConcurrentBagIsEmptyVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.EmptyDiagnosticResults);
    }

    [Test]
    [TestCase("ConcurrentBagBefore1.cs", "ConcurrentBagAfter1.cs")]
    [TestCase("ConcurrentBagBefore2.cs", "ConcurrentBagAfter2.cs")]
    [TestCase("ConcurrentBagBefore3.cs", "ConcurrentBagAfter3.cs")]
    [TestCase("ConcurrentBagBefore4.cs", "ConcurrentBagAfter4.cs")]
    public Task CodeFixesTest(string before, string after)
    {
        var code = ResourceReader.ReadFromFile(before);
        var fixedCode = ResourceReader.ReadFromFile(after);

        return ConcurrentBagIsEmptyVerifier.VerifyCodeFixAsync(code, fixedCode);
    }
}