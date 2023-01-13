using Collections.Analyzer;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Tests.EnumerableTests
{
    public class AssignEnumerableVerifier : CodeFixVerifier<
        AssignEnumerableDiagnostic, RemoveRedundantMethodCallCodeFix,
        AssignEnumerableTests,
        NUnitVerifier>
    {
    }
}