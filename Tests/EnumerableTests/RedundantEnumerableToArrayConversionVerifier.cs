namespace Tests.EnumerableTests
{
    using Collections.Analyzer;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Testing.Verifiers;

    internal sealed class RedundantEnumerableToArrayConversionVerifier: CodeFixVerifier<RedundantEnumerableToArrayConversionDiagnostic, RemoveRedundantMethodCallCodeFix,
        RedundantEnumerableToArrayConversionTests,
        NUnitVerifier>
    {
    }
}