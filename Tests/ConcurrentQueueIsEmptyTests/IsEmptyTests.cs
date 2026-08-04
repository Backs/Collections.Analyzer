using System.Threading.Tasks;
using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0007;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace Tests.ConcurrentQueueIsEmptyTests;

[TestFixture]
public class IsEmptyTests: CSharpCodeFixTest<
    ConcurrentCollectionIsEmptyDiagnostic, ReplaceAnyWithIsEmptyCodeFix, DefaultVerifier>
{
    [Test]
    public Task LocalVariableIsEmptyTest()
    {
        var code = ResourceReader.ReadFromFile("ConcurrentQueue1.cs");

        return ConcurrentQueueIsEmptyVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0007").WithSpan(12, 20, 12, 29));
    }
    
    [Test]
    public Task ReturnValueIsEmptyTest()
    {
        var code = ResourceReader.ReadFromFile("ConcurrentQueue2.cs");

        return ConcurrentQueueIsEmptyVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0007").WithSpan(10, 20, 10, 34));
    }

    [Test]
    public Task IsEmptyNoWarnTest()
    {
        var code = ResourceReader.ReadFromFile("ConcurrentQueue3.cs");

        return ConcurrentQueueIsEmptyVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.EmptyDiagnosticResults);
    }

    [Test]
    [TestCase("ConcurrentQueueBefore1.cs", "ConcurrentQueueAfter1.cs")]
    [TestCase("ConcurrentQueueBefore2.cs", "ConcurrentQueueAfter2.cs")]
    [TestCase("ConcurrentQueueBefore3.cs", "ConcurrentQueueAfter3.cs")]
    [TestCase("ConcurrentQueueBefore4.cs", "ConcurrentQueueAfter4.cs")]
    public Task CodeFixesTest(string before, string after)
    {
        var code = ResourceReader.ReadFromFile(before);
        var fixedCode = ResourceReader.ReadFromFile(after);

        return ConcurrentQueueIsEmptyVerifier.VerifyCodeFixAsync(code, fixedCode);
    }
}