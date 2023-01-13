using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Collections.Analyzer.Diagnostics
{
    internal static class StringExtensions
    {
        public static bool ToCharArrayCalled(IMethodSymbol methodSymbol)
        {
            return methodSymbol.ContainingType.OriginalDefinition.Name == nameof(String)
                   && methodSymbol.Name == nameof(string.ToCharArray);
        }

        public static bool IsLinqMethodCalledOnString(
            SyntaxNodeAnalysisContext context,
            InvocationExpressionSyntax invocationExpression,
            IMethodSymbol methodSymbol,
            IReadOnlyCollection<string> methods)
        {
            if (invocationExpression.Expression is MemberAccessExpressionSyntax
                {
                    Expression: IdentifierNameSyntax identifier
                }
                && methods.Contains(methodSymbol.Name))
                if (context.SemanticModel.GetTypeInfo(identifier).Type?.Name == nameof(String))
                    return true;

            return false;
        }

        public static bool IsLinqMethodCalledOnMethod(
            SyntaxNodeAnalysisContext context,
            InvocationExpressionSyntax invocationExpression,
            IMethodSymbol methodSymbol,
            IReadOnlyCollection<string> methods)
        {
            if (invocationExpression.Expression is MemberAccessExpressionSyntax
                {
                    Expression: InvocationExpressionSyntax invocationExpressionSyntax
                }
                && methods.Contains(methodSymbol.Name))
                if (context.SemanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol is IMethodSymbol ms
                    && ms.ReturnType.Name == nameof(String))
                    return true;

            return false;
        }
    }
}