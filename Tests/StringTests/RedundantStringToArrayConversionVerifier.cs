using Collections.Analyzer;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Tests.StringTests
{
    internal sealed class RedundantStringToArrayConversionVerifier : CodeFixVerifier<
        RedundantStringToArrayConversionDiagnostic, RemoveRedundantMethodCallCodeFix,
        RedundantStringToArrayConversionTests,
        NUnitVerifier>
    {
    }
}