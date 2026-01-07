using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0009;
using Microsoft.CodeAnalysis.Testing;

namespace Tests.ListTests;

public class ListLoopCapacityVerifier: CodeFixVerifier<
    ListLoopDiagnostic, 
    SetListCapacityFromLoopCodeFix,
    ListLoopTests, 
    DefaultVerifier>
{
}