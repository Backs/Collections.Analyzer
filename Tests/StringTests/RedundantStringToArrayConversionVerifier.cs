namespace Tests.StringTests
{
    using CollectionsDiagnostic;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Testing.Verifiers;

    internal sealed class RedundantStringToArrayConversionVerifier : CodeFixVerifier<RedundantStringToArrayConversionDiagnostic, RemoveRedundantMethodCallCodeFix,
        RedundantStringToArrayConversionTests,
        NUnitVerifier>
    {
    }
}