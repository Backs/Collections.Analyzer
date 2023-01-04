using Collections.Analyzer;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Tests.ArrayTests
{
    public sealed class ObjectInitializerVerifier: CodeFixVerifier<ObjectInitializerDiagnostic, RemoveRedundantMethodCallCodeFix,
        ObjectInitializerTests,
        NUnitVerifier>
    {
        
    }
}