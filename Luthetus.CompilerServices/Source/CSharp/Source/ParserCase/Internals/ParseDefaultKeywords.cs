using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Expression;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.CSharp.ParserCase.Internals;

public class ParseDefaultKeywords
{
    public static void HandleAsTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleBaseTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleBoolTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleBreakTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleByteTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleCaseTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleCatchTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleCharTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleCheckedTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleConstTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleContinueTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleDecimalTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleDefaultTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleDelegateTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleDoTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleDoubleTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleElseTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleEnumTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        HandleStorageModifierTokenKeyword(
            consumedKeywordToken,
            model);

        // Why was this method invocation here? (2024-01-23)
        //
        // HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleEventTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleExplicitTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleExternTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleFalseTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleFinallyTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleFixedTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleFloatTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleForTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleForeachTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleGotoTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleImplicitTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleInTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleIntTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleIsTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleLockTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleLongTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleNullTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleObjectTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleOperatorTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleOutTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleParamsTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleProtectedTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        model.SyntaxStack.Push(consumedKeywordToken);
    }

    public static void HandleReadonlyTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleRefTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleSbyteTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleShortTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleSizeofTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleStackallocTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleStringTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleStructTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        HandleStorageModifierTokenKeyword(
            consumedKeywordToken,
            model);
    }

    public static void HandleSwitchTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleThisTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleThrowTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleTrueTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleTryTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleTypeofTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleUintTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleUlongTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleUncheckedTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleUnsafeTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleUshortTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleVoidTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleVolatileTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleWhileTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleUnrecognizedTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleDefault(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {

        if (UtilityApi.IsTypeIdentifierKeywordSyntaxKind(consumedKeywordToken.SyntaxKind))
        {
            // One enters this conditional block with the 'keywordToken' having already been consumed.
            model.TokenWalker.Backtrack();
            var typeClauseNode = model.TokenWalker.MatchTypeClauseNode(model);
            model.SyntaxStack.Push(typeClauseNode);
        }
        else
        {
            throw new NotImplementedException($"Implement the {consumedKeywordToken.SyntaxKind} keyword.");
        }
    }

    public static void HandleTypeIdentifierKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {

        if (UtilityApi.IsTypeIdentifierKeywordSyntaxKind(consumedKeywordToken.SyntaxKind))
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
            throw new NotImplementedException($"Implement the {consumedKeywordToken.SyntaxKind} keyword.");
        }
    }

    public static void HandleNewTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
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
            ParseFunctions.HandleFunctionParameters(
                (OpenParenthesisToken)model.TokenWalker.Match(SyntaxKind.OpenParenthesisToken),
                model);

            ObjectInitializationNode? boundObjectInitializationNode = null;

            if (model.TokenWalker.Peek(0).SyntaxKind == SyntaxKind.OpenBraceToken)
            {
                ParseTypes.HandleObjectInitialization(
                    (OpenBraceToken)model.TokenWalker.Consume(),
                    model);

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
                ParseTypes.HandleGenericArguments(
                    (OpenAngleBracketToken)model.TokenWalker.Consume(),
                    model);
            }

            FunctionParametersListingNode? functionParametersListingNode = null;

            if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken)
            {
                ParseFunctions.HandleFunctionParameters(
                    (OpenParenthesisToken)model.TokenWalker.Consume(),
                    model);

                functionParametersListingNode = (FunctionParametersListingNode?)model.SyntaxStack.Pop();
            }

            ObjectInitializationNode? boundObjectInitializationNode = null;

            if (model.TokenWalker.Peek(0).SyntaxKind == SyntaxKind.OpenBraceToken)
            {
                ParseTypes.HandleObjectInitialization(
                    (OpenBraceToken)model.TokenWalker.Consume(),
                    model);

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

    public static void HandlePublicTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        model.SyntaxStack.Push(consumedKeywordToken);
    }

    public static void HandleInternalTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        model.SyntaxStack.Push(consumedKeywordToken);
    }

    public static void HandlePrivateTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        model.SyntaxStack.Push(consumedKeywordToken);
    }

    public static void HandleStaticTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleOverrideTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleVirtualTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleAbstractTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleSealedTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleIfTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
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

        var boundIfStatementNode = model.Binder.BindIfStatementNode(consumedKeywordToken, expression);
        model.SyntaxStack.Push(boundIfStatementNode);
    }

    public static void HandleUsingTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        ParseOthers.HandleNamespaceIdentifier(model);

        var handleNamespaceIdentifierResult = model.SyntaxStack.Pop();

        if (handleNamespaceIdentifierResult.SyntaxKind == SyntaxKind.EmptyNode)
        {
            model.DiagnosticBag.ReportTodoException(consumedKeywordToken.TextSpan, "Expected a namespace identifier.");
            return;
        }
        var namespaceIdentifier = (IdentifierToken)handleNamespaceIdentifierResult;

        var boundUsingStatementNode = model.Binder.BindUsingStatementNode(
            consumedKeywordToken,
            namespaceIdentifier,
            model);

        model.CurrentCodeBlockBuilder.ChildList.Add(boundUsingStatementNode);
        model.SyntaxStack.Push(boundUsingStatementNode);
    }

    public static void HandleInterfaceTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        ParseDefaultKeywords.HandleStorageModifierTokenKeyword(
            consumedKeywordToken,
            model);
    }

    public static void HandleStorageModifierTokenKeyword(
        ISyntaxToken consumedStorageModifierToken,
        ParserModel model)
    {
        IdentifierToken identifierToken;

        if (UtilityApi.IsContextualKeywordSyntaxKind(model.TokenWalker.Current.SyntaxKind))
        {
            var contextualKeywordToken = (KeywordContextualToken)model.TokenWalker.Consume();
            // Take the contextual keyword as an identifier
            identifierToken = new IdentifierToken(contextualKeywordToken.TextSpan);
        }
        else
        {
            identifierToken = (IdentifierToken)model.TokenWalker.Match(SyntaxKind.IdentifierToken);
        }

        GenericArgumentsListingNode? genericArgumentsListingNode = null;

        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
        {
            ParseTypes.HandleGenericArguments(
                (OpenAngleBracketToken)model.TokenWalker.Consume(),
                model);

            genericArgumentsListingNode = (GenericArgumentsListingNode?)model.SyntaxStack.Pop();
        }

        var storageModifierKind = UtilityApi.GetStorageModifierKindFromToken(consumedStorageModifierToken);
        
        if (storageModifierKind is null)
            return;

        var accessModifierKind = AccessModifierKind.Public;

        var hasPartialModifier = false;
        if (model.SyntaxStack.TryPeek(out var syntax) && syntax is ISyntaxToken syntaxToken)
        {
            if (syntaxToken.SyntaxKind == SyntaxKind.PartialTokenContextualKeyword)
            {
                _ = model.SyntaxStack.Pop();
                hasPartialModifier = true;
            }
        }

        if (model.SyntaxStack.TryPeek(out syntax) && syntax is ISyntaxToken firstSyntaxToken)
        {
            var firstOutput = UtilityApi.GetAccessModifierKindFromToken(firstSyntaxToken);

            if (firstOutput is not null)
            {
                _ = model.SyntaxStack.Pop();
                accessModifierKind = firstOutput.Value;

                if (model.SyntaxStack.TryPeek(out syntax) && syntax is ISyntaxToken secondSyntaxToken)
                {
                    var secondOutput = UtilityApi.GetAccessModifierKindFromToken(secondSyntaxToken);

                    if (secondOutput is not null)
                    {
                        _ = model.SyntaxStack.Pop();

                        if ((firstOutput.Value.ToString().ToLower() == "protected" &&
                                secondOutput.Value.ToString().ToLower() == "internal") ||
                            (firstOutput.Value.ToString().ToLower() == "internal" &&
                                secondOutput.Value.ToString().ToLower() == "protected"))
                        {
                            accessModifierKind = AccessModifierKind.ProtectedInternal;
                        }
                        else if ((firstOutput.Value.ToString().ToLower() == "private" &&
                                    secondOutput.Value.ToString().ToLower() == "protected") ||
                                (firstOutput.Value.ToString().ToLower() == "protected" &&
                                    secondOutput.Value.ToString().ToLower() == "private"))
                        {
                            accessModifierKind = AccessModifierKind.PrivateProtected;
                        }
                        // else use the firstOutput.
                    }
                }
            }
        }

        var typeDefinitionNode = new TypeDefinitionNode(
            accessModifierKind,
            hasPartialModifier,
            storageModifierKind.Value,
            identifierToken,
            null,
            genericArgumentsListingNode,
            null,
            null,
            null);

        model.Binder.BindTypeDefinitionNode(typeDefinitionNode, model);
        model.Binder.BindTypeIdentifier(identifierToken, model);
        model.SyntaxStack.Push(typeDefinitionNode);
    }

    public static void HandleClassTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        HandleStorageModifierTokenKeyword(
            consumedKeywordToken,
            model);
    }

    public static void HandleNamespaceTokenKeyword(
        KeywordToken consumedKeywordToken,
        ParserModel model)
    {
        ParseOthers.HandleNamespaceIdentifier(model);

        var handleNamespaceIdentifierResult = model.SyntaxStack.Pop();

        if (handleNamespaceIdentifierResult.SyntaxKind == SyntaxKind.EmptyNode)
        {
            model.DiagnosticBag.ReportTodoException(consumedKeywordToken.TextSpan, "Expected a namespace identifier.");
            return;
        }
        var namespaceIdentifier = (IdentifierToken)handleNamespaceIdentifierResult;

        if (model.FinalizeNamespaceFileScopeCodeBlockNodeAction is not null)
            model.DiagnosticBag.ReportTodoException(consumedKeywordToken.TextSpan, "Need to add logic to report diagnostic when there is already a file scoped namespace.");

        var namespaceStatementNode = new NamespaceStatementNode(
            consumedKeywordToken,
            namespaceIdentifier,
            new CodeBlockNode(ImmutableArray<ISyntax>.Empty));

        model.Binder.SetCurrentNamespaceStatementNode(namespaceStatementNode, model);

        model.SyntaxStack.Push(namespaceStatementNode);
    }

    public static void HandleReturnTokenKeyword(
        KeywordToken consumedKeywordToken,
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

        var returnExpression = (IExpressionNode)model.SyntaxStack.Pop();

        var returnStatementNode = model.Binder.BindReturnStatementNode(
            consumedKeywordToken,
            returnExpression);

        model.CurrentCodeBlockBuilder.ChildList.Add(returnStatementNode);
        model.SyntaxStack.Push(returnStatementNode);
    }
}
