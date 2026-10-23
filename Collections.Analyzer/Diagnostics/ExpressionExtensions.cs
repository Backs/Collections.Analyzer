using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Collections.Analyzer.Diagnostics;

public static class ExpressionExtensions
{
    private static readonly IReadOnlyCollection<string> Methods = new [] {nameof(Enumerable.ToArray), nameof(Enumerable.ToList)}.ToFrozenSet();

    public static bool IsRedundantMethod(SyntaxNodeAnalysisContext context,
        ExpressionSyntax invocationExpression)
    {
        return context.SemanticModel.GetSymbolInfo(invocationExpression).Symbol is IMethodSymbol redundantMethod &&
               Methods.Contains(redundantMethod.Name);
    }

    public static ITypeSymbol? GetCallerType(SyntaxNodeAnalysisContext context,
        InvocationExpressionSyntax invocationExpression)
    {
        if (invocationExpression.Expression is not MemberAccessExpressionSyntax memberAccessExpression)
            return null;
        var symbol = context.SemanticModel.GetSymbolInfo(memberAccessExpression.Expression).Symbol;

        return symbol switch
        {
            ILocalSymbol localSymbol => localSymbol.Type,
            IMethodSymbol methodSymbol => methodSymbol.ReturnType,
            IPropertySymbol propertySymbol => propertySymbol.Type,
            IFieldSymbol fieldSymbol => fieldSymbol.Type,
            IParameterSymbol parameterSymbol => parameterSymbol.Type,
            _ => context.SemanticModel.GetTypeInfo(memberAccessExpression.Expression).Type
        };
    }

    public static SyntaxNode? FindCollectionInitializer(EqualsValueClauseSyntax? equalsValue)
    {
        if (equalsValue == null)
            return null;

        if (equalsValue.Value is ImplicitArrayCreationExpressionSyntax implicitArray)
            return implicitArray.Initializer;

        if (equalsValue.Value is ArrayCreationExpressionSyntax arrayCreation)
            return arrayCreation.Initializer;

        if (equalsValue.Value is InitializerExpressionSyntax initializerExpression)
            return initializerExpression;

        if (equalsValue.Value is CollectionExpressionSyntax collectionExpression)
            return collectionExpression;

        return null;
    }

    public static int GetCollectionElementCount(SyntaxNode? initializer)
    {
        return initializer switch
        {
            InitializerExpressionSyntax initExpr => initExpr.Expressions.Count,
            CollectionExpressionSyntax collExpr => collExpr.Elements.Count,
            _ => 0
        };
    }

    public static bool SupportsFrozenSet(this Compilation compilation)
    {
        return compilation.GetTypeByMetadataName("System.Collections.Frozen.FrozenSet`1") != null
               && compilation.GetTypeByMetadataName("System.Collections.Frozen.FrozenSet") != null;
    }
}