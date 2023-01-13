using Collections.Analyzer;
using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0003;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Tests.EnumerableTests
{
    internal sealed class RedundantEnumerableToArrayConversionVerifier : CodeFixVerifier<
        EnumerableToArrayOnReturnDiagnostic, RemoveRedundantMethodCallCodeFix,
        RedundantEnumerableToArrayConversionTests,
        NUnitVerifier>
    {
    }
}