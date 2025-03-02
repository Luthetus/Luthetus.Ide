using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public class ParseContextualKeywords
{
    public static void HandleVarTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	if (parserComputation.StatementBuilder.ChildList.Count == 0)
    		ParseTokens.ParseIdentifierToken(compilationUnit, ref parserComputation);
    	else
    		ParseOthers.StartStatement_Expression(compilationUnit, ref parserComputation);
    }

    public static void HandlePartialTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleAddTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleAndTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleAliasTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleAscendingTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleArgsTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleAsyncTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleAwaitTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleByTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleDescendingTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleDynamicTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleEqualsTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleFileTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleFromTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleGetTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleGlobalTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleGroupTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleInitTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleIntoTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleJoinTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleLetTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleManagedTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleNameofTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleNintTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleNotTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleNotnullTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleNuintTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleOnTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleOrTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleOrderbyTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleRecordTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
        ParseDefaultKeywords.HandleStorageModifierTokenKeyword(compilationUnit, ref parserComputation);
    }

    public static void HandleRemoveTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleRequiredTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleScopedTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleSelectTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleSetTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleUnmanagedTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleValueTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleWhenTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleWhereTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleWithTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleYieldTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }

    public static void HandleUnrecognizedTokenContextualKeyword(CSharpCompilationUnit compilationUnit, ref CSharpParserComputation parserComputation)
    {
    	parserComputation.StatementBuilder.ChildList.Add(parserComputation.TokenWalker.Consume());
    }
}
