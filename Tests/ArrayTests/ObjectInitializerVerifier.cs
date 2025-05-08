using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0003;
using Microsoft.CodeAnalysis.Testing;

namespace Tests.ArrayTests;

public sealed class ObjectInitializerVerifier: CodeFixVerifier<ObjectInitializerDiagnostic, RemoveRedundantMethodCallCodeFix,
    ObjectInitializerTests,
    DefaultVerifier>
{
        
}