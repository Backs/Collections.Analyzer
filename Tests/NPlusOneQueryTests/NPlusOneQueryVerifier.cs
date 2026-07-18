using Collections.Analyzer.Diagnostics.CI0010;
using Microsoft.CodeAnalysis.Testing;

namespace Tests.NPlusOneQueryTests;

public class NPlusOneQueryVerifier : AnalyzerVerifier<
    NPlusOneQueryAnalyzer,
    NPlusOneQueryTests,
    DefaultVerifier>
{
}
