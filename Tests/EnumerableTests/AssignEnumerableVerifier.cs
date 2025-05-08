using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0003;
using Microsoft.CodeAnalysis.Testing;

namespace Tests.EnumerableTests;

public class AssignEnumerableVerifier : CodeFixVerifier<
    AssignEnumerableDiagnostic, 
    RemoveRedundantMethodCallCodeFix,
    AssignEnumerableTests,
    DefaultVerifier>
{
}