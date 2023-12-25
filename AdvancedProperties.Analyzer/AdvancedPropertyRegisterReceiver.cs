using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

namespace AdvancedProperties.Analyzer;

/// <summary>
/// Finds all property which register AdvancedProperty with
/// AdvancedProperty.Register<TOwner, TValue>
/// </summary>

public class AdvancedPropertyRegisterReceiver : ISyntaxReceiver
{
    public List<APropRegisterInfo> CandidateFields { get; } = [];

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is FieldDeclarationSyntax fieldDeclaration &&
            fieldDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword) &&
            fieldDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword) &&
            fieldDeclaration.Modifiers.Any(SyntaxKind.ReadOnlyKeyword))
        {
            // Проверяем, содержит ли выражение инициализации вызов AdvancedProperty.Register
            var variableDeclaration = fieldDeclaration.Declaration;
            foreach (var variable in variableDeclaration.Variables)
            {
                if (variable.Initializer?.Value is InvocationExpressionSyntax invocationExpression &&
                    IsAdvancedPropertyRegisterCall(invocationExpression))
                {
                    CandidateFields.Add(ExtractAPropRegisterInfo(fieldDeclaration));
                    break;
                }
            }
        }
    }

    private bool IsAdvancedPropertyRegisterCall(InvocationExpressionSyntax invocation)
    {
        return invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
               memberAccess.Name.Identifier.Text == "Register" &&
               memberAccess.Expression.ToString() == "AdvancedProperty";
    }

    public APropRegisterInfo ExtractAPropRegisterInfo(FieldDeclarationSyntax fieldDeclarationSyntax)
    {
        var variableDeclaration = fieldDeclarationSyntax.Declaration;
        var variable = variableDeclaration.Variables[0];

        var genericType = (GenericNameSyntax)variableDeclaration.Type;
        var typeArguments = genericType.TypeArgumentList.Arguments;

        var propertyName = variable.Identifier.Text;

        var tOwner = typeArguments[0];
        var tValue = typeArguments[1];

        var file = fieldDeclarationSyntax.SyntaxTree.FilePath;

        return new APropRegisterInfo(propertyName, tOwner, tValue, file);
    }
}
