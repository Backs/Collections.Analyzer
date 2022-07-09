namespace Tests.StringTests
{
    using Collections.Analyzer;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Testing.Verifiers;

    internal sealed class StringJoinToArrayConversionVerifier : CodeFixVerifier<StringJoinToArrayDiagnostic, RemoveRedundantMethodCallCodeFix,
        StringJoinToArrayConversionTests,
        NUnitVerifier>
    {
    }
}