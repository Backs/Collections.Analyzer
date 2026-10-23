using System.Threading.Tasks;
using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0008;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace Tests.ArrayContainsToHashSetTests;

[TestFixture]
public class ArrayContainsToFrozenSetTests : CSharpCodeFixTest<
    ArrayContainsToHashSetDiagnostic, ArrayContainsToFrozenSetCodeFix, DefaultVerifier>
{
    [Test]
    [TestCase("ArrayContainsBefore1.cs", "ArrayContainsFrozenSetAfter1.cs")]
    [TestCase("ArrayContainsBefore2.cs", "ArrayContainsFrozenSetAfter2.cs")]
    [TestCase("ArrayContainsBefore3.cs", "ArrayContainsFrozenSetAfter3.cs")]
    [TestCase("ArrayContainsBefore4.cs", "ArrayContainsFrozenSetAfter4.cs")]
    [TestCase("ArrayContainsBefore5.cs", "ArrayContainsFrozenSetAfter5.cs")]
    [TestCase("ArrayContainsBefore6.cs", "ArrayContainsFrozenSetAfter6.cs")]
    [TestCase("ArrayContainsBefore7.cs", "ArrayContainsFrozenSetAfter7.cs")]
    [TestCase("ArrayContainsBefore10.cs", "ArrayContainsFrozenSetAfter10.cs")]
    [TestCase("ArrayContainsBefore11.cs", "ArrayContainsFrozenSetAfter11.cs")]
    public Task CodeFixesTest(string before, string after)
    {
        var code = ResourceReader.ReadFromFile(before);
        var fixedCode = ResourceReader.ReadFromFile(after);

        var test = new CSharpCodeFixTest<ArrayContainsToHashSetDiagnostic, ArrayContainsToFrozenSetCodeFix, DefaultVerifier>
        {
            TestCode = code,
            FixedCode = fixedCode,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80
        };

        test.SolutionTransforms.Add((solution, projectId) =>
        {
            var project = solution.GetProject(projectId)!;
            var parseOptions = ((Microsoft.CodeAnalysis.CSharp.CSharpParseOptions)project.ParseOptions!).WithLanguageVersion(Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp12);
            return solution.WithProjectParseOptions(projectId, parseOptions);
        });

        return test.RunAsync();
    }

    [Test]
    public Task WhenFrozenSetNotSupported_CodeFixIsNotOffered()
    {
        var code = ResourceReader.ReadFromFile("ArrayContainsBefore1.cs");

        var test = new CSharpCodeFixTest<ArrayContainsToHashSetDiagnostic, ArrayContainsToFrozenSetCodeFix, DefaultVerifier>
        {
            TestCode = code,
            FixedCode = code,
            ReferenceAssemblies = ReferenceAssemblies.NetStandard.NetStandard20
        };

        return test.RunAsync();
    }
}
