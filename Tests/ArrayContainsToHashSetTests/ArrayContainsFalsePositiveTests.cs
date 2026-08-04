using System.Threading.Tasks;
using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0008;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace Tests.ArrayContainsToHashSetTests;

[TestFixture]
public class ArrayContainsFalsePositiveTests : CSharpCodeFixTest<
    ArrayContainsToHashSetDiagnostic, ArrayContainsToHashSetCodeFix, DefaultVerifier>
{
    [Test]
    public Task CustomContainsMethod_ShouldNotWarn()
    {
        var code = ResourceReader.ReadFromFile("ArrayContainsFalsePositive1.cs");

        return ArrayContainsVerifier.VerifyAnalyzerAsync(code, DiagnosticResult.EmptyDiagnosticResults);
    }

    [Test]
    public Task ExtensionContainsMethod_ShouldNotWarn()
    {
        var code = ResourceReader.ReadFromFile("ArrayContainsFalsePositive2.cs");

        return ArrayContainsVerifier.VerifyAnalyzerAsync(code, DiagnosticResult.EmptyDiagnosticResults);
    }

    [Test]
    public Task ArrayLengthUsage_ShouldNotWarn()
    {
        var code = ResourceReader.ReadFromFile("ArrayContainsFalsePositive3.cs");

        return ArrayContainsVerifier.VerifyAnalyzerAsync(code, DiagnosticResult.EmptyDiagnosticResults);
    }

    [Test]
    public Task ArrayPassedAsArgument_ShouldNotWarn()
    {
        var code = ResourceReader.ReadFromFile("ArrayContainsFalsePositive4.cs");

        return ArrayContainsVerifier.VerifyAnalyzerAsync(code, DiagnosticResult.EmptyDiagnosticResults);
    }

    [Test]
    public Task ArrayAliasing_ShouldNotWarn()
    {
        var code = ResourceReader.ReadFromFile("ArrayContainsFalsePositive5.cs");

        return ArrayContainsVerifier.VerifyAnalyzerAsync(code, DiagnosticResult.EmptyDiagnosticResults);
    }

    [Test]
    public Task StaticLinqContains_ShouldWarn()
    {
        var code = ResourceReader.ReadFromFile("ArrayContainsFalsePositive6.cs");

        return ArrayContainsVerifier.VerifyAnalyzerAsync(code,
            DiagnosticResult.CompilerWarning("CI0008").WithSpan(7, 15, 7, 22).WithArguments("numbers", "int"));
    }
}
