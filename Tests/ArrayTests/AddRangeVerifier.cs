using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0003;
using Microsoft.CodeAnalysis.Testing;

namespace Tests.ArrayTests;

public class AddRangeVerifier : CodeFixVerifier<AddRangeDiagnostic, RemoveRedundantMethodCallCodeFix,
    AddRangeTests,
    DefaultVerifier>
{
}