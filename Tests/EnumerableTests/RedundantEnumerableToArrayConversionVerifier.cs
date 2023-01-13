using Collections.Analyzer;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Tests.EnumerableTests
{
    internal sealed class RedundantEnumerableToArrayConversionVerifier : CodeFixVerifier<
        RedundantEnumerableToArrayOnReturnConversionDiagnostic, RemoveRedundantMethodCallCodeFix,
        RedundantEnumerableToArrayConversionTests,
        NUnitVerifier>
    {
    }
}