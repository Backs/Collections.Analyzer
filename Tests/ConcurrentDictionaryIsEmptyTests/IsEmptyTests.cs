using System.Threading.Tasks;
using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0007;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace Tests.ConcurrentDictionaryIsEmptyTests;

[TestFixture]
public class IsEmptyTests : CSharpCodeFixTest<
    ConcurrentCollectionIsEmptyDiagnostic, ReplaceAnyWithIsEmptyCodeFix, DefaultVerifier>
{
    [Test]
    public Task LocalVariableIsEmptyTest()
    {
        var code = ResourceReader.ReadFromFile("ConcurrentDictionary1.txt");

        return ConcurrentDictionaryIsEmptyVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0007").WithSpan(15, 20, 15, 28));
    }
    
    [Test]
    public Task ReturnValueIsEmptyTest()
    {
        var code = ResourceReader.ReadFromFile("ConcurrentDictionary3.txt");

        return ConcurrentDictionaryIsEmptyVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0007").WithSpan(10, 20, 10, 33));
    }

    [Test]
    public Task IsEmptyNoWarnTest()
    {
        var code = ResourceReader.ReadFromFile("ConcurrentDictionary2.txt");

        return ConcurrentDictionaryIsEmptyVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.EmptyDiagnosticResults);
    }

    [Test]
    [TestCase("ConcurrentDictionaryBefore1.txt", "ConcurrentDictionaryAfter1.txt")]
    [TestCase("ConcurrentDictionaryBefore2.txt", "ConcurrentDictionaryAfter2.txt")]
    [TestCase("ConcurrentDictionaryBefore3.txt", "ConcurrentDictionaryAfter3.txt")]
    [TestCase("ConcurrentDictionaryBefore4.txt", "ConcurrentDictionaryAfter4.txt")]
    public Task CodeFixesTest(string before, string after)
    {
        var code = ResourceReader.ReadFromFile(before);
        var fixedCode = ResourceReader.ReadFromFile(after);

        return ConcurrentDictionaryIsEmptyVerifier.VerifyCodeFixAsync(code, fixedCode);
    }
}