using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0007;
using Microsoft.CodeAnalysis.Testing;

namespace Tests.ConcurrentBagIsEmptyTests;

public class ConcurrentBagIsEmptyVerifier : CodeFixVerifier<
    ConcurrentCollectionIsEmptyDiagnostic,
    ReplaceAnyWithIsEmptyCodeFix,
    IsEmptyTests,
    DefaultVerifier>
{
}