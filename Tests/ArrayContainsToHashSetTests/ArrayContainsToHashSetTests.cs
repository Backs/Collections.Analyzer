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
        var code = ResourceReader.ReadFromFile("ArrayContains1.txt");

        return ArrayContainsVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0008").WithSpan(7, 13, 7, 20));
    }
    
    [Test]
    public Task MinArrayLength_ShouldWarn()
    {
        var code = ResourceReader.ReadFromFile("ArrayContains1.txt");

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
        var code = ResourceReader.ReadFromFile("ArrayContains1.txt");

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
        var code = ResourceReader.ReadFromFile("ArrayContains2.txt");

        return ArrayContainsVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0008").WithSpan(5, 28, 5, 34));
    }

    [Test]
    public Task FieldCollectionInitializer_ShouldWarn()
    {
        var code = ResourceReader.ReadFromFile("ArrayContains3.txt");

        return ArrayContainsVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0008").WithSpan(5, 28, 5, 34));
    }

    [Test]
    public Task PropertyImplicitArray_ShouldWarn()
    {
        var code = ResourceReader.ReadFromFile("ArrayContains4.txt");

        return ArrayContainsVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0008").WithSpan(5, 18, 5, 23));
    }

    [Test]
    public Task ArrayWithIndexAccess_ShouldNotWarn()
    {
        var code = ResourceReader.ReadFromFile("ArrayContains5.txt");

        return ArrayContainsVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.EmptyDiagnosticResults);
    }

    [Test]
    public Task ArrayWithModification_ShouldNotWarn()
    {
        var code = ResourceReader.ReadFromFile("ArrayContains6.txt");

        return ArrayContainsVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.EmptyDiagnosticResults);
    }

    [Test]
    public Task ArrayWithoutContains_ShouldNotWarn()
    {
        var code = ResourceReader.ReadFromFile("ArrayContains7.txt");

        return ArrayContainsVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.EmptyDiagnosticResults);
    }

    [Test]
    [TestCase("ArrayContainsBefore1.txt", "ArrayContainsAfter1.txt")]
    [TestCase("ArrayContainsBefore2.txt", "ArrayContainsAfter2.txt")]
    [TestCase("ArrayContainsBefore3.txt", "ArrayContainsAfter3.txt", Ignore = "Different syntax tree in result")]
    [TestCase("ArrayContainsBefore4.txt", "ArrayContainsAfter4.txt")]
    public Task CodeFixesTest(string before, string after)
    {
        var code = ResourceReader.ReadFromFile(before);
        var fixedCode = ResourceReader.ReadFromFile(after);

        return ArrayContainsVerifier.VerifyCodeFixAsync(code, fixedCode);
    }
}