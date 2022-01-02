namespace Tests.StringTests
{
    using System.Threading.Tasks;
    using Collections.Analyzer;
    using Microsoft.CodeAnalysis.CSharp.Testing;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Testing.Verifiers;
    using NUnit.Framework;

    public class ListFromStringTests: CSharpCodeFixTest<ListFromStringDiagnostic,
        AddToCharArrayCodeFix, NUnitVerifier>
    {
        [Test]
        public Task ListConstructor1Test()
        {
            var code = ResourceReader.ReadFromFile("StringListConstructor1.txt");

            return ListFromStringVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0004").WithSpan(12, 41, 12, 54));
        }
        
        [Test]
        public Task ListConstructor2Test()
        {
            var code = ResourceReader.ReadFromFile("StringListConstructor2.txt");

            return ListFromStringVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0004").WithSpan(12, 41, 12, 44));
        }
        
        [TestCase("StringListConstructorBefore1.txt", "StringListConstructorAfter.txt")]
        [TestCase("StringListConstructorBefore2.txt", "StringListConstructorAfter.txt")]
        public Task CodeFixesTest(string before, string after)
        {
            var code = ResourceReader.ReadFromFile(before);
            var fixedCode = ResourceReader.ReadFromFile(after);

            return ListFromStringVerifier.VerifyCodeFixAsync(code, fixedCode);
        }
    }
}