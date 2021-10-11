namespace Tests.StringTests
{
    using System.Threading.Tasks;
    using CollectionsDiagnostic;
    using Microsoft.CodeAnalysis.CSharp.Testing;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Testing.Verifiers;
    using NUnit.Framework;

    public class RedundantStringToArrayConversionTests: CSharpCodeFixTest<RedundantStringToArrayConversionDiagnostic, RemoveRedundantStringConversionCodeFix, NUnitVerifier>
    {
        [Test]
        public async Task ForeachToArrayTest()
        {
            const string code = @"namespace Examples
{
    using System;
    using System.Linq;

    public class ForeachToArray
    {
        public static void RunForeach()
        {
            var str = ""string"";

            foreach (var c in str.ToArray())
            {
                Console.WriteLine(c);
            }
        }
    }
}";

            await RedundantStringToArrayConversionVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0001").WithSpan(12, 31, 12, 44))
            .ConfigureAwait(false);
        }
        
        [Test]
        public async Task ForeachToArrayCodeFixTest()
        {
            const string code = @"namespace Examples
{
    using System;
    using System.Linq;

    public class ForeachToArray
    {
        public static void RunForeach()
        {
            var str = ""string"";

            foreach (var c in {|CI0001:str.ToArray()|})
            {
                Console.WriteLine(c);
            }
        }
    }
}";

            const string fixedCode = @"namespace Examples
{
    using System;
    using System.Linq;

    public class ForeachToArray
    {
        public static void RunForeach()
        {
            var str = ""string"";

            foreach (var c in str)
            {
                Console.WriteLine(c);
            }
        }
    }
}";
            await RedundantStringToArrayConversionVerifier.VerifyCodeFixAsync(code, fixedCode).ConfigureAwait(false);
        }
        
        [Test]
        public async Task ForeachToCharArrayTest()
        {
            const string code = @"using System;
namespace Examples
{
    public class Foreach
    {
        public static void RunForeach()
        {
            var str = ""string"";

            foreach (var c in str.ToCharArray())
            {
                Console.WriteLine(c);
            }
        }
    }
}";

            await RedundantStringToArrayConversionVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0001").WithSpan(10, 31, 10, 48))
            .ConfigureAwait(false);
        }

        [Test]
        public async Task ForeachToCharArrayCodeFixTest()
        {
            const string code = @"using System;
namespace Examples
{
    public class Foreach
    {
        public static void RunForeach()
        {
            var value = ""string"";

            foreach (var c in {|CI0001:value.ToCharArray()|})
            {
                Console.WriteLine(c);
            }
        }
    }
}";

            const string fixedCode = @"using System;
namespace Examples
{
    public class Foreach
    {
        public static void RunForeach()
        {
            var value = ""string"";

            foreach (var c in value)
            {
                Console.WriteLine(c);
            }
        }
    }
}";
            await RedundantStringToArrayConversionVerifier.VerifyCodeFixAsync(code, fixedCode).ConfigureAwait(false);
        }
        
        [Test]
        public async Task ListConstructorTest()
        {
            const string code = @"namespace Examples.Strings
{
    using System.Collections.Generic;
    using System.Linq;

    public class Constructor
    {
        public static void List()
        {
            var str = ""string"";

            var result = new List<char>(str.ToArray());
        }
    }
}";

            await RedundantStringToArrayConversionVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0001").WithSpan(12, 41, 12, 54))
            .ConfigureAwait(false);
        }
        
        [Test]
        public async Task ListConstructorCodeFixTest()
        {
            const string code = @"namespace Examples.Strings
{
    using System.Collections.Generic;
    using System.Linq;

    public class Constructor
    {
        public static void List()
        {
            var str = ""string"";

            var result = new List<char>({|CI0001:str.ToArray()|});
        }
    }
}";

            const string fixedCode = @"namespace Examples.Strings
{
    using System.Collections.Generic;
    using System.Linq;

    public class Constructor
    {
        public static void List()
        {
            var str = ""string"";

            var result = new List<char>(str);
        }
    }
}";
            await RedundantStringToArrayConversionVerifier.VerifyCodeFixAsync(code, fixedCode).ConfigureAwait(false);
        }
        
        [Test]
        public async Task SelectToArrayTest()
        {
            const string code = @"namespace Examples.Strings
{
    using System.Linq;

    public class LinqMethods
    {
        public void ToArraySelect()
        {
            var str = ""string"";

            var result = str.ToArray().Select(o => o);
        }
    }
}";

            await RedundantStringToArrayConversionVerifier
            .VerifyAnalyzerAsync(code, DiagnosticResult.CompilerWarning("CI0001").WithSpan(11, 26, 11, 39))
            .ConfigureAwait(false);
        }
    }
}