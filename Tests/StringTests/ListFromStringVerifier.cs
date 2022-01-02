namespace Tests.StringTests
{
    using Collections.Analyzer;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Testing.Verifiers;

    public class ListFromStringVerifier: CodeFixVerifier<ListFromStringDiagnostic, AddToCharArrayCodeFix,
        ListFromStringTests,
        NUnitVerifier>
    {
    }
}