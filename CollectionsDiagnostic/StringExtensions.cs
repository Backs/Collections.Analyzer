namespace CollectionsDiagnostic
{
    using Microsoft.CodeAnalysis;

    internal static class StringExtensions
    {
        public static bool IsTypeMethodCalled(IMethodSymbol methodSymbol)
        {
            return methodSymbol.ContainingType.OriginalDefinition.Name == "String"
                   && methodSymbol.Name is "ToCharArray";
        }
    }
}