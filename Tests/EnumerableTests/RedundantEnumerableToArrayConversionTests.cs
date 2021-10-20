namespace Tests.EnumerableTests
{
    using System.Threading.Tasks;
    using CollectionsDiagnostic;
    using Microsoft.CodeAnalysis.CSharp.Testing;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Testing.Verifiers;
    using NUnit.Framework;

    public class RedundantEnumerableToArrayConversionTests: CSharpCodeFixTest<RedundantEnumerableToArrayConversionDiagnostic,
        RemoveRedundantMethodCallCodeFix, NUnitVerifier>
    {
        [Test]
        public Task EnumerableToArrayTest()
        {
            var code = ResourceReader.ReadFromFile("EnumerableToArray.txt");

            return RedundantEnumerableToArrayConversionVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0003").WithSpan(12, 20, 12, 34));
        }
    }
}