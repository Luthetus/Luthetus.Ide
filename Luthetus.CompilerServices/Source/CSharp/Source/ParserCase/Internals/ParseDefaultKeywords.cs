using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.CompilerServices.Lang.CSharp.ParserCase.Internals;

public class ParseDefaultKeywords
{
    public static void HandleAsTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleBaseTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleBoolTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleBreakTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleByteTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleCaseTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleCatchTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleCharTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleCheckedTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleConstTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleContinueTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleDecimalTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleDefaultTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleDelegateTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleDoTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleDoubleTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleElseTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleEnumTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleEventTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleExplicitTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleExternTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleFalseTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleFinallyTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleFixedTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleFloatTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleForTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleForeachTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleGotoTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleImplicitTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleInTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleIntTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleIsTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleLockTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleLongTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleNullTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleObjectTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleOperatorTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleOutTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleParamsTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleProtectedTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleReadonlyTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleRefTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleSbyteTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleShortTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleSizeofTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleStackallocTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleStringTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleStructTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleSwitchTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleThisTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleThrowTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleTrueTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleTryTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleTypeofTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleUintTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleUlongTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleUncheckedTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleUnsafeTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleUshortTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleVoidTokenKeyword(ParserModel model)
    {
        HandleTypeIdentifierKeyword(model);
    }

    public static void HandleVolatileTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleWhileTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleUnrecognizedTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleDefault(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();

        if (UtilityApi.IsTypeIdentifierKeywordSyntaxKind(keywordToken.SyntaxKind))
        {
            // One enters this conditional block with the 'keywordToken' having already been consumed.
            model.TokenWalker.Backtrack();
            var typeClauseNode = model.TokenWalker.MatchTypeClauseNode(model);
            model.SyntaxStack.Push(typeClauseNode);
        }
        else
        {
            throw new NotImplementedException($"Implement the {keywordToken.SyntaxKind} keyword.");
        }
    }

    public static void HandleTypeIdentifierKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();

