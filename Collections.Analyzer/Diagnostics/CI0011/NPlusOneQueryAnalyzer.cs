using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Collections.Analyzer.Diagnostics.CI0011;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NPlusOneQueryAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    internal static readonly DiagnosticDescriptor Rule = new(
        "CI0011",
        Resources.CI0011_Title,
        Resources.CI0011_MessageFormat,
        Categories.Performance,
        DiagnosticSeverity.Warning,
        true,
        Resources.CI0011_Description
    );
    
    private static readonly FrozenSet<string> LinqMethodNames =
    new[] {
        nameof(Enumerable.Select),
        nameof(Enumerable.Where),
        nameof(Enumerable.Any),
        nameof(Enumerable.All),
        nameof(Enumerable.Count),
        nameof(Enumerable.First),
        nameof(Enumerable.FirstOrDefault),
        nameof(Enumerable.Single),
        nameof(Enumerable.SingleOrDefault)
    }.ToFrozenSet();

    private static readonly FrozenSet<string> TestAttributeNames =
        new[]
        {
            "FactAttribute", "TheoryAttribute", "TestAttribute", "TestCaseAttribute", "TestFixtureAttribute",
            "TestMethodAttribute", "TestClassAttribute"
        }.ToFrozenSet();
    
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeLoopNode,
            SyntaxKind.ForEachStatement,
            SyntaxKind.ForStatement,
            SyntaxKind.ForEachVariableStatement);
        context.RegisterSyntaxNodeAction(AnalyzeLinqInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeLinqInvocation(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not InvocationExpressionSyntax invocation) return;

        var config = GetConfig(context);
        
        // Skip analysis if we are inside a test method, and it's not explicitly enabled via .editorconfig
        if (context.SemanticModel.GetEnclosingSymbol(invocation.SpanStart) is IMethodSymbol enclosingMethodSymbol
            && IsTestMethod(enclosingMethodSymbol)
            && !config.AnalyzeTestMethodsEnabled)
        {
            return;
        }

        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess) return;

        // Check if the method is one of the LINQ methods that can cause N+1 query issues when used with lambdas
        var methodName = memberAccess.Name.Identifier.Text;
        if (!LinqMethodNames.Contains(methodName)) return;

        if (invocation.ArgumentList.Arguments.Count == 0) return;
        var argument = invocation.ArgumentList.Arguments[0];

        // We are looking for lambda expressions passed to LINQ methods
        if (argument.Expression is not LambdaExpressionSyntax lambda) return;

        var body = lambda switch
        {
            SimpleLambdaExpressionSyntax simple => simple.Body,
            ParenthesizedLambdaExpressionSyntax parenthesized => parenthesized.Body,
            _ => null
        };

        if (body == null) return;

        var lambdaParameters = GetLambdaParameters(lambda, context.SemanticModel);
        if (lambdaParameters.Count == 0) return;

        var innerInvocations = body.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>();
        foreach (var innerInvocation in innerInvocations)
        {
            AnalyzeInvocation(context, innerInvocation, lambdaParameters);
        }
    }

    private static ISet<ISymbol> GetLambdaParameters(LambdaExpressionSyntax lambda, SemanticModel semanticModel)
    {
        IEnumerable<ParameterSyntax> parameters = lambda switch
        {
            SimpleLambdaExpressionSyntax simple => new[] { simple.Parameter },
            ParenthesizedLambdaExpressionSyntax parenthesized => parenthesized.ParameterList.Parameters,
            _ => Array.Empty<ParameterSyntax>()
        };

        return parameters
            .Select(p => semanticModel.GetDeclaredSymbol(p))
            .OfType<ISymbol>()
            .ToImmutableHashSet(SymbolEqualityComparer.Default);
    }

    private static void AnalyzeLoopNode(SyntaxNodeAnalysisContext context)
    {
        var loopNode = context.Node;

        var config = GetConfig(context);
        // Skip analysis if we are inside a test method, and it's not explicitly enabled via .editorconfig
        if (context.SemanticModel.GetEnclosingSymbol(loopNode.SpanStart) is IMethodSymbol enclosingMethodSymbol 
            && IsTestMethod(enclosingMethodSymbol)
            && !config.AnalyzeTestMethodsEnabled)
        {
            return;
        }

        // Get variables declared by the loop (e.g., 'item' in 'foreach (var item in items)')
        var loopVariables = GetLoopVariableSymbols(loopNode, context.SemanticModel);
        if (loopVariables.Count == 0)
        {
            return;
        }

        // Search for all method calls within the loop body
        var invocations = loopNode.DescendantNodes().OfType<InvocationExpressionSyntax>();
        foreach (var invocation in invocations)
        {
            AnalyzeInvocation(context, invocation, loopVariables);
        }
    }

    private static void AnalyzeInvocation(
        SyntaxNodeAnalysisContext context, 
        InvocationExpressionSyntax invocation, 
        ISet<ISymbol> loopVariables)
    {
        var methodSymbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
        if (methodSymbol == null)
        {
            return;
        }

        // Check if the invocation uses any of the loop variables
        if (!UsesLoopVariable(context, invocation, loopVariables))
        {
            return;
        }

        var config = GetConfig(context);
        // If a data access method is called using a loop variable, report a diagnostic
        if (IsDataAccessMethod(methodSymbol, config))
        {
            var diagnostic = Diagnostic.Create(Rule, invocation.GetLocation(), methodSymbol.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static bool UsesLoopVariable(
        SyntaxNodeAnalysisContext context, 
        InvocationExpressionSyntax invocation, 
        ISet<ISymbol> loopVariables)
    {
        foreach (var argument in invocation.ArgumentList.Arguments)
        {
            var dataFlowAnalysis = context.SemanticModel.AnalyzeDataFlow(argument.Expression)!;
            if (!dataFlowAnalysis.Succeeded)
            {
                continue;
            }

            if (dataFlowAnalysis.ReadInside.Any(loopVariables.Contains))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsDataAccessMethod(IMethodSymbol methodSymbol, NPlusOneQueryConfig config)
    {
        if (methodSymbol.IsStatic || !IsDataAccessType(methodSymbol.ContainingType, config))
        {
            return false;
        }

        var methodName = methodSymbol.Name;

        // Skip bulk/batch methods by name
        if (config.BulkMethodSubstrings.Any(methodName.Contains))
        {
            return false;
        }

        // Skip methods that take a collection as a parameter (excluding string/byte[])
        if (methodSymbol.Parameters.Any(p => IsBulkParameter(p.Type)))
        {
            return false;
        }

        return config.DataAccessMethodPrefixes.Any(methodName.StartsWith);
    }

    private static bool IsBulkParameter(ITypeSymbol type)
    {
        if (type.SpecialType == SpecialType.System_String)
        {
            return false;
        }

        if (type is IArrayTypeSymbol arrayType)
        {
            // Allow byte[] as a non-bulk parameter (often used for blobs, keys, etc.)
            return arrayType.ElementType.SpecialType != SpecialType.System_Byte;
        }

        // Check for generic collections (IEnumerable<T>, List<T>, etc.)
        return type.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T ||
               type.AllInterfaces.Any(i => i.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T);
    }

    private static bool IsDataAccessType(INamedTypeSymbol? type, NPlusOneQueryConfig config)
    {
        if (type == null)
        {
            return false;
        }

        if (IsDataAccessTypeName(type.Name, config.DataAccessTypeSubstrings))
        {
            return true;
        }

        return type.AllInterfaces.Any(i => IsDataAccessTypeName(i.Name, config.DataAccessTypeSubstrings));
    }

    private static bool IsDataAccessTypeName(string typeName, IEnumerable<string> substrings)
    {
        return !string.IsNullOrEmpty(typeName) && substrings.Any(typeName.Contains);
    }

    private static ISet<ISymbol> GetLoopVariableSymbols(SyntaxNode loopNode, SemanticModel semanticModel)
    {
        var symbols = new HashSet<ISymbol>(SymbolEqualityComparer.Default);

        switch (loopNode)
        {
            case ForEachStatementSyntax forEachStatement:
                var symbol = semanticModel.GetDeclaredSymbol(forEachStatement);
                if (symbol != null) symbols.Add(symbol);
                break;

            case ForStatementSyntax forStatement:
                var variableDeclarator = forStatement.Declaration?.Variables.FirstOrDefault();
                if (variableDeclarator != null)
                {
                    var variableSymbol = semanticModel.GetDeclaredSymbol(variableDeclarator);
                    if (variableSymbol != null) symbols.Add(variableSymbol);
                }
                break;

            case ForEachVariableStatementSyntax forEachVariableStatement:
                if (forEachVariableStatement.Variable is DeclarationExpressionSyntax declarationExpression)
                {
                    var designations = declarationExpression.Designation
                        .DescendantNodesAndSelf()
                        .OfType<SingleVariableDesignationSyntax>();

                    foreach (var designation in designations)
                    {
                        var declaredSymbol = semanticModel.GetDeclaredSymbol(designation);
                        if (declaredSymbol != null) symbols.Add(declaredSymbol);
                    }
                }
                break;
        }

        return symbols;
    }

    private static bool IsTestMethod(IMethodSymbol methodSymbol)
    {
        if (HasTestAttribute(methodSymbol.GetAttributes())) return true;

        return methodSymbol.ContainingType != null &&
               HasTestAttribute(methodSymbol.ContainingType.GetAttributes());
    }

    private static bool HasTestAttribute(ImmutableArray<AttributeData> attributes)
    {
        return attributes.Any(attribute =>
        {
            var name = attribute.AttributeClass?.Name;
            return name != null && TestAttributeNames.Contains(name);
        });
    }

    private sealed class NPlusOneQueryConfig
    {
        public bool AnalyzeTestMethodsEnabled { get; }
        public IReadOnlyCollection<string> DataAccessTypeSubstrings { get; }
        public IReadOnlyCollection<string> DataAccessMethodPrefixes { get; }
        public IReadOnlyCollection<string> BulkMethodSubstrings { get; }
        
        private static readonly string[] DataAccessTypeSubstringsDefaults = { "Repository", "Reader", "Writer", "Handler" };
        private static readonly string[] DataAccessMethodPrefixesDefaults = { "Read", "Find", "Get", "TryRead", "TryGet", "TryFind" };
        private static readonly string[] BulkMethodSubstringsDefaults = { "Batch", "Bulk", "Range" };

        private const string AnalyzeTestMethodsOption = "dotnet_diagnostic.CI0011.analyze_test_methods";
        private const string DataAccessTypeSubstringsOption = "dotnet_diagnostic.CI0011.data_access_type_substrings";
        private const string DataAccessMethodPrefixesOption = "dotnet_diagnostic.CI0011.data_access_method_prefixes";
        private const string BulkMethodSubstringsOption = "dotnet_diagnostic.CI0011.bulk_method_substrings";


        private NPlusOneQueryConfig(
            bool analyzeTestMethodsEnabled,
            IReadOnlyCollection<string> dataAccessTypeSubstrings,
            IReadOnlyCollection<string> dataAccessMethodPrefixes,
            IReadOnlyCollection<string> bulkMethodSubstrings)
        {
            AnalyzeTestMethodsEnabled = analyzeTestMethodsEnabled;
            DataAccessTypeSubstrings = dataAccessTypeSubstrings;
            DataAccessMethodPrefixes = dataAccessMethodPrefixes;
            BulkMethodSubstrings = bulkMethodSubstrings;
        }

        public static NPlusOneQueryConfig FromOptions(AnalyzerConfigOptions options)
        {
            var analyzeTestMethodsEnabled = options.TryGetValue(AnalyzeTestMethodsOption, out var testMethodsValue) &&
                                            bool.TryParse(testMethodsValue, out var testMethodsResult) && testMethodsResult;

            var typeSubstrings = AnalyzerConfigHelper.GetList(options, DataAccessTypeSubstringsOption, DataAccessTypeSubstringsDefaults);
            var methodPrefixes = AnalyzerConfigHelper.GetList(options, DataAccessMethodPrefixesOption, DataAccessMethodPrefixesDefaults);
            var bulkSubstrings = AnalyzerConfigHelper.GetList(options, BulkMethodSubstringsOption, BulkMethodSubstringsDefaults);

            return new NPlusOneQueryConfig(analyzeTestMethodsEnabled, typeSubstrings, methodPrefixes, bulkSubstrings);
        }
    }

    private static NPlusOneQueryConfig GetConfig(SyntaxNodeAnalysisContext context)
    {
        return AnalyzerConfigHelper.GetConfig(context.Options, context.Node.SyntaxTree, NPlusOneQueryConfig.FromOptions);
    }
}
