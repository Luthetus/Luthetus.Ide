using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.CompilerServices.Lang.CSharp.ParserCase.Internals;

public static class ParseVariables
{
    public static void HandleVariableReference(
        IdentifierToken consumedIdentifierToken,
        CSharpParserModel model)
    {
        var variableReferenceNode = model.Binder.ConstructAndBindVariableReferenceNode(
            consumedIdentifierToken,
            model);

        model.SyntaxStack.Push(variableReferenceNode);
    }

    public static void HandleVariableDeclaration(
        TypeClauseNode consumedTypeClauseNode,
        IdentifierToken consumedIdentifierToken,
        VariableKind variableKind,
        CSharpParserModel model)
    {
		IVariableDeclarationNode variableDeclarationNode;

		if (variableKind == VariableKind.Local || variableKind == VariableKind.Closure)
		{
			variableDeclarationNode = new VariableDeclarationNode(
	            consumedTypeClauseNode,
	            consumedIdentifierToken,
	            variableKind,
	            false);
		}
		else if (variableKind == VariableKind.Field)
		{
			variableDeclarationNode = new FieldDefinitionNode(
	            consumedTypeClauseNode,
	            consumedIdentifierToken,
	            variableKind,
	            false);
		}
		else if (variableKind == VariableKind.Property)
		{
			variableDeclarationNode = new PropertyDefinitionNode(
	            consumedTypeClauseNode,
	            consumedIdentifierToken,
	            variableKind,
	            false);
		}
		else
		{
			throw new NotImplementedException($"The {nameof(VariableKind)}: {variableKind} was not recognized.");
		}

        model.Binder.BindVariableDeclarationNode(variableDeclarationNode, model);
        model.CurrentCodeBlockBuilder.ChildList.Add(variableDeclarationNode);

        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
        {
            if (model.TokenWalker.Peek(1).SyntaxKind == SyntaxKind.CloseAngleBracketToken)
            {
                HandlePropertyExpression(
                    variableDeclarationNode,
                    (EqualsToken)model.TokenWalker.Consume(),
                    (CloseAngleBracketToken)model.TokenWalker.Consume(),
                    model);
            }
            else
            {
                // Variable initialization occurs here.
                HandleVariableAssignment(
                    consumedIdentifierToken,
                    (EqualsToken)model.TokenWalker.Consume(),
                    model);
            }
        }
        else
        {
            if (variableDeclarationNode.TypeClauseNode.TypeIdentifierToken.SyntaxKind ==
                SyntaxKind.VarTokenContextualKeyword)
            {
                model.DiagnosticBag.ReportImplicitlyTypedVariablesMustBeInitialized(
                    consumedIdentifierToken.TextSpan);
            }
        }

        if (variableKind == VariableKind.Property &&
            model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
        {
            HandlePropertyDeclaration(
                variableDeclarationNode,
                (OpenBraceToken)model.TokenWalker.Consume(),
                model);
        }
        else
        {
            _ = model.TokenWalker.Match(SyntaxKind.StatementDelimiterToken);
        }
    }

    public static void HandlePropertyDeclaration(
        IVariableDeclarationNode consumedVariableDeclarationNode,
        OpenBraceToken consumedOpenBraceToken,
        CSharpParserModel model)
    {
        while (!model.TokenWalker.IsEof)
        {
            var token = model.TokenWalker.Consume();

            if (UtilityApi.IsAccessibilitySyntaxKind(token.SyntaxKind))
            {
                model.DiagnosticBag.ReportTodoException(token.TextSpan, "TODO: Implement accessibility modifiers for properties.");
                continue;
            }
            else if (token.SyntaxKind == SyntaxKind.GetTokenContextualKeyword)
            {
                consumedVariableDeclarationNode.HasGetter = true;

                if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.StatementDelimiterToken)
                {
                    _ = model.TokenWalker.Consume();
                    consumedVariableDeclarationNode.GetterIsAutoImplemented = true;
                }
                else if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
                {
                    // TODO: Parse getter body
                }
            }
            else if (token.SyntaxKind == SyntaxKind.SetTokenContextualKeyword)
            {
                consumedVariableDeclarationNode.HasSetter = true;

                if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.StatementDelimiterToken)
                {
                    _ = model.TokenWalker.Consume();
                    consumedVariableDeclarationNode.SetterIsAutoImplemented = true;
                }
                else if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
                {
                    // TODO: Parse setter body
                }
            }
            else if (token.SyntaxKind == SyntaxKind.CloseBraceToken)
            {
                break;
            }
            else
            {
                // TODO: Remove this else block if it is uneccessary
                model.DiagnosticBag.ReportTodoException(token.TextSpan, "TODO: Implement parsing for this property syntax.");
                continue;
            }
        }

        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
        {
            // Property initialization occurs here.
            HandleVariableAssignment(
                consumedVariableDeclarationNode.IdentifierToken,
                (EqualsToken)model.TokenWalker.Consume(),
                model);
        }
    }

    public static void HandlePropertyExpression(
        IVariableDeclarationNode consumedVariableDeclarationNode,
        EqualsToken consumedEqualsToken,
        CloseAngleBracketToken consumedCloseAngleBracketToken,
        CSharpParserModel model)
    {
        ParseOthers.HandleExpression(
            null,
            null,
            null,
            null,
            null,
            null,
            model);

        consumedVariableDeclarationNode.HasGetter = true;
    }

    public static void HandleVariableAssignment(
        IdentifierToken consumedIdentifierToken,
        EqualsToken consumedEqualsToken,
        CSharpParserModel model)
    {
        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.NewTokenKeyword)
        {
            ParseFunctions.HandleConstructorInvocation(model);
        }
        else
        {
            ParseOthers.HandleExpression(
                null,
                null,
                null,
                null,
                null,
                null,
                model);
        }

        var rightHandExpression = (IExpressionNode)model.SyntaxStack.Pop();

        var variableAssignmentExpressionNode = new VariableAssignmentExpressionNode(
            consumedIdentifierToken,
            consumedEqualsToken,
            rightHandExpression);

        model.Binder.BindVariableAssignmentExpressionNode(variableAssignmentExpressionNode, model);
        model.CurrentCodeBlockBuilder.ChildList.Add(variableAssignmentExpressionNode);
    }
}
