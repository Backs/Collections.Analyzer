namespace Tests.ArrayLengthTests
{
    using Collections.Analyzer;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Testing.Verifiers;

    internal sealed class RedundantToArrayLengthVerifier: CodeFixVerifier<RedundantToArrayLengthDiagnostic, ReplaceWithCountCodeFix,
        RedundantToArrayLengthTests,
        NUnitVerifier>
    {
    }
}