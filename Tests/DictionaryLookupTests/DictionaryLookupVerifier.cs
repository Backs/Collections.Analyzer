using System.Threading.Tasks;
using Collections.Analyzer.Diagnostics.CI0010;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace Tests.DictionaryLookupTests;

public static class DictionaryLookupVerifier
{
    public static Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
    {
        var test = new CSharpAnalyzerTest<DictionaryLookupDiagnostic, DefaultVerifier>
        {
            TestCode = source
        };
        test.ExpectedDiagnostics.AddRange(expected);
        return test.RunAsync();
    }
}
