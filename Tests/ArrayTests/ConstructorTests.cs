using System.Threading.Tasks;
using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0003;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace Tests.ArrayTests;

public class ConstructorTests : CSharpCodeFixTest<ConstructorDiagnostic,
    RemoveRedundantMethodCallCodeFix, DefaultVerifier>
{
    [Test]
    [TestCase("Constructor1.txt")]
    [TestCase("Constructor4.txt")]
    public Task ConstructorTest(string fileName)
    {
        var code = ResourceReader.ReadFromFile(fileName);

        return ConstructorVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0003").WithSpan(12, 40, 12, 54));
    }

    [Test]
    public Task ManyArgumentsTest()
    {
        var code = ResourceReader.ReadFromFile("Constructor2.txt");

        return ConstructorVerifier
            .VerifyAnalyzerAsync(code,
                DiagnosticResult.CompilerWarning("CI0003").WithSpan(12, 48, 12, 62),
                DiagnosticResult.CompilerWarning("CI0003").WithSpan(12, 71, 12, 90));
    }
        
    [Test]
    public Task NoErrorTest()
    {
        var code = ResourceReader.ReadFromFile("Constructor3.txt");

        return ConstructorVerifier
            .VerifyAnalyzerAsync(code);
    }

    [Test]
    [TestCase("ConstructorBefore.txt", "ConstructorAfter.txt")]
    public Task CodeFixesTest(string before, string after)
    {
        var code = ResourceReader.ReadFromFile(before);
        var fixedCode = ResourceReader.ReadFromFile(after);

        return ConstructorVerifier.VerifyCodeFixAsync(code, fixedCode);
    }
}