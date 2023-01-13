using Collections.Analyzer;
using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0001;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Tests.StringTests
{
    internal sealed class RedundantStringToArrayConversionVerifier : CodeFixVerifier<
        StringToArrayDiagnostic, RemoveRedundantMethodCallCodeFix,
        RedundantStringToArrayConversionTests,
        NUnitVerifier>
    {
    }
}