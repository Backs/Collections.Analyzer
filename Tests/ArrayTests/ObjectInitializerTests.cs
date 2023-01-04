using System.Threading.Tasks;
using Collections.Analyzer;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using NUnit.Framework;

namespace Tests.ArrayTests
{
    [TestFixture]
    public class ObjectInitializerTests : CSharpCodeFixTest<ObjectInitializerDiagnostic,
        RemoveRedundantMethodCallCodeFix, NUnitVerifier>
    {
        [Test]
        public Task ObjectInitializerTest()
        {
            var code = ResourceReader.ReadFromFile("ObjectInitializer.txt");

            return ObjectInitializerVerifier
                .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0003").WithSpan(15, 30, 15, 61), DiagnosticResult.CompilerWarning("CI0003").WithSpan(16, 30, 16, 61));
        }
        
        [Test]
        [TestCase("ObjectInitializerBefore.txt", "ObjectInitializerAfter.txt")]
        public Task CodeFixesTest(string before, string after)
        {
            var code = ResourceReader.ReadFromFile(before);
            var fixedCode = ResourceReader.ReadFromFile(after);

            return ObjectInitializerVerifier.VerifyCodeFixAsync(code, fixedCode);
        }
    }
}