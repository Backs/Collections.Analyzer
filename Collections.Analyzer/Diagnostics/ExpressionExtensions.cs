using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Collections.Analyzer.Diagnostics
{
    public static class ExpressionExtensions
    {
        private static readonly IReadOnlyCollection<string> Methods =
            new HashSet<string>(new[] {nameof(Enumerable.ToArray), nameof(Enumerable.ToList)});

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
                _ => null
            };
        }
    }
}