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
        var code = ResourceReader.ReadFromFile("ListLoopArray1.cs");

        return ListCapacityVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.EmptyDiagnosticResults);
    }
    
    [Test]
    [TestCase("ListLoopArrayForBefore.cs", "ListLoopArrayForAfter.cs")]
    [TestCase("ListLoopArrayForeachBefore.cs", "ListLoopArrayForeachAfter.cs")]
    [TestCase("ListLoopCollectionForeachBefore.cs", "ListLoopCollectionForeachAfter.cs")]
    [TestCase("ListLoopImplicitNewBefore.cs", "ListLoopImplicitNewAfter.cs")]
    public Task CodeFixesTest(string before, string after)
    {
        var code = ResourceReader.ReadFromFile(before);
        var fixedCode = ResourceReader.ReadFromFile(after);

        return ListLoopCapacityVerifier.VerifyCodeFixAsync(code, fixedCode);
    }
}