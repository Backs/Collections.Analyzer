namespace CollectionsDiagnostic
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal static class LinqExtensions
    {
        public static bool IsLinqMethodCalled(
            SyntaxNodeAnalysisContext context, 
            InvocationExpressionSyntax invocationExpression,
            IMethodSymbol methodSymbol,
            string typeName,
            IReadOnlyCollection<string> methods)
        {
            if (invocationExpression.Expression is MemberAccessExpressionSyntax
            {
                Expression: IdentifierNameSyntax identifier
            })
            {
                if (context.SemanticModel.GetTypeInfo(identifier).Type?.Name == typeName && methods.Contains(methodSymbol.Name))
                {
                    return true;
                }
            }

            return false;
        }
    }
}