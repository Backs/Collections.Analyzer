namespace Tests.StringTests
{
    using CollectionsDiagnostic;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Testing.Verifiers;

    internal class ForeachStringToArrayVerifier : CodeFixVerifier<ForeachStringToArrayDiagnostic, RemoveToArrayCodeFix,
        ForeachStringToArrayTest,
        NUnitVerifier>
    {
    }
}