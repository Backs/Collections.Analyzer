using System.Threading.Tasks;
using Collections.Analyzer.Diagnostics.CI0010;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace Tests.NPlusOneQueryTests;

[TestFixture]
public class NPlusOneQueryTests : CSharpAnalyzerTest<NPlusOneQueryAnalyzer, DefaultVerifier>
{
    [Test]
    public Task ForeachLoop_WithLoopVariable_ShouldWarn()
    {
        var code = ResourceReader.ReadFromFile("NPlusOneQuery1.txt");

        return NPlusOneQueryVerifier.VerifyAnalyzerAsync(code,
            DiagnosticResult.CompilerWarning("CI0010").WithSpan(10, 24, 10, 37).WithArguments("GetData"));
    }

    [Test]
    public Task ForLoop_WithLoopVariable_ShouldWarn()
    {
        var code = ResourceReader.ReadFromFile("NPlusOneQuery2.txt");

        return NPlusOneQueryVerifier.VerifyAnalyzerAsync(code,
            DiagnosticResult.CompilerWarning("CI0010").WithSpan(10, 24, 10, 34).WithArguments("GetData"));
    }

    [Test]
    public Task ForeachLoop_WithoutLoopVariable_ShouldNotWarn()
    {
        var code = ResourceReader.ReadFromFile("NPlusOneQuery3.txt");

        return NPlusOneQueryVerifier.VerifyAnalyzerAsync(code, DiagnosticResult.EmptyDiagnosticResults);
    }

    [Test]
    public Task ForeachLoop_VoidOrTaskMethod_ShouldNotWarn()
    {
        var code = ResourceReader.ReadFromFile("NPlusOneQuery4.txt");

        return NPlusOneQueryVerifier.VerifyAnalyzerAsync(code, DiagnosticResult.EmptyDiagnosticResults);
    }

    [Test]
    public Task ForeachLoop_CollectionMethod_ShouldNotWarn()
    {
        var code = ResourceReader.ReadFromFile("NPlusOneQuery5.txt");

        return NPlusOneQueryVerifier.VerifyAnalyzerAsync(code, DiagnosticResult.EmptyDiagnosticResults);
    }

    [Test]
    public Task ForeachLoop_WithLoopVariableProperty_ShouldWarn()
    {
        var code = ResourceReader.ReadFromFile("NPlusOneQuery6.txt");

        return NPlusOneQueryVerifier.VerifyAnalyzerAsync(code,
            DiagnosticResult.CompilerWarning("CI0010").WithSpan(15, 24, 15, 40).WithArguments("GetData"));
    }
}
