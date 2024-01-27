using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.CompilerServices.Lang.CSharp.ParserCase.Internals;

public static class ParseVariables
{
    public static void HandleVariableReference(
        IdentifierToken consumedIdentifierToken,
        ParserModel model)
    {
        var variableReferenceNode = model.Binder.ConstructAndBindVariableReferenceNode(consumedIdentifierToken);
        model.SyntaxStack.Push(variableReferenceNode);
    }

    public static void HandleVariableDeclaration(
        TypeClauseNode consumedTypeClauseNode,
        IdentifierToken consumedIdentifierToken,
        VariableKind variableKind,
        ParserModel model)
    {
        var variableDeclarationNode = new VariableDeclarationNode(
            consumedTypeClauseNode,
            consumedIdentifierToken,
            variableKind,
            false);

        model.Binder.BindVariableDeclarationNode(variableDeclarationNode);
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
        VariableDeclarationNode consumedVariableDeclarationNode,
        OpenBraceToken consumedOpenBraceToken,
        ParserModel model)
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
        VariableDeclarationNode consumedVariableDeclarationNode,
        EqualsToken consumedEqualsToken,
        CloseAngleBracketToken consumedCloseAngleBracketToken,
        ParserModel model)
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
        ParserModel model)
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

        model.Binder.BindVariableAssignmentExpressionNode(variableAssignmentExpressionNode);
        model.CurrentCodeBlockBuilder.ChildList.Add(variableAssignmentExpressionNode);
    }
}
