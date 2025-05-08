using System.Threading.Tasks;
using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0002;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace Tests.ArrayTests;

public class RedundantArrayToArrayConversionTests : CSharpCodeFixTest<ArrayToArrayDiagnostic,
    RemoveRedundantMethodCallCodeFix, DefaultVerifier>
{
    [Test]
    public Task ArrayToArrayTest()
    {
        var code = ResourceReader.ReadFromFile("ArrayToArray.txt");

        return RedundantArrayToArrayConversionVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0002").WithSpan(11, 26, 11, 41));
    }

    [Test]
    public Task GetArrayToArrayTest()
    {
        var code = ResourceReader.ReadFromFile("GetArrayToArray.txt");

        return RedundantArrayToArrayConversionVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0002").WithSpan(10, 26, 10, 46));
    }

    [Test]
    [TestCase("ArrayToArrayBefore.txt", "ArrayToArrayAfter.txt")]
    [TestCase("GetArrayToArrayBefore.txt", "GetArrayToArrayAfter.txt")]
    public Task CodeFixesTest(string before, string after)
    {
        var code = ResourceReader.ReadFromFile(before);
        var fixedCode = ResourceReader.ReadFromFile(after);

        return RedundantArrayToArrayConversionVerifier.VerifyCodeFixAsync(code, fixedCode);
    }
}