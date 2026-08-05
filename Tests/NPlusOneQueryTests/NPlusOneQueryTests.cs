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

    [Test]
    public Task StaticMethod_ShouldNotWarn()
    {
        var code = ResourceReader.ReadFromFile("NPlusOneQuery17.cs");

        return NPlusOneQueryVerifier.VerifyAnalyzerAsync(code);
    }

    [Test]
    public Task BatchMethod_ShouldNotWarn()
    {
        var code = ResourceReader.ReadFromFile("NPlusOneQuery18.cs");

        return NPlusOneQueryVerifier.VerifyAnalyzerAsync(code, DiagnosticResult.EmptyDiagnosticResults);
    }

    [Test]
    public Task ParameterTypes_ShouldWarnCorrectly()
    {
        var code = ResourceReader.ReadFromFile("NPlusOneQuery20.cs");

        return NPlusOneQueryVerifier.VerifyAnalyzerAsync(code);
    }

    [Test]
    public Task TestMethod_ByDefault_ShouldNotWarn()
    {
        var code = ResourceReader.ReadFromFile("NPlusOneQuery14.cs");

        var test = new CSharpAnalyzerTest<NPlusOneQueryAnalyzer, DefaultVerifier>
        {
            TestCode = code,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80
        };

        // Add NUnit assembly reference
        test.TestState.AdditionalReferences.Add(typeof(TestAttribute).Assembly.Location);

        return test.RunAsync();
    }

    [Test]
    public Task TestMethod_WhenEnabledViaEditorConfig_ShouldWarn()
    {
        var code = ResourceReader.ReadFromFile("NPlusOneQuery14.cs");

        var test = new CSharpAnalyzerTest<NPlusOneQueryAnalyzer, DefaultVerifier>
        {
            TestCode = code,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80
        };

        // Add NUnit assembly reference
        test.TestState.AdditionalReferences.Add(typeof(TestAttribute).Assembly.Location);

        test.TestState.AnalyzerConfigFiles.Add(
            ("/.editorconfig", $"is_global = true{System.Environment.NewLine}dotnet_diagnostic.CI0011.analyze_test_methods = true")
        );

        test.ExpectedDiagnostics.Add(
            DiagnosticResult.CompilerWarning("CI0011").WithSpan(13, 24, 13, 48).WithArguments("GetData")
        );

        return test.RunAsync();
    }

    [Test]
    public Task CustomTypeSuffixes_ShouldWarn()
    {
        var code = ResourceReader.ReadFromFile("NPlusOneQuery15.cs");

        var test = new CSharpAnalyzerTest<NPlusOneQueryAnalyzer, DefaultVerifier>
        {
            TestCode = code,
        };

        test.TestState.AnalyzerConfigFiles.Add(
            ("/.editorconfig", $"is_global = true{System.Environment.NewLine}dotnet_diagnostic.CI0011.data_access_type_suffixes = Service")
        );

        return test.RunAsync();
    }

    [Test]
    public Task CustomMethodPrefixes_ShouldWarn()
    {
        var code = ResourceReader.ReadFromFile("NPlusOneQuery16.cs");

        var test = new CSharpAnalyzerTest<NPlusOneQueryAnalyzer, DefaultVerifier>
        {
            TestCode = code,
        };

        test.TestState.AnalyzerConfigFiles.Add(
            ("/.editorconfig", $"is_global = true{System.Environment.NewLine}dotnet_diagnostic.CI0011.data_access_method_prefixes = Fetch")
        );

        return test.RunAsync();
    }

    [Test]
    public Task CustomConfiguration_WithoutSetting_ShouldNotWarn()
    {
        var code1 = ResourceReader.ReadFromFile("NPlusOneQuery15_NoMarkup.cs");
        var code2 = ResourceReader.ReadFromFile("NPlusOneQuery16_NoMarkup.cs");

        return Task.WhenAll(
            NPlusOneQueryVerifier.VerifyAnalyzerAsync(code1, DiagnosticResult.EmptyDiagnosticResults),
            NPlusOneQueryVerifier.VerifyAnalyzerAsync(code2, DiagnosticResult.EmptyDiagnosticResults)
        );
    }

    [Test]
    public Task CustomBulkMethodSubstrings_ShouldWarnCorrectly()
    {
        var code = ResourceReader.ReadFromFile("NPlusOneQuery21.cs");

        var test = new CSharpAnalyzerTest<NPlusOneQueryAnalyzer, DefaultVerifier>
        {
            TestCode = code,
        };

        test.TestState.AnalyzerConfigFiles.Add(
            ("/.editorconfig", $"is_global = true{System.Environment.NewLine}dotnet_diagnostic.CI0011.bulk_method_substrings = CustomSuffix")
        );

        return test.RunAsync();
    }
}
