using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public class ParseContextualKeywords
{
    public static void HandleVarTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	if (parserModel.StatementBuilder.ChildList.Count == 0)
    		ParseTokens.ParseIdentifierToken(compilationUnit, ref parserModel);
    	else
    		ParseOthers.StartStatement_Expression(compilationUnit, ref parserModel);
    }

    public static void HandlePartialTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleAddTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleAndTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleAliasTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleAscendingTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleArgsTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleAsyncTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleAwaitTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleByTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleDescendingTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleDynamicTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleEqualsTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleFileTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleFromTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleGetTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleGlobalTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleGroupTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleInitTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleIntoTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleJoinTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleLetTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleManagedTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleNameofTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleNintTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleNotTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleNotnullTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleNuintTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleOnTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleOrTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleOrderbyTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleRecordTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
        ParseDefaultKeywords.HandleStorageModifierTokenKeyword(compilationUnit, ref parserModel);
    }

    public static void HandleRemoveTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleRequiredTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleScopedTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleSelectTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleSetTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleUnmanagedTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleValueTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleWhenTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleWhereTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var whereTokenContextualKeyword = (KeywordContextualToken)parserModel.TokenWalker.Consume();
    	
    	if (parserModel.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind != SyntaxKind.TypeDefinitionNode)
    		return;
    	
    	while (!parserModel.TokenWalker.IsEof)
    	{
    		if (parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken ||
    			parserModel.TokenWalker.Current.SyntaxKind == SyntaxKind.StatementDelimiterToken)
    		{
    			break;
    		}
    		
    		_ = parserModel.TokenWalker.Consume();
    	}
    }

    public static void HandleWithTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleYieldTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }

    public static void HandleUnrecognizedTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	parserModel.StatementBuilder.ChildList.Add((KeywordContextualToken)parserModel.TokenWalker.Consume());
    }
}
