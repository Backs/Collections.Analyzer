using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0007;
using Microsoft.CodeAnalysis.Testing;

namespace Tests.ConcurrentDictionaryIsEmptyTests;

public class ConcurrentCollectionIsEmptyVerifier : CodeFixVerifier<
    ConcurrentCollectionIsEmptyDiagnostic,
    ReplaceAnyWithIsEmptyCodeFix,
    IsEmptyTests,
    DefaultVerifier>
{
}