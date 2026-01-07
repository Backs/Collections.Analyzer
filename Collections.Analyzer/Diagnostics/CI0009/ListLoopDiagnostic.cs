using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Collections.Analyzer.Diagnostics.CI0009;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ListLoopDiagnostic : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    internal static readonly DiagnosticDescriptor Rule = new(
        "CI0009",
        Resources.CI0009_Title,
        Resources.CI0009_MessageFormat,
        Categories.Performance,
        DiagnosticSeverity.Warning,
        true
    );

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSyntaxNodeAction(Analyze,
            SyntaxKind.ObjectCreationExpression,
            SyntaxKind.ImplicitObjectCreationExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var objectCreation = (BaseObjectCreationExpressionSyntax)context.Node;

        if (objectCreation.ArgumentList is { Arguments.Count: > 0 })
            return;

        if (!IsGenericList(context, objectCreation))
            return;

        var declarator = objectCreation.FirstAncestorOrSelf<VariableDeclaratorSyntax>();
        if (declarator == null)
            return;

        var localDeclaration = declarator.FirstAncestorOrSelf<LocalDeclarationStatementSyntax>();
        if (localDeclaration?.Parent is not BlockSyntax block)
            return;

        var startIndex = block.Statements.IndexOf(localDeclaration);
        if (startIndex == -1)
            return;

        var listVariableName = declarator.Identifier.Text;

        for (var i = startIndex + 1; i < block.Statements.Count; i++)
        {
            var statement = block.Statements[i];

            if (statement is ForStatementSyntax forStatement)
            {
                var limit = GetForLoopLimit(forStatement);
                if (limit != null && ContainsListAdd(forStatement.Statement, listVariableName))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, objectCreation.GetLocation(), limit.ToString()));
                }

                return;
            }

            if (statement is ForEachStatementSyntax foreachStatement)
            {
                if (IsSafeExpression(foreachStatement.Expression))
                {
                    var propertyName = GetCollectionCountProperty(context, foreachStatement.Expression);
                    if (propertyName != null && ContainsListAdd(foreachStatement.Statement, listVariableName))
                    {
                        var messageArg = $"{foreachStatement.Expression}.{propertyName}";
                        context.ReportDiagnostic(Diagnostic.Create(Rule, objectCreation.GetLocation(), messageArg));
                    }
                }

                return;
            }
            
            if (IsVariableUsed(statement, listVariableName))
            {
                return;
            }
        }
    }

    private static bool IsVariableUsed(StatementSyntax statement, string variableName)
    {
        return statement.DescendantNodes()
            .OfType<IdentifierNameSyntax>()
            .Any(id => id.Identifier.Text == variableName);
    }

    private static bool IsGenericList(SyntaxNodeAnalysisContext context, BaseObjectCreationExpressionSyntax node)
    {
        var typeSymbol = context.SemanticModel.GetTypeInfo(node).Type as INamedTypeSymbol;
        var listSymbol = context.Compilation.GetTypeByMetadataName("System.Collections.Generic.List`1");
        return SymbolEqualityComparer.Default.Equals(typeSymbol?.OriginalDefinition, listSymbol);
    }

    private static ExpressionSyntax? GetForLoopLimit(ForStatementSyntax forStatement)
    {
        if (forStatement.Condition is BinaryExpressionSyntax binary &&
            binary.IsKind(SyntaxKind.LessThanExpression))
        {
            return binary.Right;
        }

        return null;
    }

    private static string? GetCollectionCountProperty(SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
    {
        var type = context.SemanticModel.GetTypeInfo(expression).Type;
        if (type == null) return null;

        if (type.TypeKind == TypeKind.Array || HasProperty(type, nameof(Array.Length)))
            return nameof(Array.Length);
        if (HasProperty(type, nameof(ICollection.Count)))
            return nameof(ICollection.Count);

        return null;
    }

    private static bool HasProperty(ITypeSymbol type, string propertyName)
    {
        return type.GetMembers(propertyName).OfType<IPropertySymbol>().Any();
    }

    private static bool IsSafeExpression(ExpressionSyntax expression)
    {
        return expression is IdentifierNameSyntax or MemberAccessExpressionSyntax;
    }

    private static bool ContainsListAdd(StatementSyntax body, string listName)
    {
        return body.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Any(invocation =>
                invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Name.Identifier.Text == nameof(List<int>.Add) &&
                memberAccess.Expression is IdentifierNameSyntax identifier &&
                identifier.Identifier.Text == listName);
    }
}