namespace Tests.ArrayTests
{
    using System.Threading.Tasks;
    using CollectionsDiagnostic;
    using Microsoft.CodeAnalysis.CSharp.Testing;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Testing.Verifiers;
    using NUnit.Framework;

    public class RedundantArrayToArrayConversionTests: CSharpCodeFixTest<RedundantArrayToArrayConversionDiagnostic,
        RemoveRedundantMethodCallCodeFix, NUnitVerifier>
    {
        [Test]
        public Task ArrayToArrayTest()
        {
            var code = ResourceReader.ReadFromFile("ArrayToArray.txt");

            return RedundantArrayToArrayConversionVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0002").WithSpan(11, 26, 11, 41));
        }
        
        [Test]
        public Task GetArrayToArrayTest()
        {
            var code = ResourceReader.ReadFromFile("GetArrayToArray.txt");

            return RedundantArrayToArrayConversionVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0002").WithSpan(10, 26, 10, 46));
        }
        
        [Test]
        [TestCase("ArrayToArrayBefore.txt", "ArrayToArrayAfter.txt")]
        [TestCase("GetArrayToArrayBefore.txt", "GetArrayToArrayAfter.txt")]
        public Task CodeFixesTest(string before, string after)
        {
            var code = ResourceReader.ReadFromFile(before);
            var fixedCode = ResourceReader.ReadFromFile(after);

            return RedundantArrayToArrayConversionVerifier.VerifyCodeFixAsync(code, fixedCode);
        }
    }
}