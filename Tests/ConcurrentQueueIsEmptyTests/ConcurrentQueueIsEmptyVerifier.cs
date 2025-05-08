using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0007;
using Microsoft.CodeAnalysis.Testing;

namespace Tests.ConcurrentQueueIsEmptyTests;

public class ConcurrentQueueIsEmptyVerifier : CodeFixVerifier<
    ConcurrentCollectionIsEmptyDiagnostic,
    ReplaceAnyWithIsEmptyCodeFix,
    IsEmptyTests,
    DefaultVerifier>
{
}