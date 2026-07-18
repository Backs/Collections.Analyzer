using System;
using System.Threading.Tasks;
using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0008;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace Tests.ArrayContainsToHashSetTests;

[TestFixture]
public class ArrayContainsToHashSetTests : CSharpCodeFixTest<
    ArrayContainsToHashSetDiagnostic, ArrayContainsToHashSetCodeFix, DefaultVerifier>
{
    [Test]
    public Task LocalVariableImplicitArray_ShouldWarn()
    {
        var code = ResourceReader.ReadFromFile("ArrayContains1.cs");

        return ArrayContainsVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0008").WithSpan(7, 13, 7, 20));
    }
    
    [Test]
    public Task MinArrayLength_ShouldWarn()
    {
        var code = ResourceReader.ReadFromFile("ArrayContains1.cs");

        var test = new CSharpCodeFixTest<ArrayContainsToHashSetDiagnostic, ArrayContainsToHashSetCodeFix, DefaultVerifier>
        {
            TestCode = code,
        };
        
        test.TestState.AnalyzerConfigFiles.Add(
            ("/.editorconfig", $"is_global = true{Environment.NewLine}dotnet_diagnostic.CI0008.min_items_count = 5")
        );

        test.ExpectedDiagnostics.Add(
            DiagnosticResult.CompilerWarning("CI0008").WithSpan(7, 13, 7, 20)
        );
        
        return test.RunAsync();
    }

    [Test]
    public Task MinArrayLength_ShouldNotWarn()
    {
        var code = ResourceReader.ReadFromFile("ArrayContains1.cs");

        var test = new CSharpCodeFixTest<ArrayContainsToHashSetDiagnostic, ArrayContainsToHashSetCodeFix, DefaultVerifier>
        {
            TestCode = code,
        };
        
        test.TestState.AnalyzerConfigFiles.Add(
            ("/.editorconfig", $"is_global = true{Environment.NewLine}dotnet_diagnostic.CI0008.min_items_count = 10")
        );

        return test.RunAsync();
    }

    [Test]
    public Task FieldExplicitArray_ShouldWarn()
    {
        var code = ResourceReader.ReadFromFile("ArrayContains2.cs");

        return ArrayContainsVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0008").WithSpan(5, 28, 5, 34));
    }

    [Test]
    public Task FieldCollectionInitializer_ShouldWarn()
    {
        var code = ResourceReader.ReadFromFile("ArrayContains3.cs");

        return ArrayContainsVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0008").WithSpan(5, 28, 5, 34));
    }

    [Test]
    public Task PropertyImplicitArray_ShouldWarn()
    {
        var code = ResourceReader.ReadFromFile("ArrayContains4.cs");

        return ArrayContainsVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0008").WithSpan(5, 18, 5, 23));
    }

    [Test]
    public Task ArrayWithIndexAccess_ShouldNotWarn()
    {
        var code = ResourceReader.ReadFromFile("ArrayContains5.cs");

        return ArrayContainsVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.EmptyDiagnosticResults);
    }

    [Test]
    public Task ArrayWithModification_ShouldNotWarn()
    {
        var code = ResourceReader.ReadFromFile("ArrayContains6.cs");

        return ArrayContainsVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.EmptyDiagnosticResults);
    }

    [Test]
    public Task ArrayWithoutContains_ShouldNotWarn()
    {
        var code = ResourceReader.ReadFromFile("ArrayContains7.cs");

        return ArrayContainsVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.EmptyDiagnosticResults);
    }

    [Test]
    [TestCase("ArrayContainsBefore1.cs", "ArrayContainsAfter1.cs")]
    [TestCase("ArrayContainsBefore2.cs", "ArrayContainsAfter2.cs")]
    [TestCase("ArrayContainsBefore3.cs", "ArrayContainsAfter3.cs", Ignore = "Different syntax tree in result")]
    [TestCase("ArrayContainsBefore4.cs", "ArrayContainsAfter4.cs")]
    public Task CodeFixesTest(string before, string after)
    {
        var code = ResourceReader.ReadFromFile(before);
        var fixedCode = ResourceReader.ReadFromFile(after);

        return ArrayContainsVerifier.VerifyCodeFixAsync(code, fixedCode);
    }
}