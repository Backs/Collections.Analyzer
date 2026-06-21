using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Collections.Analyzer.Diagnostics.CI0003;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RedundantEnumerableToArrayDiagnostic : DiagnosticAnalyzer
{
    internal static readonly DiagnosticDescriptor Rule = new(
        "CI0003",
        Resources.CI0003_Title,
        Resources.CI0003_Title,
        Categories.Performance,
        DiagnosticSeverity.Warning,
        true
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        context.RegisterSyntaxNodeAction(AnalyzeVariableDeclaration, SyntaxKind.VariableDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeObjectCreation, SyntaxKind.ObjectCreationExpression);
        context.RegisterSyntaxNodeAction(AnalyzeObjectInitializer, SyntaxKind.ObjectInitializerExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocationExpression = (InvocationExpressionSyntax) context.Node;

        if (ExpressionExtensions.IsRedundantMethod(context, invocationExpression))
        {
            // Case: list.AddRange(collection.ToArray()) or list.AddRange(collection.ToList())
            if (invocationExpression.Parent is ArgumentSyntax argument &&
                argument.Parent is ArgumentListSyntax argumentList &&
                argumentList.Parent is InvocationExpressionSyntax parentInvocation &&
                parentInvocation.Expression is MemberAccessExpressionSyntax memberAccessExpression &&
                memberAccessExpression.Name.ToString() == nameof(List<object>.AddRange))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, invocationExpression.GetLocation(),
                    invocationExpression.ToString()));
                return;
            }

            // Case: return collection.ToArray() or return collection.ToList() when the return type is IEnumerable
            if (invocationExpression.Parent is ReturnStatementSyntax)
            {
                if (invocationExpression.Expression is MemberAccessExpressionSyntax
                    {
                        Expression: IdentifierNameSyntax identifier
                    }
                    && context.SemanticModel.GetTypeInfo(identifier).Type!.AllInterfaces.Any(o =>
                        o.Name == nameof(IEnumerable))
                    && GetReturnType(invocationExpression, context)?.Name == nameof(IEnumerable)
                   )
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule,
                        invocationExpression.GetLocation(),
                        invocationExpression.ToString()));
                    return;
                }

                if (invocationExpression.Expression is MemberAccessExpressionSyntax
                    {
                        Expression: InvocationExpressionSyntax innerInvocation
                    }
                    && GetReturnType(innerInvocation, context)?.Name == nameof(IEnumerable)
                    && context.SemanticModel.GetSymbolInfo(innerInvocation).Symbol is IMethodSymbol ms
                    && ms.ReturnType.AllInterfaces.Any(o => o.Name == nameof(IEnumerable)))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule,
                        invocationExpression.GetLocation(),
                        invocationExpression.ToString()));
                    return;
                }
            }
        }

        // Case: string.Join(..., collection.ToArray()) or string.Join(..., collection.ToList())
        if (invocationExpression.Expression is MemberAccessExpressionSyntax
            {
                Expression: PredefinedTypeSyntax
            } ma && ma.Name.ToString() == nameof(string.Join) &&
            invocationExpression.ArgumentList.Arguments.ElementAtOrDefault(1)?.Expression is
                InvocationExpressionSyntax argInvocation)
        {
            if (ExpressionExtensions.IsRedundantMethod(context, argInvocation))
                context.ReportDiagnostic(Diagnostic.Create(Rule, argInvocation.GetLocation(),
                    argInvocation.ToString()));
        }
    }

    private static void AnalyzeVariableDeclaration(SyntaxNodeAnalysisContext context)
    {
        var declarationExpression = (VariableDeclarationSyntax) context.Node;

        // Case: IEnumerable x = collection.ToArray() or IEnumerable x = collection.ToList()
        foreach (var variable in declarationExpression.Variables)
        {
            if (context.SemanticModel.GetDeclaredSymbol(variable) is ILocalSymbol localSymbol &&
                localSymbol.Type.Name == nameof(IEnumerable)
                && variable.Initializer?.Value is InvocationExpressionSyntax invocationExpression &&
                ExpressionExtensions.IsRedundantMethod(context, invocationExpression))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, invocationExpression.GetLocation(),
                    invocationExpression.ToString()));
            }
        }
    }

    private static void AnalyzeObjectCreation(SyntaxNodeAnalysisContext context)
    {
        var objectCreationExpression = (ObjectCreationExpressionSyntax) context.Node;

        if (objectCreationExpression.ArgumentList?.Arguments == null) return;

        // Case: new List<int>(collection.ToArray()) or other constructors accepting IEnumerable
        var constructor = context.SemanticModel.GetSymbolInfo(objectCreationExpression).Symbol as IMethodSymbol;

        if (constructor == null) return;

        var parameters = constructor.Parameters;

        for (var i = 0; i < objectCreationExpression.ArgumentList.Arguments.Count; i++)
        {
            var argument = objectCreationExpression.ArgumentList.Arguments[i];
            if (argument.Expression is not InvocationExpressionSyntax invocationExpression) continue;

            if (!ExpressionExtensions.IsRedundantMethod(context, invocationExpression)) continue;

            var callerType = ExpressionExtensions.GetCallerType(context, invocationExpression);
            if (callerType == null)
                continue;

            var parameter = parameters[i];

            if (SymbolEqualityComparer.Default.Equals(callerType, parameter.Type) ||
                callerType.AllInterfaces.Contains(parameter.Type, SymbolEqualityComparer.Default))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, invocationExpression.GetLocation(),
                    invocationExpression.ToString()));
            }
        }
    }

    private static void AnalyzeObjectInitializer(SyntaxNodeAnalysisContext context)
    {
        var initializerExpressionSyntax = (InitializerExpressionSyntax) context.Node;

        // Case: new MyObj { ArrayProperty = collection.ToArray() }
        foreach (var expression in initializerExpressionSyntax.Expressions.OfType<AssignmentExpressionSyntax>())
        {
            if (expression.Left is IdentifierNameSyntax &&
                expression.Right is InvocationExpressionSyntax rightInvocationExpression &&
                IsArrayProperty(context, rightInvocationExpression) &&
                ExpressionExtensions.IsRedundantMethod(context, rightInvocationExpression)
               )
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule,
                    rightInvocationExpression.GetLocation(),
                    rightInvocationExpression.ToString()));
            }
        }
    }

    private static bool IsArrayProperty(SyntaxNodeAnalysisContext context,
        InvocationExpressionSyntax invocationExpression)
    {
        return invocationExpression.Expression is MemberAccessExpressionSyntax
               {
                   Expression: MemberAccessExpressionSyntax memberAccessExpression
               } &&
               context.SemanticModel.GetTypeInfo(memberAccessExpression).Type?.Kind == SymbolKind.ArrayType;
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

        if (typeSymbol is {IsGenericType: true, Name: nameof(Task)}
            or {IsGenericType: true, Name: nameof(ValueTask)})
        {
            return typeSymbol.TypeArguments.FirstOrDefault();
        }

        return typeSymbol;
    }
}