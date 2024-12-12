using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public class ParseContextualKeywords
{
    public static void HandleVarTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	if (compilationUnit.ParserModel.StatementBuilder.ChildList.Count == 0)
    		ParseTokens.ParseIdentifierToken(compilationUnit);
    	else
    		ParseOthers.StartStatement_Expression(compilationUnit);
    }

    public static void HandlePartialTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleAddTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleAndTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleAliasTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleAscendingTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleArgsTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleAsyncTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleAwaitTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleByTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleDescendingTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleDynamicTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleEqualsTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleFileTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleFromTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleGetTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleGlobalTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleGroupTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleInitTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleIntoTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleJoinTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleLetTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleManagedTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleNameofTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleNintTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleNotTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleNotnullTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleNuintTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleOnTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleOrTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleOrderbyTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleRecordTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
        ParseDefaultKeywords.HandleStorageModifierTokenKeyword(compilationUnit);
    }

    public static void HandleRemoveTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleRequiredTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleScopedTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleSelectTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleSetTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleUnmanagedTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleValueTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleWhenTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleWhereTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	var whereTokenContextualKeyword = (KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume();
    	
    	if (compilationUnit.ParserModel.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind != SyntaxKind.TypeDefinitionNode)
    		return;
    	
    	while (!compilationUnit.ParserModel.TokenWalker.IsEof)
    	{
    		if (compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken ||
    			compilationUnit.ParserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.StatementDelimiterToken)
    		{
    			break;
    		}
    		
    		_ = compilationUnit.ParserModel.TokenWalker.Consume();
    	}
    }

    public static void HandleWithTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleYieldTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }

    public static void HandleUnrecognizedTokenContextualKeyword(CSharpCompilationUnit compilationUnit)
    {
    	compilationUnit.ParserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)compilationUnit.ParserModel.TokenWalker.Consume());
    }
}
