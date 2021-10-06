namespace Tests.StringTests
{
    using System.Threading.Tasks;
    using CollectionsDiagnostic;
    using Microsoft.CodeAnalysis.CSharp.Testing;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Testing.Verifiers;
    using NUnit.Framework;

    internal class ForeachStringToCharArrayTest : CSharpCodeFixTest<ForeachStringToArrayDiagnostic, RemoveToArrayCodeFix, NUnitVerifier>
    {
        [Test]
        public async Task IncorrectUsageTest()
        {
            const string code = @"using System;
namespace Examples
{
    public class Foreach
    {
        public static void RunForeach()
        {
            var str = ""string"";

            foreach (var c in str.ToCharArray())
            {
                Console.WriteLine(c);
            }
        }
    }
}";

            await ForeachStringToArrayVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("STRING_TOCHARARRAY0001").WithSpan(10, 31, 10, 48))
            .ConfigureAwait(false);
        }

        [Test]
        public async Task CodeFixTest()
        {
            const string code = @"using System;
namespace Examples
{
    public class Foreach
    {
        public static void RunForeach()
        {
            var value = ""string"";

            foreach (var c in {|STRING_TOCHARARRAY0001:value.ToCharArray()|})
            {
                Console.WriteLine(c);
            }
        }
    }
}";

            const string fixedCode = @"using System;
namespace Examples
{
    public class Foreach
    {
        public static void RunForeach()
        {
            var value = ""string"";

            foreach (var c in value)
            {
                Console.WriteLine(c);
            }
        }
    }
}";
            await ForeachStringToArrayVerifier.VerifyCodeFixAsync(code, fixedCode).ConfigureAwait(false);
        }
    }
}