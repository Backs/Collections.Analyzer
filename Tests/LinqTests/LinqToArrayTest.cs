namespace Tests.LinqTests
{
    using System.Threading.Tasks;
    using CollectionsDiagnostic;
    using Microsoft.CodeAnalysis.CSharp.Testing;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Testing.Verifiers;
    using NUnit.Framework;

    public class LinqToArrayTest : CSharpCodeFixTest<LinqToArrayDiagnostic, RemoveToArrayCodeFix, NUnitVerifier>
    {
        [Test]
        public async Task SelectIncorrectUsageTest()
        {
            const string code = @"namespace Examples.Strings
{
    using System.Linq;

    public class LinqMethods
    {
        public void ToArraySelect()
        {
            var str = ""string"";

            var result = str.ToArray().Select(o => o);
        }
    }
}";

            await LinqToArrayVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("STRING_TOARRAY0001").WithSpan(11, 26, 11, 39))
            .ConfigureAwait(false);
        }
    }
}