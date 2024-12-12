using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public class ParseFunctions
{
    public static void HandleFunctionDefinition(
        IdentifierToken consumedIdentifierToken,
        TypeClauseNode consumedTypeClauseNode,
        GenericParametersListingNode? consumedGenericArgumentsListingNode,
        CSharpCompilationUnit compilationUnit)
    {
    	if (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
    	{
    		var successGenericParametersListingNode = ParseOthers.TryParseExpression(
    			SyntaxKind.GenericParametersListingNode,
    			model,
    			out var genericParametersListingNode);
    			
    		if (successGenericParametersListingNode)
    			consumedGenericArgumentsListingNode = (GenericParametersListingNode)genericParametersListingNode;
    	}
    
        if (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenParenthesisToken)
            return;

        var functionArgumentsListingNode = HandleFunctionArguments(compilationUnit);

        var functionDefinitionNode = new FunctionDefinitionNode(
            AccessModifierKind.Public,
            consumedTypeClauseNode,
            consumedIdentifierToken,
            consumedGenericArgumentsListingNode,
            functionArgumentsListingNode,
            null,
            null);

        compilationUnit.ParserModel.Binder.BindFunctionDefinitionNode(functionDefinitionNode, compilationUnit);
        compilationUnit.ParserModel.SyntaxStack.Push(functionDefinitionNode);
        compilationUnit.ParserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = functionDefinitionNode;

        if (compilationUnit.ParserModel.CurrentCodeBlockBuilder.CodeBlockOwner is TypeDefinitionNode typeDefinitionNode &&
            typeDefinitionNode.IsInterface)
        {
            // TODO: Would method constraints break this code? "public T Aaa<T>() where T : OtherClass"
            var statementDelimiterToken = compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.StatementDelimiterToken);

			foreach (var argument in functionDefinitionNode.FunctionArgumentsListingNode.FunctionArgumentEntryNodeList)
	    	{
	    		if (argument.IsOptional)
	    			compilationUnit.ParserModel.Binder.BindFunctionOptionalArgument(argument, compilationUnit);
	    		else
	    			compilationUnit.ParserModel.Binder.BindVariableDeclarationNode(argument.VariableDeclarationNode, compilationUnit);
	    	}
        }
    }

    public static void HandleConstructorDefinition(
    	TypeDefinitionNode typeDefinitionNodeCodeBlockOwner,
        IdentifierToken consumedIdentifierToken,
        CSharpCompilationUnit compilationUnit)
    {
    	var functionArgumentsListingNode = HandleFunctionArguments(compilationUnit);

        var typeClauseNode = new TypeClauseNode(
            typeDefinitionNodeCodeBlockOwner.TypeIdentifierToken,
            null,
            null);

        var constructorDefinitionNode = new ConstructorDefinitionNode(
            typeClauseNode,
            consumedIdentifierToken,
            null,
            functionArgumentsListingNode,
            null,
            null);

        compilationUnit.ParserModel.Binder.BindConstructorDefinitionIdentifierToken(consumedIdentifierToken, compilationUnit);
        compilationUnit.ParserModel.SyntaxStack.Push(constructorDefinitionNode);
        compilationUnit.ParserModel.CurrentCodeBlockBuilder.InnerPendingCodeBlockOwner = constructorDefinitionNode;

        if (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.ColonToken)
        {
        	_ = compilationUnit.ParserModel.TokenWalker.Consume();
            // Constructor invokes some other constructor as well
        	// 'this(...)' or 'base(...)'
        	
        	KeywordToken keywordToken;
        	
        	if (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.ThisTokenKeyword)
        		keywordToken = (KeywordToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.ThisTokenKeyword);
        	else if (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.BaseTokenKeyword)
        		keywordToken = (KeywordToken)compilationUnit.ParserModel.TokenWalker.Match(SyntaxKind.BaseTokenKeyword);
        	else
        		keywordToken = default;
        	
        	while (!compilationUnit.ParserModel.TokenWalker.IsEof)
            {
            	// TODO: This won't work because an OpenBraceToken can appear inside the "other constructor invocation"...
            	// 	  ...If one were to skip over this syntax for the time being, it should be done by counting the
            	//       matched OpenParenthesisToken and CloseParenthesisToken until it evens out.
                if (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken ||
                    compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
                {
                    break;
                }

                _ = compilationUnit.ParserModel.TokenWalker.Consume();
            }
        }
    }

    /// <summary>Use this method for function definition, whereas <see cref="HandleFunctionParameters"/> should be used for function invocation.</summary>
    public static FunctionArgumentsListingNode HandleFunctionArguments(CSharpCompilationUnit compilationUnit)
    {
    	var openParenthesisToken = (OpenParenthesisToken)compilationUnit.ParserModel.TokenWalker.Consume();
    	var functionArgumentEntryNodeList = new List<FunctionArgumentEntryNode>();
    	var openParenthesisCount = 1;
    	var corruptState = false;
    	
    	while (!compilationUnit.ParserModel.TokenWalker.IsEof)
        {
        	if (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken)
        	{
        		openParenthesisCount++;
        	}
        	else if (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseParenthesisToken)
        	{
        		openParenthesisCount--;
        		
        		if (openParenthesisCount == 0)
        		{
        			break;
        		}
        	}
            else if (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
            {
                break;
            }
            else if (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken &&
            		 compilationUnit.ParserModel.TokenWalker.Next.SyntaxKind == SyntaxKind.CloseAngleBracketToken)
            {
            	break;
            }
            else if (!corruptState)
            {
            	var tokenIndexOriginal = compilationUnit.ParserModel.TokenWalker.Index;
				var successTypeClauseNode = ParseOthers.TryParseExpression(SyntaxKind.TypeClauseNode, model, out var typeClauseNode);
		    	var successName = false;
		    	
		    	if (successTypeClauseNode)
		    	{
		    		// 'TypeClauseNode' or 'VariableDeclarationNode'
		    		var successNameableToken = false;
		    		
		    		if (UtilityApi.IsConvertibleToIdentifierToken(compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind))
		    		{
		    			var identifierToken = UtilityApi.ConvertToIdentifierToken(compilationUnit.ParserModel.TokenWalker.Consume(), compilationUnit);
		    			successNameableToken = true;
		    			
		    			if (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
		    			{
		    				// Optional
		    			}
					        
					    var variableDeclarationNode = new VariableDeclarationNode(
					        (TypeClauseNode)typeClauseNode,
					        identifierToken,
					        VariableKind.Local,
					        false);
		    			
		    			var functionArgumentEntryNode = new FunctionArgumentEntryNode(
					        variableDeclarationNode,
					        optionalCompileTimeConstantToken: null,
					        isOptional: false,
					        hasParamsKeyword: false,
					        hasOutKeyword: false,
					        hasInKeyword: false,
					        hasRefKeyword: false);
		    			
		    			functionArgumentEntryNodeList.Add(functionArgumentEntryNode);
		    			
		    			if (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken)
		    				_ = compilationUnit.ParserModel.TokenWalker.Consume();
		    				
		    			if (tokenIndexOriginal < compilationUnit.ParserModel.TokenWalker.Index)
		    				continue; // Already consumed so avoid the one at the end of the while loop
		    		}
		    		
		    		if (!successNameableToken)
		    			corruptState = true;
		    	}
		    	else
		    	{
		    		corruptState = true;
		    	}
            }

            _ = compilationUnit.ParserModel.TokenWalker.Consume();
        }
        
        var closeParenthesisToken = default(CloseParenthesisToken);
        
        if (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.CloseParenthesisToken)
        	closeParenthesisToken = (CloseParenthesisToken)compilationUnit.ParserModel.TokenWalker.Consume();
        
        return new FunctionArgumentsListingNode(
        	openParenthesisToken,
	        functionArgumentEntryNodeList,
	        closeParenthesisToken);
    }
}
