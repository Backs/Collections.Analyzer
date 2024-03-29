﻿using System.Threading.Tasks;
using Collections.Analyzer;
using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0005;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using NUnit.Framework;

namespace Tests.ArrayLengthTests
{
    public class RedundantToArrayLengthTests : CSharpCodeFixTest<ToArrayLengthDiagnostic,
        ReplaceWithCountCodeFix, NUnitVerifier>
    {
        [Test]
        [TestCase("ArrayCountBefore.txt", "ArrayCountAfter.txt")]
        [TestCase("ListCountBefore.txt", "ListCountAfter.txt")]
        public Task CodeFixesTest(string before, string after)
        {
            var code = ResourceReader.ReadFromFile(before);
            var fixedCode = ResourceReader.ReadFromFile(after);

            return RedundantToArrayLengthVerifier.VerifyCodeFixAsync(code, fixedCode);
        }
    }
}