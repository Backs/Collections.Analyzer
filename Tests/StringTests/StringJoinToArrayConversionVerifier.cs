using Collections.Analyzer;
using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0003;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Tests.StringTests
{
    internal sealed class StringJoinToArrayConversionVerifier : CodeFixVerifier<StringJoinToArrayDiagnostic,
        RemoveRedundantMethodCallCodeFix,
        StringJoinToArrayConversionTests,
        NUnitVerifier>
    {
    }
}