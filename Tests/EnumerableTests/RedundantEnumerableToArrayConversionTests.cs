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
            var code = @"namespace Examples
{
    using System.Collections.Generic;
    using System.Linq;

    public class ReturnArray
    {
        public static IEnumerable<int> Method1()
        {
            IEnumerable<int> list = new List<int>();

            list = list.Select(o => o);

            return list.ToArray();
        }
    }
}";

            return RedundantEnumerableToArrayConversionVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0003").WithSpan(11, 26, 11, 41));
        }
    }
}