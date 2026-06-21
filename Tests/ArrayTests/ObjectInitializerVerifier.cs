using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0003;
using Microsoft.CodeAnalysis.Testing;

namespace Tests.ArrayTests;

public sealed class ObjectInitializerVerifier: CodeFixVerifier<RedundantEnumerableToArrayDiagnostic, RemoveRedundantMethodCallCodeFix,
    ObjectInitializerTests,
    DefaultVerifier>
{
        
}