using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public class ParseContextualKeywords
{
    public static void HandleVarTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	if (model.StatementBuilder.ChildList.Count == 0)
    		ParseTokens.ParseIdentifierToken(model);
    	else
    		ParseOthers.StartStatement_Expression(model);
    }

    public static void HandlePartialTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleAddTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleAndTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleAliasTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleAscendingTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleArgsTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleAsyncTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleAwaitTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleByTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleDescendingTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleDynamicTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleEqualsTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleFileTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleFromTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleGetTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleGlobalTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleGroupTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleInitTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleIntoTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleJoinTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleLetTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleManagedTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleNameofTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleNintTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleNotTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleNotnullTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleNuintTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleOnTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleOrTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleOrderbyTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleRecordTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
        ParseDefaultKeywords.HandleStorageModifierTokenKeyword(model);
    }

    public static void HandleRemoveTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleRequiredTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleScopedTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleSelectTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleSetTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleUnmanagedTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleValueTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleWhenTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleWhereTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var whereTokenContextualKeyword = (KeywordContextualToken)model.TokenWalker.Consume();
    	
    	if (model.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind != SyntaxKind.TypeDefinitionNode)
    		return;
    	
    	while (!model.TokenWalker.IsEof)
    	{
    		if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken ||
    			model.TokenWalker.Current.SyntaxKind == SyntaxKind.StatementDelimiterToken)
    		{
    			break;
    		}
    		
    		_ = model.TokenWalker.Consume();
    	}
    }

    public static void HandleWithTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleYieldTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }

    public static void HandleUnrecognizedTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	model.StatementBuilder.ChildList.Add((KeywordContextualToken)model.TokenWalker.Consume());
    }
}
