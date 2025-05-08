using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0007;
using Microsoft.CodeAnalysis.Testing;

namespace Tests.ConcurrentStackIsEmptyTests;

public class ConcurrentStackIsEmptyVerifier : CodeFixVerifier<
    ConcurrentCollectionIsEmptyDiagnostic,
    ReplaceAnyWithIsEmptyCodeFix,
    IsEmptyTests,
    DefaultVerifier>
{
}