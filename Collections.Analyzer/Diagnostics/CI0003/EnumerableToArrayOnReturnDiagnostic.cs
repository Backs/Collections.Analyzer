using System.Collections;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Collections.Analyzer.Diagnostics.CI0003
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class EnumerableToArrayOnReturnDiagnostic : DiagnosticAnalyzer
    {
        internal static readonly DiagnosticDescriptor RedundantEnumerableToArrayRule = new(
            "CI0003",
            Resources.CI0003_Title,
            Resources.CI0003_Title,
            Categories.Performance,
            DiagnosticSeverity.Warning,
            true
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(RedundantEnumerableToArrayRule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax) context.Node;

            if (!ExpressionExtensions.IsRedundantMethod(context, invocationExpression)) return;

            if (invocationExpression.Parent is not ReturnStatementSyntax) return;

            if (invocationExpression.Expression is MemberAccessExpressionSyntax
                {
                    Expression: IdentifierNameSyntax identifier
                }
                && context.SemanticModel.GetTypeInfo(identifier).Type!.AllInterfaces.Any(o =>
                    o.Name == nameof(IEnumerable))
                && GetReturnType(invocationExpression, context)?.Name == nameof(IEnumerable)
               )
            {
                context.ReportDiagnostic(Diagnostic.Create(RedundantEnumerableToArrayRule,
                    invocationExpression.GetLocation(),
                    invocationExpression.ToString()));
            }
            else if (invocationExpression.Expression is MemberAccessExpressionSyntax
                     {
                         Expression: InvocationExpressionSyntax invocationExpressionSyntax
                     }
                     && GetReturnType(invocationExpressionSyntax, context)?.Name == nameof(IEnumerable)
                     && context.SemanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol is IMethodSymbol ms
                     && ms.ReturnType.AllInterfaces.Any(o => o.Name == nameof(IEnumerable)))
            {
                context.ReportDiagnostic(Diagnostic.Create(RedundantEnumerableToArrayRule,
                    invocationExpression.GetLocation(),
                    invocationExpression.ToString()));
            }
        }

        private static ITypeSymbol? GetReturnType(SyntaxNode? node, SyntaxNodeAnalysisContext context)
        {
            while (node != null)
            {
                switch (node)
                {
                    case MethodDeclarationSyntax method:
                        return GetType(context, method.ReturnType);
                    case PropertyDeclarationSyntax property:
                        return GetType(context, property.Type);
                    default:
                        node = node.Parent;
                        break;
                }
            }

            return null;
        }

        private static ITypeSymbol? GetType(SyntaxNodeAnalysisContext context, ExpressionSyntax returnType)
        {
            var typeSymbol = context.SemanticModel.GetTypeInfo(returnType).Type as INamedTypeSymbol;

            if (typeSymbol == null)
                return null;
            
            if (typeSymbol is {IsGenericType: true, Name: nameof(Task)})
            {
                return typeSymbol.TypeArguments.FirstOrDefault();
            }
            
            if (typeSymbol is {IsGenericType: true, Name: nameof(ValueTask)})
            {
                return typeSymbol.TypeArguments.FirstOrDefault();
            }
            
            return typeSymbol;
        }
    }
}