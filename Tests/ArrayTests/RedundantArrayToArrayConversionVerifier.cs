namespace Tests.ArrayTests
{
    using Collections.Analyzer;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Testing.Verifiers;

    internal sealed class RedundantArrayToArrayConversionVerifier: CodeFixVerifier<RedundantArrayToArrayConversionDiagnostic, RemoveRedundantMethodCallCodeFix,
        RedundantArrayToArrayConversionTests,
        NUnitVerifier>
    {
    }
}