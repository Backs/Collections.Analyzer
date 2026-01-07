using System.Threading.Tasks;
using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0009;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace Tests.ListTests;

[TestFixture]
public class ListLoopTests : CSharpCodeFixTest<ListLoopDiagnostic, SetListCapacityFromLoopCodeFix, DefaultVerifier>
{
    [Test]
    public Task NoWarningTest()
    {
        var code = ResourceReader.ReadFromFile("ListLoopArray1.txt");

        return ListCapacityVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.EmptyDiagnosticResults);
    }
    
    [Test]
    [TestCase("ListLoopArrayForBefore.txt", "ListLoopArrayForAfter.txt")]
    [TestCase("ListLoopArrayForeachBefore.txt", "ListLoopArrayForeachAfter.txt")]
    [TestCase("ListLoopCollectionForeachBefore.txt", "ListLoopCollectionForeachAfter.txt")]
    [TestCase("ListLoopImplicitNewBefore.txt", "ListLoopImplicitNewAfter.txt")]
    public Task CodeFixesTest(string before, string after)
    {
        var code = ResourceReader.ReadFromFile(before);
        var fixedCode = ResourceReader.ReadFromFile(after);

        return ListLoopCapacityVerifier.VerifyCodeFixAsync(code, fixedCode);
    }
}