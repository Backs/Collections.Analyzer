namespace Tests.StringTests
{
    using System.Threading.Tasks;
    using CollectionsDiagnostic;
    using Microsoft.CodeAnalysis.CSharp.Testing;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Testing.Verifiers;
    using NUnit.Framework;

    public class ForeachStringToArrayTest: CSharpCodeFixTest<RedundantStringToArrayConversionDiagnostic, RemoveRedundantStringConversionCodeFix, NUnitVerifier>
    {
        [Test]
        public async Task IncorrectUsageTest()
        {
            const string code = @"namespace Examples
{
    using System;
    using System.Linq;

    public class ForeachToArray
    {
        public static void RunForeach()
        {
            var str = ""string"";

            foreach (var c in str.ToArray())
            {
                Console.WriteLine(c);
            }
        }
    }
}";

            await ForeachStringToArrayVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0001").WithSpan(12, 31, 12, 44))
            .ConfigureAwait(false);
        }
        
        [Test]
        public async Task CodeFixTest()
        {
            const string code = @"namespace Examples
{
    using System;
    using System.Linq;

    public class ForeachToArray
    {
        public static void RunForeach()
        {
            var str = ""string"";

            foreach (var c in {|CI0001:str.ToArray()|})
            {
                Console.WriteLine(c);
            }
        }
    }
}";

            const string fixedCode = @"namespace Examples
{
    using System;
    using System.Linq;

    public class ForeachToArray
    {
        public static void RunForeach()
        {
            var str = ""string"";

            foreach (var c in str)
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