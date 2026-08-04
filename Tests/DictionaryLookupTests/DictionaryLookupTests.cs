using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace Tests.DictionaryLookupTests;

public class DictionaryLookupTests
{
    [Test]
    public Task DictionaryLookup1Test()
    {
        var code = ResourceReader.ReadFromFile("DictionaryLookup1.cs");

        return DictionaryLookupVerifier.VerifyAnalyzerAsync(code);
    }

    [Test]
    public Task DictionaryLookup2Test()
    {
        var code = ResourceReader.ReadFromFile("DictionaryLookup2.cs");

        return DictionaryLookupVerifier.VerifyAnalyzerAsync(code);
    }

    [Test]
    public Task DictionaryLookup3Test()
    {
        var code = ResourceReader.ReadFromFile("DictionaryLookup3.cs");

        return DictionaryLookupVerifier.VerifyAnalyzerAsync(code);
    }

    [Test]
    public Task DictionaryLookup4Test()
    {
        var code = ResourceReader.ReadFromFile("DictionaryLookup4.cs");

        return DictionaryLookupVerifier.VerifyAnalyzerAsync(code);
    }

    [Test]
    public Task DictionaryLookup5Test()
    {
        var code = ResourceReader.ReadFromFile("DictionaryLookup5.cs");

        return DictionaryLookupVerifier.VerifyAnalyzerAsync(code);
    }

    [Test]
    public Task DictionaryLookup6Test()
    {
        var code = ResourceReader.ReadFromFile("DictionaryLookup6.cs");

        return DictionaryLookupVerifier.VerifyAnalyzerAsync(code);
    }

    [Test]
    public Task DictionaryLookup7Test()
    {
        var code = ResourceReader.ReadFromFile("DictionaryLookup7.cs");

        return DictionaryLookupVerifier.VerifyAnalyzerAsync(code);
    }

    [Test]
    public Task DictionaryLookup8Test()
    {
        var code = ResourceReader.ReadFromFile("DictionaryLookup8.cs");

        return DictionaryLookupVerifier.VerifyAnalyzerAsync(code);
    }

    [Test]
    public Task DictionaryLookup9Test()
    {
        var code = ResourceReader.ReadFromFile("DictionaryLookup9.cs");

        return DictionaryLookupVerifier.VerifyAnalyzerAsync(code);
    }
}
