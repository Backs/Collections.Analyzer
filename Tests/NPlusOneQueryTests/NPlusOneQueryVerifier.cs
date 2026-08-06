using Collections.Analyzer.Diagnostics.CI0011;
using Microsoft.CodeAnalysis.Testing;

namespace Tests.NPlusOneQueryTests;

public class NPlusOneQueryVerifier : AnalyzerVerifier<
    NPlusOneQueryAnalyzer,
    NPlusOneQueryTests,
    DefaultVerifier>
{
}
