namespace Tests.StringTests
{
    using System.Threading.Tasks;
    using Collections.Analyzer;
    using Microsoft.CodeAnalysis.CSharp.Testing;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Testing.Verifiers;
    using NUnit.Framework;

    public class RedundantStringToArrayConversionTests : CSharpCodeFixTest<RedundantStringToArrayConversionDiagnostic,
        RemoveRedundantMethodCallCodeFix, NUnitVerifier>
    {
        [Test]
        public Task ForeachToArrayTest()
        {
            var code = ResourceReader.ReadFromFile("ForeachStringToArray.txt");

            return RedundantStringToArrayConversionVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0001").WithSpan(12, 31, 12, 44));
        }

        [Test]
        public Task ForeachToCharArrayTest()
        {
            var code = ResourceReader.ReadFromFile("ForeachStringToCharArray.txt");

            return RedundantStringToArrayConversionVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0001").WithSpan(10, 31, 10, 48));
        }

        [Test]
        public Task SelectToArrayTest()
        {
            var code = ResourceReader.ReadFromFile("SelectStringToArray.txt");

            return RedundantStringToArrayConversionVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0001").WithSpan(11, 26, 11, 39));
        }

        [Test]
        public Task GetStringSelectToArrayTest()
        {
            var code = ResourceReader.ReadFromFile("GetStringSelectToArray.txt");

            return RedundantStringToArrayConversionVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0001").WithSpan(10, 26, 10, 47));
        }

        [TestCase("ForeachStringToArrayBefore.txt", "ForeachStringToArrayAfter.txt")]
        [TestCase("ForeachStringToListBefore.txt", "ForeachStringToListAfter.txt")]
        [TestCase("ForeachStringToCharArrayBefore.txt", "ForeachStringToCharArrayAfter.txt")]
        [TestCase("GetStringToCharArraySelectBefore.txt", "GetStringToCharArraySelectAfter.txt")]
        public Task CodeFixesTest(string before, string after)
        {
            var code = ResourceReader.ReadFromFile(before);
            var fixedCode = ResourceReader.ReadFromFile(after);

            return RedundantStringToArrayConversionVerifier.VerifyCodeFixAsync(code, fixedCode);
        }
    }
}