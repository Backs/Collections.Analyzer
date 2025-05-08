using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0001;
using Microsoft.CodeAnalysis.Testing;

namespace Tests.StringTests;

internal sealed class RedundantStringToArrayConversionVerifier : CodeFixVerifier<
    StringToArrayDiagnostic, RemoveRedundantMethodCallCodeFix,
    RedundantStringToArrayConversionTests,
    DefaultVerifier>
{
}