using System.Threading.Tasks;
using Collections.Analyzer;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using NUnit.Framework;

namespace Tests.ArrayTests
{
    public class ConstructorTests : CSharpCodeFixTest<ConstructorDiagnostic,
        RemoveRedundantMethodCallCodeFix, NUnitVerifier>
    {
        [Test]
        public Task ConstructorTest()
        {
            var code = ResourceReader.ReadFromFile("Constructor1.txt");

            return ConstructorVerifier
                .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0003").WithSpan(12, 40, 12, 54));
        }

        [Test]
        public Task ManyArgumentsTest()
        {
            var code = ResourceReader.ReadFromFile("Constructor2.txt");

            return ConstructorVerifier
                .VerifyAnalyzerAsync(code,
                    DiagnosticResult.CompilerWarning("CI0003").WithSpan(12, 48, 12, 62),
                    DiagnosticResult.CompilerWarning("CI0003").WithSpan(12, 71, 12, 90));
        }
        
        [Test]
        [TestCase("ConstructorBefore.txt", "ConstructorAfter.txt")]
        public Task CodeFixesTest(string before, string after)
        {
            var code = ResourceReader.ReadFromFile(before);
            var fixedCode = ResourceReader.ReadFromFile(after);

            return ConstructorVerifier.VerifyCodeFixAsync(code, fixedCode);
        }
    }
}