using System.Threading.Tasks;
using Collections.Analyzer;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using NUnit.Framework;

namespace Tests.ArrayTests
{
    public class AddRangeTests: CSharpCodeFixTest<AddRangeDiagnostic,
        RemoveRedundantMethodCallCodeFix, NUnitVerifier>
    {
        [Test]
        public Task AddRange1Test()
        {
            var code = ResourceReader.ReadFromFile("AddRange1.txt");

            return AddRangeVerifier
                .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0003").WithSpan(13, 27, 13, 42));
        }
        
        [Test]
        public Task AddRange2Test()
        {
            var code = ResourceReader.ReadFromFile("AddRange2.txt");

            return AddRangeVerifier
                .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0003").WithSpan(12, 27, 12, 47));
        }
        
        [Test]
        [TestCase("AddRangeBefore.txt", "AddRangeAfter.txt")]
        public Task CodeFixesTest(string before, string after)
        {
            var code = ResourceReader.ReadFromFile(before);
            var fixedCode = ResourceReader.ReadFromFile(after);

            return AddRangeVerifier.VerifyCodeFixAsync(code, fixedCode);
        }
    }
}