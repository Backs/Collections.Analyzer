using Collections.Analyzer;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Tests.ArrayLengthTests
{
    internal sealed class RedundantToArrayLengthVerifier : CodeFixVerifier<RedundantToArrayLengthDiagnostic,
        ReplaceWithCountCodeFix,
        RedundantToArrayLengthTests,
        NUnitVerifier>
    {
    }
}