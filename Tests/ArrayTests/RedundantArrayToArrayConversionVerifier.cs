using Collections.Analyzer;
using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0002;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Tests.ArrayTests
{
    internal sealed class RedundantArrayToArrayConversionVerifier : CodeFixVerifier<
        ArrayToArrayDiagnostic, RemoveRedundantMethodCallCodeFix,
        RedundantArrayToArrayConversionTests,
        NUnitVerifier>
    {
    }
}