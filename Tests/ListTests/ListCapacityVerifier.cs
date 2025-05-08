using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0006;
using Microsoft.CodeAnalysis.Testing;

namespace Tests.ListTests;

public class ListCapacityVerifier : CodeFixVerifier<
    ListInitializerDiagnostic, 
    SetListCapacityCodeFix,
    ListCapacityTests, 
    DefaultVerifier>
{
}