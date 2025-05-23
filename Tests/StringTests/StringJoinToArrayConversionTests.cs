﻿using System.Threading.Tasks;
using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0003;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace Tests.StringTests;

public class StringJoinToArrayConversionTests : CSharpCodeFixTest<StringJoinToArrayDiagnostic,
    RemoveRedundantMethodCallCodeFix, DefaultVerifier>
{
    [Test]
    public Task StringJoinToArrayTest()
    {
        var code = ResourceReader.ReadFromFile("StringJoin1.txt");

        return StringJoinToArrayConversionVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0003").WithSpan(11, 44, 11, 78));
    }

    [TestCase("StringJoinBefore.txt", "StringJoinAfter.txt")]
    public Task CodeFixesTest(string before, string after)
    {
        var code = ResourceReader.ReadFromFile(before);
        var fixedCode = ResourceReader.ReadFromFile(after);

        return StringJoinToArrayConversionVerifier.VerifyCodeFixAsync(code, fixedCode);
    }
}