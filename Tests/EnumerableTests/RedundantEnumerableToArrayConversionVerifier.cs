using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0003;
using Microsoft.CodeAnalysis.Testing;

namespace Tests.EnumerableTests;

internal sealed class RedundantEnumerableToArrayConversionVerifier : CodeFixVerifier<
    RedundantEnumerableToArrayDiagnostic, RemoveRedundantMethodCallCodeFix,
    RedundantEnumerableToArrayConversionTests,
    DefaultVerifier>
{
}