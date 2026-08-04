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
        var code = ResourceReader.ReadFromFile("ListInitializer1.cs");

        return ListCapacityVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0006").WithSpan(9, 25, 9, 51));
    }

    [Test]
    public Task ListLessCapacityTest()
    {
        var code = ResourceReader.ReadFromFile("ListInitializer2.cs");

        return ListCapacityVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0006").WithSpan(9, 25, 9, 54));
    }
    
    [Test]
    public Task ListObjectCreationCapacityTest()
    {
        var code = ResourceReader.ReadFromFile("ListInitializer3.cs");

        return ListCapacityVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0006").WithSpan(9, 31, 9, 49));
    }
    
    [Test]
    public Task ListObjectCreationLessCapacityTest()
    {
        var code = ResourceReader.ReadFromFile("ListInitializer4.cs");

        return ListCapacityVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0006").WithSpan(9, 31, 9, 50));
    }

    [Test]
    [TestCase("ListInitializerBefore1.cs", "ListInitializerAfter1.cs")]
    [TestCase("ListInitializerBefore2.cs", "ListInitializerAfter2.cs")]
    [TestCase("ListInitializerBefore3.cs", "ListInitializerAfter3.cs")]
    public Task CodeFixesTest(string before, string after)
    {
        var code = ResourceReader.ReadFromFile(before);
        var fixedCode = ResourceReader.ReadFromFile(after);

        return ListCapacityVerifier.VerifyCodeFixAsync(code, fixedCode);
    }
}