using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0003;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Tests.EnumerableTests
{
    public class AssignEnumerableVerifier : CodeFixVerifier<
        AssignEnumerableDiagnostic, 
        RemoveRedundantMethodCallCodeFix,
        AssignEnumerableTests,
        NUnitVerifier>
    {
    }
}