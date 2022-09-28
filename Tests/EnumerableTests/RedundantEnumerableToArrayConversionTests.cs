using System.Threading.Tasks;
using Collections.Analyzer;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using NUnit.Framework;

namespace Tests.EnumerableTests
{
    public class RedundantEnumerableToArrayConversionTests : CSharpCodeFixTest<
        RedundantEnumerableToArrayConversionDiagnostic,
        RemoveRedundantMethodCallCodeFix, NUnitVerifier>
    {
        [Test]
        public Task EnumerableToArrayTest()
        {
            var code = ResourceReader.ReadFromFile("EnumerableToArray.txt");

            return RedundantEnumerableToArrayConversionVerifier
                .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0003").WithSpan(12, 20, 12, 34));
        }

        [Test]
        public Task GetSetToArrayTest()
        {
            var code = ResourceReader.ReadFromFile("GetSetToArray.txt");

            return RedundantEnumerableToArrayConversionVerifier
                .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0003").WithSpan(10, 20, 10, 38));
        }
    }
}