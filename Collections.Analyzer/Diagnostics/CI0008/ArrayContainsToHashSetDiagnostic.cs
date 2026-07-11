using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Collections.Analyzer.Diagnostics.CI0008;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ArrayContainsToHashSetDiagnostic : DiagnosticAnalyzer
{
    private const string MinArrayLengthOption = "dotnet_diagnostic.CI0008.min_items_count";
    private const int DefaultMinArrayLength = 1;

    private static readonly DiagnosticDescriptor Rule = new(
        "CI0008",
        Resources.CI0008_Title,
        Resources.CI0008_MessageFormat,
        Categories.Performance,
        DiagnosticSeverity.Warning,
        true,
        Resources.CI0008_Description
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSyntaxNodeAction(AnalyzeLocalDeclaration, SyntaxKind.LocalDeclarationStatement);
        context.RegisterSyntaxNodeAction(AnalyzeFieldDeclaration, SyntaxKind.FieldDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzePropertyDeclaration, SyntaxKind.PropertyDeclaration);
    }

    private static void AnalyzeLocalDeclaration(SyntaxNodeAnalysisContext context)
    {
        var localDeclaration = (LocalDeclarationStatementSyntax)context.Node;

        var minLength = GetMinArrayLength(context);

        foreach (var variable in localDeclaration.Declaration.Variables)
        {
            AnalyzeVariable(context, variable, localDeclaration.Declaration.Type, minLength);
        }
    }

    private static void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
    {
        var fieldDeclaration = (FieldDeclarationSyntax)context.Node;

        var minLength = GetMinArrayLength(context);

        foreach (var variable in fieldDeclaration.Declaration.Variables)
        {
            AnalyzeVariable(context, variable, fieldDeclaration.Declaration.Type, minLength);
        }
    }

    private static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
    {
        var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

        var typeInfo = context.SemanticModel.GetTypeInfo(propertyDeclaration.Type);
        if (typeInfo.Type is not IArrayTypeSymbol arrayType)
            return;

        var initializer = propertyDeclaration.Initializer;
        if (initializer == null)
            return;

        var arrayInitializer = FindArrayInitializer(initializer);
        if (arrayInitializer == null)
            return;

        var arraySize = CountArrayElements(arrayInitializer);
        var minLength = GetMinArrayLength(context);

        if (arraySize < minLength)
            return;

        var propertySymbol = context.SemanticModel.GetDeclaredSymbol(propertyDeclaration);
        if (propertySymbol == null)
            return;

        var classDeclaration = propertyDeclaration.FirstAncestorOrSelf<ClassDeclarationSyntax>();
        if (classDeclaration == null)
            return;

        var usageAnalysis = AnalyzeVariableUsage(classDeclaration, propertySymbol, context);

        if (usageAnalysis.ShouldWarn)
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                propertyDeclaration.Identifier.GetLocation(),
                propertySymbol.Name,
                arrayType.ElementType.ToDisplayString()
            );
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static bool IsLinqContains(IMethodSymbol? methodSymbol, Compilation compilation)
    {
        if (methodSymbol == null || methodSymbol.Name != nameof(Enumerable.Contains))
            return false;

        var enumerableType = compilation.GetTypeByMetadataName("System.Linq.Enumerable");
        return SymbolEqualityComparer.Default.Equals(methodSymbol.ContainingType, enumerableType);
    }

    private static InitializerExpressionSyntax? FindArrayInitializer(EqualsValueClauseSyntax? equalsValue)
    {
        if (equalsValue == null)
            return null;

        if (equalsValue.Value is ImplicitArrayCreationExpressionSyntax implicitArray)
            return implicitArray.Initializer;

        if (equalsValue.Value is ArrayCreationExpressionSyntax arrayCreation)
            return arrayCreation.Initializer;

        if (equalsValue.Value is InitializerExpressionSyntax initializerExpression)
            return initializerExpression;

        return null;
    }

    private static void AnalyzeVariable(SyntaxNodeAnalysisContext context,
        VariableDeclaratorSyntax variable,
        TypeSyntax typeSyntax, int minLength)
    {
        var typeInfo = context.SemanticModel.GetTypeInfo(typeSyntax);
        if (typeInfo.Type is not IArrayTypeSymbol arrayType)
            return;

        var initializer = FindArrayInitializer(variable.Initializer);
        if (initializer == null)
            return;

        var arraySize = CountArrayElements(initializer);
        if (arraySize < minLength)
            return;

        var variableSymbol = context.SemanticModel.GetDeclaredSymbol(variable);
        if (variableSymbol == null)
            return;

        var scope = GetAnalysisScope(variable);
        if (scope == null)
            return;

        var usageAnalysis = AnalyzeVariableUsage(scope, variableSymbol, context);

        if (usageAnalysis.ShouldWarn)
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                variable.Identifier.GetLocation(),
                variableSymbol.Name,
                arrayType.ElementType.ToDisplayString()
            );
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static SyntaxNode? GetAnalysisScope(SyntaxNode node)
    {
        // For local variables - method
        var method = node.FirstAncestorOrSelf<BaseMethodDeclarationSyntax>();
        if (method != null)
            return method;

        // For fields - the entire class
        var classDeclaration = node.FirstAncestorOrSelf<ClassDeclarationSyntax>();
        if (classDeclaration != null)
            return classDeclaration;

        return null;
    }

    private static UsageAnalysis AnalyzeVariableUsage(
        SyntaxNode scope,
        ISymbol variableSymbol,
        SyntaxNodeAnalysisContext context)
    {
        var result = new UsageAnalysis();
        var descendantNodes = scope.DescendantNodes();

        foreach (var node in descendantNodes)
        {
            if (node is not IdentifierNameSyntax identifier)
                continue;

            if (identifier.Identifier.ValueText != variableSymbol.Name)
                continue;

            var symbolInfo = context.SemanticModel.GetSymbolInfo(identifier);
            var symbol = symbolInfo.Symbol ?? symbolInfo.CandidateSymbols.FirstOrDefault();
            if (!SymbolEqualityComparer.Default.Equals(symbol, variableSymbol))
                continue;

            if (IsLinqContainsUsage(identifier, context))
            {
                result.HasContainsCall = true;
            }
            else
            {
                result.HasUnsupportedUsage = true;
            }
        }

        return result;
    }

    private static bool IsLinqContainsUsage(IdentifierNameSyntax identifier, SyntaxNodeAnalysisContext context)
    {
        var parent = identifier.Parent;

        // Case: arr.Contains(...)
        if (parent is MemberAccessExpressionSyntax memberAccess &&
            memberAccess.Expression == identifier &&
            memberAccess.Parent is InvocationExpressionSyntax invocation)
        {
            var methodSymbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
            return IsLinqContains(methodSymbol, context.Compilation);
        }

        // Case: Enumerable.Contains(arr, ...)
        if (parent is ArgumentSyntax argument &&
            argument.Parent is ArgumentListSyntax argumentList &&
            argumentList.Parent is InvocationExpressionSyntax staticInvocation)
        {
            var methodSymbol = context.SemanticModel.GetSymbolInfo(staticInvocation).Symbol as IMethodSymbol;
            if (IsLinqContains(methodSymbol, context.Compilation) && methodSymbol!.IsStatic)
            {
                return argumentList.Arguments.IndexOf(argument) == 0;
            }
        }

        return false;
    }

    private static int GetMinArrayLength(SyntaxNodeAnalysisContext context)
    {
        var options = context.Options.AnalyzerConfigOptionsProvider.GetOptions(context.Node.SyntaxTree);

        if (options.TryGetValue(MinArrayLengthOption, out var valueString) &&
            int.TryParse(valueString, out var value) && value >= 0)
        {
            return value;
        }

        return DefaultMinArrayLength;
    }

    private static int CountArrayElements(InitializerExpressionSyntax initializer)
    {
        return initializer.Expressions.Count;
    }

    private class UsageAnalysis
    {
        public bool HasContainsCall { get; set; }
        public bool HasUnsupportedUsage { get; set; }
        public bool ShouldWarn => HasContainsCall && !HasUnsupportedUsage;
    }
}