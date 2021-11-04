namespace Tests.StringTests
{
    using CollectionsDiagnostic;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Testing.Verifiers;

    public class ListFromStringVerifier: CodeFixVerifier<ListFromStringDiagnostic, AddToCharArrayCodeFix,
        ListFromStringTests,
        NUnitVerifier>
    {
    }
}