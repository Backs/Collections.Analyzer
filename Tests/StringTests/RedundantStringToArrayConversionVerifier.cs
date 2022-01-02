namespace Tests.StringTests
{
    using Collections.Analyzer;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Testing.Verifiers;

    internal sealed class RedundantStringToArrayConversionVerifier : CodeFixVerifier<RedundantStringToArrayConversionDiagnostic, RemoveRedundantMethodCallCodeFix,
        RedundantStringToArrayConversionTests,
        NUnitVerifier>
    {
    }
}