﻿using Collections.Analyzer;
using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0003;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Tests.ArrayTests
{
    public class ConstructorVerifier : CodeFixVerifier<ConstructorDiagnostic, RemoveRedundantMethodCallCodeFix,
        ConstructorTests,
        NUnitVerifier>
    {
    }
}