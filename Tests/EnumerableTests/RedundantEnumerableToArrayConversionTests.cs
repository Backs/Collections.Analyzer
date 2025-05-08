using System.Threading.Tasks;
using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0003;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace Tests.EnumerableTests;

public class RedundantEnumerableToArrayConversionTests : CSharpCodeFixTest<
    EnumerableToArrayOnReturnDiagnostic,
    RemoveRedundantMethodCallCodeFix, DefaultVerifier>
{
    [Test]
    public Task EnumerableMethodToArrayTest()
    {
        var code = ResourceReader.ReadFromFile("EnumerableMethodToArray.txt");

        return RedundantEnumerableToArrayConversionVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0003").WithSpan(12, 20, 12, 34));
    }
        
    [Test]
    public Task AsyncEnumerableMethodToArrayTest()
    {
        var code = ResourceReader.ReadFromFile("AsyncEnumerableMethodToArray.txt");

        return RedundantEnumerableToArrayConversionVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0003").WithSpan(15, 20, 15, 34));
    }
        
    [Test]
    public Task AsyncValueTaskEnumerableMethodToArrayTest()
    {
        var code = ResourceReader.ReadFromFile("AsyncEnumerableMethodToArray2.txt");

        return RedundantEnumerableToArrayConversionVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0003").WithSpan(13, 20, 13, 34));
    }
        
    [Test]
    public Task EnumerablePropertyToArrayTest()
    {
        var code = ResourceReader.ReadFromFile("EnumerablePropertyToArray.txt");

        return RedundantEnumerableToArrayConversionVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0003").WithSpan(11, 20, 11, 33));
    }

    [Test]
    public Task GetSetToArrayTest()
    {
        var code = ResourceReader.ReadFromFile("GetSetToArray.txt");

        return RedundantEnumerableToArrayConversionVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0003").WithSpan(10, 20, 10, 38));
    }
}