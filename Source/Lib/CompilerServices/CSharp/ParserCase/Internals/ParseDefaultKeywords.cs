using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public class ParseDefaultKeywords
{
    public static void HandleAsTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleBaseTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleBoolTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleBreakTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleByteTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleCaseTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleCatchTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleCharTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleCheckedTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleConstTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleContinueTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleDecimalTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleDefaultTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleDelegateTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleDoTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleDoubleTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleElseTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleEnumTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
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
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleExplicitTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleExternTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleFalseTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleFinallyTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleFixedTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleFloatTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleForTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleForeachTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
    	var openParenthesisToken = (OpenParenthesisToken)model.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	var typeClauseNode = model.TokenWalker.MatchTypeClauseNode(model);
    	
    	var identifierToken = (IdentifierToken)model.TokenWalker.Match(SyntaxKind.IdentifierToken);
    	
    	var inKeywordToken = (KeywordToken)model.TokenWalker.Match(SyntaxKind.InTokenKeyword);
    	
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
	        
		var expressionNode = (IExpressionNode)model.SyntaxStack.Pop();
		var closeParenthesisToken = (CloseParenthesisToken)model.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
		
		var foreachStatementNode = new ForeachStatementNode(
	        consumedKeywordToken,
	        openParenthesisToken,
	        identifierToken,
	        inKeywordToken,
	        expressionNode,
	        closeParenthesisToken,
	        codeBlockNode: null);
	        
	    model.CurrentCodeBlockBuilder.ChildList.Add(foreachStatementNode);
        model.SyntaxStack.Push(foreachStatementNode);
    }

    public static void HandleGotoTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleImplicitTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleInTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleIntTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleIsTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleLockTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        var openParenthesisToken = (OpenParenthesisToken)model.TokenWalker.Match(SyntaxKind.OpenParenthesisToken);
    	
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
	        
		var expressionNode = (IExpressionNode)model.SyntaxStack.Pop();
		var closeParenthesisToken = (CloseParenthesisToken)model.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);
		
		var lockStatementNode = new LockStatementNode(
			consumedKeywordToken,
	        openParenthesisToken,
	        expressionNode,
	        closeParenthesisToken,
	        codeBlockNode: null);
	        
	    model.CurrentCodeBlockBuilder.ChildList.Add(lockStatementNode);
        model.SyntaxStack.Push(lockStatementNode);
    }

    public static void HandleLongTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleNullTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleObjectTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleOperatorTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleOutTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleParamsTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleProtectedTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        model.SyntaxStack.Push(consumedKeywordToken);
    }

    public static void HandleReadonlyTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleRefTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleSbyteTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleShortTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleSizeofTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleStackallocTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleStringTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleStructTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleStorageModifierTokenKeyword(
            consumedKeywordToken,
            model);
    }

    public static void HandleSwitchTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleThisTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleThrowTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleTrueTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleTryTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleTypeofTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleUintTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleUlongTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleUncheckedTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleUnsafeTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleUshortTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleVoidTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleTypeIdentifierKeyword(consumedKeywordToken, model);
    }

    public static void HandleVolatileTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleWhileTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleUnrecognizedTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleDefault(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
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
        	model.DiagnosticBag.ReportTodoException(consumedKeywordToken.TextSpan, $"Implement the {consumedKeywordToken.SyntaxKind} keyword.");
        }
    }

    public static void HandleTypeIdentifierKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
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
        	model.DiagnosticBag.ReportTodoException(consumedKeywordToken.TextSpan, $"Implement the {consumedKeywordToken.SyntaxKind} keyword.");
        }
    }

    public static void HandleNewTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        var typeClauseToken = model.TokenWalker.MatchTypeClauseNode(model);

        if (model.TokenWalker.Peek(0).SyntaxKind == SyntaxKind.MemberAccessToken)
        {
            // "explicit namespace qualification" OR "nested class"

            var memberAccessToken = (MemberAccessToken)model.TokenWalker.Peek(0);

            model.DiagnosticBag.ReportTodoException(
                memberAccessToken.TextSpan,
                $"Implement: \"explicit namespace qualification\" OR \"nested class\"");
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
        CSharpParserModel model)
    {
        model.SyntaxStack.Push(consumedKeywordToken);
    }

    public static void HandleInternalTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        model.SyntaxStack.Push(consumedKeywordToken);
    }

    public static void HandlePrivateTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        model.SyntaxStack.Push(consumedKeywordToken);
    }

    public static void HandleStaticTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleOverrideTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleVirtualTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleAbstractTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleSealedTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        // TODO: Implement this method
    }

    public static void HandleIfTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
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
        CSharpParserModel model)
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
        CSharpParserModel model)
    {
        ParseDefaultKeywords.HandleStorageModifierTokenKeyword(
            consumedKeywordToken,
            model);
    }

	/// <summary>
	/// Example:
	/// public class MyClass { }
	///              ^
	///
	/// Given the example the 'MyClass' is the current token
	/// upon invocation of this method.
	///
	/// Invocation of this method implies the previous token was
	/// class, interface, struct, etc...
	///
	/// The syntax token parameter to this method is said
	/// previous token.
	/// </summary>
    public static void HandleStorageModifierTokenKeyword(
        ISyntaxToken consumedStorageModifierToken,
        CSharpParserModel model)
    {
    	// Given: public partial class MyClass { }
		// Then: partial
        var hasPartialModifier = false;
        if (model.SyntaxStack.TryPeek(out var syntax) && syntax is ISyntaxToken syntaxToken)
        {
            if (syntaxToken.SyntaxKind == SyntaxKind.PartialTokenContextualKeyword)
            {
                _ = model.SyntaxStack.Pop();
                hasPartialModifier = true;
            }
        }
    
    	// TODO: Fix; the code that parses the accessModifierKind is a mess
		//
		// Given: public class MyClass { }
		// Then: public
		var accessModifierKind = AccessModifierKind.Public;
        if (model.SyntaxStack.TryPeek(out syntax) && syntax is ISyntaxToken firstSyntaxToken)
        {
            var firstOutput = UtilityApi.GetAccessModifierKindFromToken(firstSyntaxToken);

            if (firstOutput is not null)
            {
                _ = model.SyntaxStack.Pop();
                accessModifierKind = firstOutput.Value;

				// Given: protected internal class MyClass { }
				// Then: protected internal
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
    
    	// TODO: Fix nullability spaghetti code
        var storageModifierKind = UtilityApi.GetStorageModifierKindFromToken(consumedStorageModifierToken);
        if (storageModifierKind is null)
            return;
        if (storageModifierKind == StorageModifierKind.Record &&
        	model.TokenWalker.Current.SyntaxKind == SyntaxKind.StructTokenKeyword)
        {
        	var structKeywordToken = (KeywordToken)model.TokenWalker.Consume();
        	storageModifierKind = StorageModifierKind.RecordStruct;
        }
    
		// Given: public class MyClass<T> { }
		// Then: MyClass
		IdentifierToken identifierToken;
		// Retrospective: What is the purpose of this 'if (contextualKeyword) logic'?
		// Response: maybe it is because 'var' contextual keyword is allowed to be a class name?
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

		// Given: public class MyClass<T> { }
		// Then: <T>
        GenericArgumentsListingNode? genericArgumentsListingNode = null;
        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
        {
            ParseTypes.HandleGenericArguments(
                (OpenAngleBracketToken)model.TokenWalker.Consume(),
                model);

            genericArgumentsListingNode = (GenericArgumentsListingNode?)model.SyntaxStack.Pop();
        }

        var typeDefinitionNode = new TypeDefinitionNode(
            accessModifierKind,
            hasPartialModifier,
            storageModifierKind.Value,
            identifierToken,
            valueType: null,
            genericArgumentsListingNode,
            primaryConstructorFunctionArgumentsListingNode: null,
            inheritedTypeClauseNode: null,
            openBraceToken: null,
            codeBlockNode: null);

        model.Binder.BindTypeDefinitionNode(typeDefinitionNode, model);
        model.Binder.BindTypeIdentifier(identifierToken, model);
        model.SyntaxStack.Push(typeDefinitionNode);
        
        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.WhereTokenContextualKeyword)
        {
        	while (!model.TokenWalker.IsEof)
        	{
        		if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
        			break;
        		
        		_ = model.TokenWalker.Consume();
        	}
        }
    }

    public static void HandleClassTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
    {
        HandleStorageModifierTokenKeyword(
            consumedKeywordToken,
            model);
    }

    public static void HandleNamespaceTokenKeyword(
        KeywordToken consumedKeywordToken,
        CSharpParserModel model)
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

        var returnExpression = (IExpressionNode)model.SyntaxStack.Pop();

        var returnStatementNode = model.Binder.BindReturnStatementNode(
            consumedKeywordToken,
            returnExpression);

        model.CurrentCodeBlockBuilder.ChildList.Add(returnStatementNode);
        model.SyntaxStack.Push(returnStatementNode);
    }
}
