using Collections.Analyzer;
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