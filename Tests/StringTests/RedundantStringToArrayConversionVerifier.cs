namespace Tests.StringTests
{
    using CollectionsDiagnostic;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Testing.Verifiers;

    internal class RedundantStringToArrayConversionVerifier : CodeFixVerifier<RedundantStringToArrayConversionDiagnostic, RemoveRedundantStringConversionCodeFix,
        RedundantStringToArrayConversionTests,
        NUnitVerifier>
    {
    }
}