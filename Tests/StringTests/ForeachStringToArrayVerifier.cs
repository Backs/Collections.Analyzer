namespace Tests.StringTests
{
    using CollectionsDiagnostic;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Testing.Verifiers;

    internal class ForeachStringToArrayVerifier : CodeFixVerifier<RedundantStringToArrayConversionDiagnostic, RemoveRedundantStringConversionCodeFix,
        ForeachStringToArrayTest,
        NUnitVerifier>
    {
    }
}