using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.CompilerServices.Lang.CSharp.ParserCase.Internals;

public static class ParseVariables
{
    public static void HandleVariableReference(ParserModel model)
    {
        var variableDeclarationStatementNode = (VariableDeclarationNode)model.SyntaxStack.Pop();
        var identifierToken = (IdentifierToken)model.SyntaxStack.Pop();

        var variableReferenceNode = new VariableReferenceNode(
            identifierToken,
            variableDeclarationStatementNode);

        model.Binder.BindVariableReferenceNode(variableReferenceNode);
        model.SyntaxStack.Push(variableReferenceNode);
    }

    public static void HandleVariableDeclaration(VariableKind variableKind, ParserModel model)
    {
        GenericArgumentsListingNode? genericArgumentsListingNode =
            model.SyntaxStack.Peek().SyntaxKind == SyntaxKind.GenericArgumentsListingNode
                ? (GenericArgumentsListingNode)model.SyntaxStack.Pop()
                : null;

        var identifierToken = (IdentifierToken)model.SyntaxStack.Pop();
        var typeClauseNode = (TypeClauseNode)model.SyntaxStack.Pop();

        var variableDeclarationNode = new VariableDeclarationNode(
            typeClauseNode,
            identifierToken,
            variableKind,
            false);

        model.Binder.BindVariableDeclarationStatementNode(variableDeclarationNode);
        model.CurrentCodeBlockBuilder.ChildList.Add(variableDeclarationNode);

        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
        {
            if (model.TokenWalker.Peek(1).SyntaxKind == SyntaxKind.CloseAngleBracketToken)
            {
                model.SyntaxStack.Push(variableDeclarationNode);
                HandlePropertyExpression(model);
            }
            else
            {
                // Variable initialization occurs here.
                model.SyntaxStack.Push(identifierToken);
                HandleVariableAssignment(model);
            }
        }

        if (variableKind == VariableKind.Property &&
            model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
        {
            model.SyntaxStack.Push(variableDeclarationNode);
            model.SyntaxStack.Push((OpenBraceToken)model.TokenWalker.Consume());
            HandlePropertyDeclaration(model);
        }
        else
        {
            _ = model.TokenWalker.Match(SyntaxKind.StatementDelimiterToken);
        }
    }

    public static void HandlePropertyDeclaration(ParserModel model)
    {
        var openBraceToken = (OpenBraceToken)model.SyntaxStack.Pop();
        var variableDeclarationNode = (VariableDeclarationNode)model.SyntaxStack.Pop();

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
                variableDeclarationNode.HasGetter = true;

                if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.StatementDelimiterToken)
                {
                    _ = model.TokenWalker.Consume();
                    variableDeclarationNode.GetterIsAutoImplemented = true;
                }
                else if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
                {
                    // TODO: Parse getter body
                }
            }
            else if (token.SyntaxKind == SyntaxKind.SetTokenContextualKeyword)
            {
                variableDeclarationNode.HasSetter = true;

                if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.StatementDelimiterToken)
                {
                    _ = model.TokenWalker.Consume();
                    variableDeclarationNode.SetterIsAutoImplemented = true;
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
            model.SyntaxStack.Push(variableDeclarationNode.IdentifierToken);
            HandleVariableAssignment(model);
        }
    }

    public static void HandlePropertyExpression(ParserModel model)
    {
        var variableDeclarationNode = (VariableDeclarationNode)model.SyntaxStack.Pop();
        var equalsToken = (EqualsToken)model.TokenWalker.Consume();
        var closeAngleBracketToken = (CloseAngleBracketToken)model.TokenWalker.Consume();

        ParseOthers.HandleExpression(
            null,
            null,
            null,
            null,
            null,
            null,
            model);

        variableDeclarationNode.HasGetter = true;
    }

    public static void HandleVariableAssignment(ParserModel model)
    {
        var identifierToken = (IdentifierToken)model.SyntaxStack.Pop();
        var equalsToken = (EqualsToken)model.TokenWalker.Consume(); // Move past the EqualsToken

        if (UtilityApi.IsKeywordSyntaxKind(model.TokenWalker.Current.SyntaxKind))
        {
            if (model.TokenWalker.Current.SyntaxKind != SyntaxKind.NewTokenKeyword)
                return;

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
            identifierToken,
            equalsToken,
            rightHandExpression);

        model.Binder.BindVariableAssignmentExpressionNode(variableAssignmentExpressionNode);
        model.CurrentCodeBlockBuilder.ChildList.Add(variableAssignmentExpressionNode);
    }
}
