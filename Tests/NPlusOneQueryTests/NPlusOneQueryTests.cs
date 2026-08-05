using System.Threading.Tasks;
using Collections.Analyzer.Diagnostics.CI0011;
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
        var code = ResourceReader.ReadFromFile("NPlusOneQuery1.cs");

        return NPlusOneQueryVerifier.VerifyAnalyzerAsync(code);
    }

    [Test]
    public Task ForLoop_WithLoopVariable_ShouldWarn()
    {
        var code = ResourceReader.ReadFromFile("NPlusOneQuery2.cs");

        return NPlusOneQueryVerifier.VerifyAnalyzerAsync(code);
    }

    [Test]
    public Task ForeachLoop_WithoutLoopVariable_ShouldNotWarn()
    {
        var code = ResourceReader.ReadFromFile("NPlusOneQuery3.cs");

        return NPlusOneQueryVerifier.VerifyAnalyzerAsync(code, DiagnosticResult.EmptyDiagnosticResults);
    }

    [Test]
    public Task ForeachLoop_VoidOrTaskMethod_ShouldNotWarn()
    {
        var code = ResourceReader.ReadFromFile("NPlusOneQuery4.cs");

        return NPlusOneQueryVerifier.VerifyAnalyzerAsync(code, DiagnosticResult.EmptyDiagnosticResults);
    }

    [Test]
    public Task ForeachLoop_CollectionMethod_ShouldNotWarn()
    {
        var code = ResourceReader.ReadFromFile("NPlusOneQuery5.cs");

        return NPlusOneQueryVerifier.VerifyAnalyzerAsync(code, DiagnosticResult.EmptyDiagnosticResults);
    }

    [Test]
    public Task ForeachLoop_WithLoopVariableProperty_ShouldWarn()
    {
        var code = ResourceReader.ReadFromFile("NPlusOneQuery6.cs");

        return NPlusOneQueryVerifier.VerifyAnalyzerAsync(code);
    }

    [Test]
    public Task ForeachLoop_WithTupleDeconstruction_ShouldWarn()
    {
        var code = ResourceReader.ReadFromFile("NPlusOneQuery7.cs");

        return NPlusOneQueryVerifier.VerifyAnalyzerAsync(code);
    }

    [Test]
    public Task ForeachLoop_WithAsyncMethods_ShouldWarn()
    {
        var code = ResourceReader.ReadFromFile("NPlusOneQuery8.cs");

        return NPlusOneQueryVerifier.VerifyAnalyzerAsync(code);
    }

    [Test]
    public Task ForeachLoop_WithoutLoopVariableUsage_ShouldNotWarn()
    {
        var code = ResourceReader.ReadFromFile("NPlusOneQuery9.cs");

        return NPlusOneQueryVerifier.VerifyAnalyzerAsync(code, DiagnosticResult.EmptyDiagnosticResults);
    }

    [Test]
    public Task NestedLoops_WithLoopVariables_ShouldWarn()
    {
        var code = ResourceReader.ReadFromFile("NPlusOneQuery10.cs");

        return NPlusOneQueryVerifier.VerifyAnalyzerAsync(code);
    }

    [Test]
    public Task ForeachLoop_WithNonRepositoryClass_ShouldNotWarn()
    {
        var code = ResourceReader.ReadFromFile("NPlusOneQuery11.cs");

        return NPlusOneQueryVerifier.VerifyAnalyzerAsync(code, DiagnosticResult.EmptyDiagnosticResults);
    }

    [Test]
    public Task LinqSelect_WithLoopVariable_ShouldWarn()
    {
        var code = ResourceReader.ReadFromFile("NPlusOneQuery12.cs");

        return NPlusOneQueryVerifier.VerifyAnalyzerAsync(code);
    }

    [Test]
    public Task LinqSelect_WithBlockBody_ShouldWarn()
    {
        var code = ResourceReader.ReadFromFile("NPlusOneQuery13.cs");

        return NPlusOneQueryVerifier.VerifyAnalyzerAsync(code);
    }
}
