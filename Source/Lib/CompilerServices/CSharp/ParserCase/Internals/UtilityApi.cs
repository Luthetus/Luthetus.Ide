using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public static class UtilityApi
{
    public static bool IsContextualKeywordSyntaxKind(SyntaxKind syntaxKind)
    {
        switch (syntaxKind)
		{
			case SyntaxKind.AddTokenContextualKeyword:
			case SyntaxKind.AndTokenContextualKeyword:
			case SyntaxKind.AliasTokenContextualKeyword:
			case SyntaxKind.AscendingTokenContextualKeyword:
			case SyntaxKind.ArgsTokenContextualKeyword:
			case SyntaxKind.AsyncTokenContextualKeyword:
			case SyntaxKind.AwaitTokenContextualKeyword:
			case SyntaxKind.ByTokenContextualKeyword:
			case SyntaxKind.DescendingTokenContextualKeyword:
			case SyntaxKind.DynamicTokenContextualKeyword:
			case SyntaxKind.EqualsTokenContextualKeyword:
			case SyntaxKind.FileTokenContextualKeyword:
			case SyntaxKind.FromTokenContextualKeyword:
			case SyntaxKind.GetTokenContextualKeyword:
			case SyntaxKind.GlobalTokenContextualKeyword:
			case SyntaxKind.GroupTokenContextualKeyword:
			case SyntaxKind.InitTokenContextualKeyword:
			case SyntaxKind.IntoTokenContextualKeyword:
			case SyntaxKind.JoinTokenContextualKeyword:
			case SyntaxKind.LetTokenContextualKeyword:
			case SyntaxKind.ManagedTokenContextualKeyword:
			case SyntaxKind.NameofTokenContextualKeyword:
			case SyntaxKind.NintTokenContextualKeyword:
			case SyntaxKind.NotTokenContextualKeyword:
			case SyntaxKind.NotnullTokenContextualKeyword:
			case SyntaxKind.NuintTokenContextualKeyword:
			case SyntaxKind.OnTokenContextualKeyword:
			case SyntaxKind.OrTokenContextualKeyword:
			case SyntaxKind.OrderbyTokenContextualKeyword:
			case SyntaxKind.PartialTokenContextualKeyword:
			case SyntaxKind.RecordTokenContextualKeyword:
			case SyntaxKind.RemoveTokenContextualKeyword:
			case SyntaxKind.RequiredTokenContextualKeyword:
			case SyntaxKind.ScopedTokenContextualKeyword:
			case SyntaxKind.SelectTokenContextualKeyword:
			case SyntaxKind.SetTokenContextualKeyword:
			case SyntaxKind.UnmanagedTokenContextualKeyword:
			case SyntaxKind.ValueTokenContextualKeyword:
			case SyntaxKind.VarTokenContextualKeyword:
			case SyntaxKind.WhenTokenContextualKeyword:
			case SyntaxKind.WhereTokenContextualKeyword:
			case SyntaxKind.WithTokenContextualKeyword:
			case SyntaxKind.YieldTokenContextualKeyword:
				return true;
			default:
				return false;
		}
    }

    public static bool IsKeywordSyntaxKind(SyntaxKind syntaxKind)
    {
        switch (syntaxKind)
		{
			case SyntaxKind.AbstractTokenKeyword:
			case SyntaxKind.AsTokenKeyword:
			case SyntaxKind.BaseTokenKeyword:
			case SyntaxKind.BoolTokenKeyword:
			case SyntaxKind.BreakTokenKeyword:
			case SyntaxKind.ByteTokenKeyword:
			case SyntaxKind.CaseTokenKeyword:
			case SyntaxKind.CatchTokenKeyword:
			case SyntaxKind.CharTokenKeyword:
			case SyntaxKind.CheckedTokenKeyword:
			case SyntaxKind.ClassTokenKeyword:
			case SyntaxKind.ConstTokenKeyword:
			case SyntaxKind.ContinueTokenKeyword:
			case SyntaxKind.DecimalTokenKeyword:
			case SyntaxKind.DefaultTokenKeyword:
			case SyntaxKind.DelegateTokenKeyword:
			case SyntaxKind.DoTokenKeyword:
			case SyntaxKind.DoubleTokenKeyword:
			case SyntaxKind.ElseTokenKeyword:
			case SyntaxKind.EnumTokenKeyword:
			case SyntaxKind.EventTokenKeyword:
			case SyntaxKind.ExplicitTokenKeyword:
			case SyntaxKind.ExternTokenKeyword:
			case SyntaxKind.FalseTokenKeyword:
			case SyntaxKind.FinallyTokenKeyword:
			case SyntaxKind.FixedTokenKeyword:
			case SyntaxKind.FloatTokenKeyword:
			case SyntaxKind.ForTokenKeyword:
			case SyntaxKind.ForeachTokenKeyword:
			case SyntaxKind.GotoTokenKeyword:
			case SyntaxKind.IfTokenKeyword:
			case SyntaxKind.ImplicitTokenKeyword:
			case SyntaxKind.InTokenKeyword:
			case SyntaxKind.IntTokenKeyword:
			case SyntaxKind.InterfaceTokenKeyword:
			case SyntaxKind.InternalTokenKeyword:
			case SyntaxKind.IsTokenKeyword:
			case SyntaxKind.LockTokenKeyword:
			case SyntaxKind.LongTokenKeyword:
			case SyntaxKind.NamespaceTokenKeyword:
			case SyntaxKind.NewTokenKeyword:
			case SyntaxKind.NullTokenKeyword:
			case SyntaxKind.ObjectTokenKeyword:
			case SyntaxKind.OperatorTokenKeyword:
			case SyntaxKind.OutTokenKeyword:
			case SyntaxKind.OverrideTokenKeyword:
			case SyntaxKind.ParamsTokenKeyword:
			case SyntaxKind.PrivateTokenKeyword:
			case SyntaxKind.ProtectedTokenKeyword:
			case SyntaxKind.PublicTokenKeyword:
			case SyntaxKind.ReadonlyTokenKeyword:
			case SyntaxKind.RefTokenKeyword:
			case SyntaxKind.ReturnTokenKeyword:
			case SyntaxKind.SbyteTokenKeyword:
			case SyntaxKind.SealedTokenKeyword:
			case SyntaxKind.ShortTokenKeyword:
			case SyntaxKind.SizeofTokenKeyword:
			case SyntaxKind.StackallocTokenKeyword:
			case SyntaxKind.StaticTokenKeyword:
			case SyntaxKind.StringTokenKeyword:
			case SyntaxKind.StructTokenKeyword:
			case SyntaxKind.SwitchTokenKeyword:
			case SyntaxKind.ThisTokenKeyword:
			case SyntaxKind.ThrowTokenKeyword:
			case SyntaxKind.TrueTokenKeyword:
			case SyntaxKind.TryTokenKeyword:
			case SyntaxKind.TypeofTokenKeyword:
			case SyntaxKind.UintTokenKeyword:
			case SyntaxKind.UlongTokenKeyword:
			case SyntaxKind.UncheckedTokenKeyword:
			case SyntaxKind.UnsafeTokenKeyword:
			case SyntaxKind.UshortTokenKeyword:
			case SyntaxKind.UsingTokenKeyword:
			case SyntaxKind.VirtualTokenKeyword:
			case SyntaxKind.VoidTokenKeyword:
			case SyntaxKind.VolatileTokenKeyword:
			case SyntaxKind.WhileTokenKeyword:
				return true;
			default:
				return false;
		}
    }

    /// <summary>
    /// The keywords: 'string', 'bool' 'int' and etc... are keywords, but identify a type.
    /// </summary>
    public static bool IsTypeIdentifierKeywordSyntaxKind(SyntaxKind syntaxKind)
    {
        switch (syntaxKind)
        {
            case SyntaxKind.BoolTokenKeyword:
            case SyntaxKind.ByteTokenKeyword:
            case SyntaxKind.CharTokenKeyword:
            case SyntaxKind.DecimalTokenKeyword:
            case SyntaxKind.DelegateTokenKeyword:
            case SyntaxKind.DoubleTokenKeyword:
            case SyntaxKind.EnumTokenKeyword:
            case SyntaxKind.FloatTokenKeyword:
            case SyntaxKind.IntTokenKeyword:
            case SyntaxKind.LongTokenKeyword:
            case SyntaxKind.NullTokenKeyword:
            case SyntaxKind.ObjectTokenKeyword:
            case SyntaxKind.SbyteTokenKeyword:
            case SyntaxKind.ShortTokenKeyword:
            case SyntaxKind.StringTokenKeyword:
            case SyntaxKind.UintTokenKeyword:
            case SyntaxKind.UlongTokenKeyword:
            case SyntaxKind.UshortTokenKeyword:
            case SyntaxKind.VoidTokenKeyword:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// TODO: What is described in this summary has not yet been implemented.
    /// The text 'var' can be an identifier to any syntax.
    /// For example, 'var' can be the name of a class, a variable, etc...
    /// Therefore, this method will check if the <see cref="SyntaxKind"/> is
    /// <see cref="SyntaxKind.VarTokenContextualKeyword"/> AND check if
    /// any 'var' identified definitions are in scope.
    /// </summary>
    public static bool IsVarContextualKeyword(CSharpCompilationUnit compilationUnit, SyntaxKind syntaxKind)
    {
        if (syntaxKind != SyntaxKind.VarTokenContextualKeyword)
            return false;

        return true;
    }

    public static bool IsBinaryOperatorSyntaxKind(SyntaxKind syntaxKind)
    {
        switch (syntaxKind)
        {
            case SyntaxKind.PlusToken:
            case SyntaxKind.MinusToken:
            case SyntaxKind.StarToken:
            case SyntaxKind.DivisionToken:
            case SyntaxKind.MemberAccessToken:
            case SyntaxKind.CloseAngleBracketToken:
            case SyntaxKind.CloseAngleBracketEqualsToken:
            case SyntaxKind.OpenAngleBracketToken:
            case SyntaxKind.OpenAngleBracketEqualsToken:
            case SyntaxKind.BangEqualsToken:
            case SyntaxKind.EqualsToken:
            case SyntaxKind.EqualsEqualsToken:
            case SyntaxKind.AmpersandToken:
            case SyntaxKind.AmpersandAmpersandToken:
            case SyntaxKind.PipeToken:
            case SyntaxKind.PipePipeToken:
            case SyntaxKind.QuestionMarkQuestionMarkToken:
                return true;
            default:
                return false;
        }
    }

    public static bool IsUnaryOperatorSyntaxKind(SyntaxKind syntaxKind)
    {
        switch (syntaxKind)
        {
            case SyntaxKind.PlusPlusToken:
            case SyntaxKind.MinusMinusToken:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// public, private, protected, internal, ....
    /// </summary>
    public static bool IsAccessibilitySyntaxKind(SyntaxKind syntaxKind)
    {
        switch (syntaxKind)
        {
            case SyntaxKind.PublicTokenKeyword:
            case SyntaxKind.PrivateTokenKeyword:
            case SyntaxKind.ProtectedTokenKeyword:
            case SyntaxKind.InternalTokenKeyword:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// The value '0' is returned if the provided <see cref="SyntaxKind"/> was not an operator.
    /// </summary>
    public static int GetOperatorPrecedence(SyntaxKind syntaxKind)
    {
        switch (syntaxKind)
        {
            case SyntaxKind.PlusToken:
            case SyntaxKind.MinusToken:
                return 1;
            case SyntaxKind.StarToken:
            case SyntaxKind.DivisionToken:
                return 2;
            case SyntaxKind.ParenthesizedExpressionNode:
                return 3;
            default:
                return 0;
        }
    }

    /// <summary>
    /// If the provided <see cref="KeywordToken"/> does not map to a <see cref="StorageModifierKind"/>,
    /// then null is returned.
    /// </summary>
    public static StorageModifierKind GetStorageModifierKindFromToken(SyntaxToken consumedToken)
    {
        switch (consumedToken.TextSpan.GetText())
        {
            case "enum":
                return StorageModifierKind.Enum;
            case "struct":
                return StorageModifierKind.Struct;
            case "class":
                return StorageModifierKind.Class;
            case "interface":
                return StorageModifierKind.Interface;
            case "record":
                return StorageModifierKind.Record;
            default:
                return StorageModifierKind.None;
        }
    }
    
    public static AccessModifierKind GetAccessModifierKindFromToken(SyntaxToken consumedToken)
    {
        switch (consumedToken.TextSpan.GetText())
        {
            case "public":
                return AccessModifierKind.Public;
            case "protected":
                return AccessModifierKind.Protected;
            case "internal":
                return AccessModifierKind.Internal;
            case "private":
                return AccessModifierKind.Private;
            default:
                return AccessModifierKind.None;
        }
    }
    
    public static bool IsConvertibleToTypeClauseNode(SyntaxKind syntaxKind)
    {
    	if (syntaxKind == SyntaxKind.TypeClauseNode ||
    		syntaxKind == SyntaxKind.IdentifierToken ||
    		IsTypeIdentifierKeywordSyntaxKind(syntaxKind))
    	{
    		return true;
    	}
    	
    	// TODO: Perhaps for 'async' and 'await' keywords, a check for a variable declaration with the name...
    	// ...'async' or 'await' is necessary.
    	if (IsContextualKeywordSyntaxKind(syntaxKind) &&
    		syntaxKind != SyntaxKind.AsyncTokenContextualKeyword &&
    		syntaxKind != SyntaxKind.AwaitTokenContextualKeyword)
    	{
    		return true;
    	}
    	
    	return false;
    }
    
    public static TypeClauseNode ConvertTokenToTypeClauseNode(ref SyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	if (token.SyntaxKind == SyntaxKind.IdentifierToken)
    	{
    		return parserModel.ConstructOrRecycleTypeClauseNode(
	    		token,
		        valueType: null,
		        genericParameterListing: default,
		        isKeywordType: false);
    	}
	    else if (IsTypeIdentifierKeywordSyntaxKind(token.SyntaxKind))
	    {
	    	return parserModel.ConstructOrRecycleTypeClauseNode(
	    		token,
		        valueType: null,
		        genericParameterListing: default,
		        isKeywordType: true);
	    }
	    else if (IsContextualKeywordSyntaxKind(token.SyntaxKind))
	    {
	    	return parserModel.ConstructOrRecycleTypeClauseNode(
	    		token,
		        valueType: null,
		        genericParameterListing: default,
		        isKeywordType: true);
	    }
	    else
	    {
	    	// 'parserModel.TokenWalker.Current.TextSpan' isn't necessarily the syntax passed to this method.
	    	// TODO: But getting a TextSpan from a general type such as 'ISyntax' is a pain.
	    	//
	    	/*compilationUnit.DiagnosticBag.ReportTodoException(
	    		parserModel.TokenWalker.Current.TextSpan,
	    		$"The {nameof(SyntaxKind)}: {syntax.SyntaxKind}, is not convertible to a {nameof(TypeClauseNode)}. Invoke {nameof(IsConvertibleToTypeClauseNode)} and check the result, before invoking {nameof(ConvertToTypeClauseNode)}.");*/
	    	
	    	// TODO: Returning null when it can't be converted is a bad idea (the method return isn't documented as nullable).
	    	return null;
	    }
    }
    
    public static TypeClauseNode ConvertNodeToTypeClauseNode(ISyntaxNode node, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	if (node.SyntaxKind == SyntaxKind.TypeClauseNode)
    	{
    		return (TypeClauseNode)node;
    	}
	    else
	    {
	    	// 'parserModel.TokenWalker.Current.TextSpan' isn't necessarily the syntax passed to this method.
	    	// TODO: But getting a TextSpan from a general type such as 'ISyntax' is a pain.
	    	//
	    	/*compilationUnit.DiagnosticBag.ReportTodoException(
	    		parserModel.TokenWalker.Current.TextSpan,
	    		$"The {nameof(SyntaxKind)}: {syntax.SyntaxKind}, is not convertible to a {nameof(TypeClauseNode)}. Invoke {nameof(IsConvertibleToTypeClauseNode)} and check the result, before invoking {nameof(ConvertToTypeClauseNode)}.");*/
	    	
	    	// TODO: Returning null when it can't be converted is a bad idea (the method return isn't documented as nullable).
	    	return null;
	    }
    }
    
    public static bool IsConvertibleToIdentifierToken(SyntaxKind syntaxKind)
    {
    	return syntaxKind == SyntaxKind.IdentifierToken ||
    		   IsContextualKeywordSyntaxKind(syntaxKind);
    }
    
    /// <summary>
    /// I added 'ref' to the 'SyntaxToken token' but a lot of the code
    /// actually was passing a member of a variable.
    ///
    /// So I had to copy the struct anyhow in order to make a variable that could be used as the 'ref'.
    /// I'm just bleh right now idk.
    /// </summary>
    public static SyntaxToken ConvertToIdentifierToken(ref SyntaxToken token, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	if (token.SyntaxKind == SyntaxKind.IdentifierToken)
    	{
    		return (SyntaxToken)token;
    	}
	    else if (IsContextualKeywordSyntaxKind(token.SyntaxKind))
	    {
	    	var keywordContextualToken = (SyntaxToken)token;
	    	return new SyntaxToken(SyntaxKind.IdentifierToken, keywordContextualToken.TextSpan);
	    }
	    else
	    {
	    	// 'parserModel.TokenWalker.Current.TextSpan' isn't necessarily the syntax passed to this method.
	    	// TODO: But getting a TextSpan from a general type such as 'ISyntax' is a pain.
	    	//
	    	/*compilationUnit.DiagnosticBag.ReportTodoException(
	    		parserModel.TokenWalker.Current.TextSpan,
	    		$"The {nameof(SyntaxKind)}: {syntax.SyntaxKind}, is not convertible to a {nameof(SyntaxKind.IdentifierToken)}. Invoke {nameof(IsConvertibleToIdentifierToken)} and check the result, before invoking {nameof(ConvertToIdentifierToken)}.");*/
	    		
	    	// TODO: Returning default when it can't be converted might be a fine idea? It isn't as bad as returning null.
	    	return default;
	    }
    }
}