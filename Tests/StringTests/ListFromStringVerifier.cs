using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0004;
using Microsoft.CodeAnalysis.Testing;

namespace Tests.StringTests;

public class ListFromStringVerifier : CodeFixVerifier<ListFromStringDiagnostic, AddToCharArrayCodeFix,
    ListFromStringTests,
    DefaultVerifier>
{
}