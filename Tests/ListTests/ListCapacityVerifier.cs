using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0006;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Tests.ListTests
{
    public class ListCapacityVerifier : CodeFixVerifier<
        ListInitializerDiagnostic, 
        SetListCapacityCodeFix,
        ListCapacityTests, 
        NUnitVerifier>
    {
    }
}