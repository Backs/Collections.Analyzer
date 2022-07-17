using Collections.Analyzer;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Tests.ArrayTests
{
    public class AddRangeVerifier: CodeFixVerifier<AddRangeDiagnostic, RemoveRedundantMethodCallCodeFix,
        AddRangeTests,
        NUnitVerifier>
    {
        
    }
}