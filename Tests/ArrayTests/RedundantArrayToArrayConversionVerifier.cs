namespace Tests.ArrayTests
{
    using CollectionsDiagnostic;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Testing.Verifiers;

    internal sealed class RedundantArrayToArrayConversionVerifier: CodeFixVerifier<RedundantArrayToArrayConversionDiagnostic, RemoveRedundantMethodCallCodeFix,
        RedundantArrayToArrayConversionTests,
        NUnitVerifier>
    {
    }
}