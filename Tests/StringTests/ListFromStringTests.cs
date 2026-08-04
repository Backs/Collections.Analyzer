using System.Threading.Tasks;
using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0004;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace Tests.StringTests;

public class ListFromStringTests : CSharpCodeFixTest<ListFromStringDiagnostic,
    AddToCharArrayCodeFix, DefaultVerifier>
{
    [Test]
    public Task ListConstructor1Test()
    {
        var code = ResourceReader.ReadFromFile("StringListConstructor1.cs");

        return ListFromStringVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0004").WithSpan(12, 41, 12, 54));
    }

    [Test]
    public Task ListConstructor2Test()
    {
        var code = ResourceReader.ReadFromFile("StringListConstructor2.cs");

        return ListFromStringVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0004").WithSpan(12, 41, 12, 44));
    }

    [TestCase("StringListConstructorBefore1.cs", "StringListConstructorAfter.cs")]
    [TestCase("StringListConstructorBefore2.cs", "StringListConstructorAfter.cs")]
    public Task CodeFixesTest(string before, string after)
    {
        var code = ResourceReader.ReadFromFile(before);
        var fixedCode = ResourceReader.ReadFromFile(after);

        return ListFromStringVerifier.VerifyCodeFixAsync(code, fixedCode);
    }
}