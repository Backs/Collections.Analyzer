using Collections.Analyzer;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Tests.ArrayTests
{
    internal sealed class RedundantArrayToArrayConversionVerifier : CodeFixVerifier<
        RedundantArrayToArrayConversionDiagnostic, RemoveRedundantMethodCallCodeFix,
        RedundantArrayToArrayConversionTests,
        NUnitVerifier>
    {
    }
}