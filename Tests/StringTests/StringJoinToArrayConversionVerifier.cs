using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0003;
using Microsoft.CodeAnalysis.Testing;

namespace Tests.StringTests;

internal sealed class StringJoinToArrayConversionVerifier : CodeFixVerifier<StringJoinToArrayDiagnostic,
    RemoveRedundantMethodCallCodeFix,
    StringJoinToArrayConversionTests,
    DefaultVerifier>
{
}