        if (UtilityApi.IsTypeIdentifierKeywordSyntaxKind(keywordToken.SyntaxKind))
        {
            // One enters this conditional block with the 'keywordToken' having already been consumed.
            model.TokenWalker.Backtrack();
            var typeClauseNode = model.TokenWalker.MatchTypeClauseNode(model);

            if (model.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind == SyntaxKind.AttributeNode)
                typeClauseNode.AttributeNode = (AttributeNode)model.SyntaxStack.Pop();

            model.SyntaxStack.Push(typeClauseNode);
        }
        else
        {
            throw new NotImplementedException($"Implement the {keywordToken.SyntaxKind} keyword.");
        }
    }

    public static void HandleNewTokenKeyword(ParserModel model)
    {
        var typeClauseToken = model.TokenWalker.MatchTypeClauseNode(model);

        if (model.TokenWalker.Peek(0).SyntaxKind == SyntaxKind.MemberAccessToken)
        {
            // "explicit namespace qualification" OR "nested class"
            throw new NotImplementedException();
        }

        // TODO: Fix _cSharpParser.model.Binder.TryGetClassReferenceHierarchically, it broke on (2023-07-26)
        //
        // _cSharpParser.model.Binder.TryGetClassReferenceHierarchically(typeClauseToken, null, out boundClassReferenceNode);

        // TODO: combine the logic for 'new()' without a type identifier and 'new List<int>()' with a type identifier. To start I am going to isolate them in their own if conditional blocks.
        if (typeClauseToken.IsFabricated)
        {
            // If "new()" LACKS a type identifier then the OpenParenthesisToken must be there. This is true even still for when there is object initialization OpenBraceToken. For new() the parenthesis are required.
            // valid inputs:
            //     new()
            //     new(){}
            //     new(...)
            //     new(...){}
            model.SyntaxStack.Push(model.TokenWalker.Match(SyntaxKind.OpenParenthesisToken));
            ParseFunctions.HandleFunctionParameters(model);

            ObjectInitializationNode? boundObjectInitializationNode = null;

            if (model.TokenWalker.Peek(0).SyntaxKind == SyntaxKind.OpenBraceToken)
            {
                model.SyntaxStack.Push((OpenBraceToken)model.TokenWalker.Consume());
                ParseTypes.HandleObjectInitialization(model);
                boundObjectInitializationNode = (ObjectInitializationNode?)model.SyntaxStack.Pop();
            }

            // TODO: Fix _cSharpParser.model.Binder.BindConstructorInvocationNode, it broke on (2023-07-26)
            //
            // var boundConstructorInvocationNode = _cSharpParser.model.Binder.BindConstructorInvocationNode(
            //     keywordToken,
            //     boundClassReferenceNode,
            //     boundFunctionArgumentsNode,
            //     boundObjectInitializationNode);
            //
            // _cSharpParser._currentCodeBlockBuilder.Children.Add(boundConstructorInvocationNode);
        }
        else
        {
            // If "new List<int>()" HAS a type identifier then the OpenParenthesisToken is optional, given that the object initializer syntax OpenBraceToken is found, and one wishes to invoke the parameterless constructor.
            // valid inputs:
            //     new List<int>()
            //     new List<int>(){}
            //     new List<int>{}
            //     new List<int>(...)
            //     new List<int>(...){}
            //     new string(...){}

            if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
            {
                model.SyntaxStack.Push((OpenAngleBracketToken)model.TokenWalker.Consume());
                ParseTypes.HandleGenericArguments(model);
            }

            FunctionParametersListingNode? functionParametersListingNode = null;

            if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken)
            {
                model.SyntaxStack.Push((OpenParenthesisToken)model.TokenWalker.Consume());
                ParseFunctions.HandleFunctionParameters(model);
                functionParametersListingNode = (FunctionParametersListingNode?)model.SyntaxStack.Pop();
            }

            ObjectInitializationNode? boundObjectInitializationNode = null;

            if (model.TokenWalker.Peek(0).SyntaxKind == SyntaxKind.OpenBraceToken)
            {
                model.SyntaxStack.Push((OpenBraceToken)model.TokenWalker.Consume());
                ParseTypes.HandleObjectInitialization(model);
                boundObjectInitializationNode = (ObjectInitializationNode?)model.SyntaxStack.Pop();
            }

            // TODO: Fix _cSharpParser.model.Binder.BindConstructorInvocationNode, it broke on (2023-07-26)
            //
            // var boundConstructorInvocationNode = _cSharpParser.model.Binder.BindConstructorInvocationNode(
            //     keywordToken,
            //     boundClassReferenceNode,
            //     functionParametersListingNode,
            //     boundObjectInitializationNode);
            //
            // _cSharpParser._currentCodeBlockBuilder.Children.Add(boundConstructorInvocationNode);
        }
    }

    public static void HandlePublicTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleInternalTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandlePrivateTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleStaticTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleOverrideTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleVirtualTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleAbstractTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleSealedTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        // TODO: Implement this method
    }

    public static void HandleIfTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        var openParenthesisToken = model.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);

        if (openParenthesisToken.IsFabricated)
            return;

        ParseOthers.HandleExpression(
            null,
            null,
            null,
            null,
            null,
            new[]
            {
                new ExpressionDelimiter(
                    SyntaxKind.OpenParenthesisToken,
                    SyntaxKind.CloseParenthesisToken,
                    null,
                    null)
            },
            model);

        var expression = (IExpressionNode)model.SyntaxStack.Pop();

        var boundIfStatementNode = model.Binder.BindIfStatementNode(keywordToken, expression);
        model.SyntaxStack.Push(boundIfStatementNode);
    }

    public static void HandleUsingTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        ParseOthers.HandleNamespaceIdentifier(model);

        var handleNamespaceIdentifierResult = model.SyntaxStack.Pop();

        if (handleNamespaceIdentifierResult.SyntaxKind == SyntaxKind.EmptyNode)
        {
            model.DiagnosticBag.ReportTodoException(keywordToken.TextSpan, "Expected a namespace identifier.");
            return;
        }
        var namespaceIdentifier = (IdentifierToken)handleNamespaceIdentifierResult;

        var boundUsingStatementNode = model.Binder.BindUsingStatementNode(
            keywordToken,
            namespaceIdentifier);

        model.CurrentCodeBlockBuilder.ChildList.Add(boundUsingStatementNode);
        model.SyntaxStack.Push(boundUsingStatementNode);
    }

    public static void HandleInterfaceTokenKeyword(ParserModel model)
    {
        var identifierToken = (IdentifierToken)model.TokenWalker.Match(SyntaxKind.IdentifierToken);
        GenericArgumentsListingNode? genericArgumentsListingNode = null;

        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
        {
            model.SyntaxStack.Push((OpenAngleBracketToken)model.TokenWalker.Consume());
            ParseTypes.HandleGenericArguments(model);

            genericArgumentsListingNode = (GenericArgumentsListingNode?)model.SyntaxStack.Pop();
        }

        var typeDefinitionNode = new TypeDefinitionNode(
            identifierToken,
            null,
            genericArgumentsListingNode,
            null,
            null)
        {
            IsInterface = true
        };

        model.Binder.BindTypeDefinitionNode(typeDefinitionNode);
        model.Binder.BindTypeIdentifier(identifierToken);
        model.SyntaxStack.Push(typeDefinitionNode);
    }

    public static void HandleClassTokenKeyword(ParserModel model)
    {
        var identifierToken = (IdentifierToken)model.TokenWalker.Match(SyntaxKind.IdentifierToken);
        GenericArgumentsListingNode? genericArgumentsListingNode = null;

        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
        {
            model.SyntaxStack.Push((OpenAngleBracketToken)model.TokenWalker.Consume());
            ParseTypes.HandleGenericArguments(model);

            genericArgumentsListingNode = (GenericArgumentsListingNode?)model.SyntaxStack.Pop();
        }

        var typeDefinitionNode = new TypeDefinitionNode(
            identifierToken,
            null,
            genericArgumentsListingNode,
            null,
            null);

        model.Binder.BindTypeDefinitionNode(typeDefinitionNode);
        model.Binder.BindTypeIdentifier(identifierToken);
        model.SyntaxStack.Push(typeDefinitionNode);
    }

    public static void HandleNamespaceTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();
        ParseOthers.HandleNamespaceIdentifier(model);

        var handleNamespaceIdentifierResult = model.SyntaxStack.Pop();

        if (handleNamespaceIdentifierResult.SyntaxKind == SyntaxKind.EmptyNode)
        {
            model.DiagnosticBag.ReportTodoException(keywordToken.TextSpan, "Expected a namespace identifier.");
            return;
        }
        var namespaceIdentifier = (IdentifierToken)handleNamespaceIdentifierResult;

        if (model.FinalizeNamespaceFileScopeCodeBlockNodeAction is not null)
            model.DiagnosticBag.ReportTodoException(keywordToken.TextSpan, "Need to add logic to report diagnostic when there is already a file scoped namespace.");

        var namespaceStatementNode = new NamespaceStatementNode(
            keywordToken,
            namespaceIdentifier,
            new CodeBlockNode(ImmutableArray<ISyntax>.Empty));

        model.SyntaxStack.Push(namespaceStatementNode);
    }

    public static void HandleReturnTokenKeyword(ParserModel model)
    {
        var keywordToken = (KeywordToken)model.SyntaxStack.Pop();

        ParseOthers.HandleExpression(
            null,
            null,
            null,
            null,
            null,
            null,
            model);

        var returnExpression = (IExpressionNode)model.SyntaxStack.Pop();

        var returnStatementNode = model.Binder.BindReturnStatementNode(
            keywordToken,
            returnExpression);

        model.CurrentCodeBlockBuilder.ChildList.Add(returnStatementNode);
        model.SyntaxStack.Push(returnStatementNode);
    }
}
