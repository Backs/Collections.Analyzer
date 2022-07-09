namespace Tests.StringTests
{
    using System.Threading.Tasks;
    using Collections.Analyzer;
    using Microsoft.CodeAnalysis.CSharp.Testing;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Testing.Verifiers;
    using NUnit.Framework;

    public class StringJoinToArrayConversionTests : CSharpCodeFixTest<StringJoinToArrayDiagnostic,
        RemoveRedundantMethodCallCodeFix, NUnitVerifier>
    {
        [Test]
        public Task StringJoinToArrayTest()
        {
            var code = ResourceReader.ReadFromFile("StringJoin1.txt");

            return StringJoinToArrayConversionVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0003").WithSpan(11, 44, 11, 78));
        }
        
        [TestCase("StringJoinBefore.txt", "StringJoinAfter.txt")]
        public Task CodeFixesTest(string before, string after)
        {
            var code = ResourceReader.ReadFromFile(before);
            var fixedCode = ResourceReader.ReadFromFile(after);

            return StringJoinToArrayConversionVerifier.VerifyCodeFixAsync(code, fixedCode);
        }
    }
}