using Collections.Analyzer;
using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0004;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Tests.StringTests
{
    public class ListFromStringVerifier : CodeFixVerifier<ListFromStringDiagnostic, AddToCharArrayCodeFix,
        ListFromStringTests,
        NUnitVerifier>
    {
    }
}