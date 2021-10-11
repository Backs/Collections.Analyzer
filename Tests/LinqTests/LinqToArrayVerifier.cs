namespace Tests.LinqTests
{
    using CollectionsDiagnostic;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Testing.Verifiers;
    using Tests.StringTests;

    internal class LinqToArrayVerifier : CodeFixVerifier<RedundantStringToArrayConversionDiagnostic, RemoveRedundantStringConversionCodeFix,
        LinqToArrayTest,
        NUnitVerifier>
    {
        
    }
}