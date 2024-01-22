using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.CSharp.ParserCase.Internals;

public static class ParseTypes
{
    public static void HandleStaticClassIdentifier(ParserModel model)
    {
        var identifierToken = (IdentifierToken)model.SyntaxStack.Pop();

        // The identifier token was already consumed, so a Backtrack() is needed.
        model.TokenWalker.Backtrack();
        model.SyntaxStack.Push(MatchTypeClause(model));
    }

    public static void HandleUndefinedTypeOrNamespaceReference(ParserModel model)
    {
        var identifierToken = (IdentifierToken)model.SyntaxStack.Pop();
        var identifierReferenceNode = new AmbiguousIdentifierNode(identifierToken);

        model.Binder.BindTypeIdentifier(identifierToken);

        model.DiagnosticBag.ReportUndefinedTypeOrNamespace(
            identifierToken.TextSpan,
            identifierToken.TextSpan.GetText());

        model.SyntaxStack.Push(identifierReferenceNode);
    }

    public static void HandleTypeReference(ParserModel model)
    {
        var typeDefinitionNode = (TypeDefinitionNode)model.SyntaxStack.Pop();

        var memberIdentifierToken = (IdentifierToken)model.SyntaxStack.Pop();
        model.Binder.BindTypeIdentifier(memberIdentifierToken);

        var memberAccessToken = (MemberAccessToken)model.TokenWalker.Match(SyntaxKind.MemberAccessToken);

        if (memberAccessToken.IsFabricated)
            throw new NotImplementedException("Implement a static class being member accessed, but the statement ends there --- it is incomplete.");

        var identifierToken = (IdentifierToken)model.TokenWalker.Match(SyntaxKind.IdentifierToken);

        if (identifierToken.IsFabricated)
            throw new NotImplementedException("Implement a static class being member accessed, but the statement ends there --- it is incomplete.");

        var matchingFunctionDefinitionNodes = typeDefinitionNode
            .GetFunctionDefinitionNodes()
            .Where(fd =>
                fd.FunctionIdentifierToken.TextSpan.GetText() == identifierToken.TextSpan.GetText())
            .ToImmutableArray();

        model.SyntaxStack.Push(identifierToken);

        ParseFunctions.HandleFunctionReferences(matchingFunctionDefinitionNodes, model);
    }

    /// <summary>
    /// This method is used for generic type usage such as, 'var words = new List&lt;string&gt;;'
    /// </summary>
    public static void HandleGenericParameters(ParserModel model)
    {
        var openAngleBracketToken = (OpenAngleBracketToken)model.SyntaxStack.Pop();

        if (SyntaxKind.CloseAngleBracketToken == model.TokenWalker.Current.SyntaxKind)
        {
            model.SyntaxStack.Push(new GenericParametersListingNode(
                openAngleBracketToken,
                ImmutableArray<GenericParameterEntryNode>.Empty,
                (CloseAngleBracketToken)model.TokenWalker.Consume()));

            return;
        }

        var mutableGenericParametersListing = new List<GenericParameterEntryNode>();

        while (true)
        {
            // TypeClause
            var typeClauseNode = MatchTypeClause(model);

            if (typeClauseNode.IsFabricated)
                break;

            var genericParameterEntryNode = new GenericParameterEntryNode(typeClauseNode);
            mutableGenericParametersListing.Add(genericParameterEntryNode);

            if (SyntaxKind.CommaToken == model.TokenWalker.Current.SyntaxKind)
            {
                var commaToken = (CommaToken)model.TokenWalker.Consume();

                // TODO: Track comma tokens?
                //
                // functionArgumentListing.Add(commaToken);
            }
            else
            {
                break;
            }
        }

        var closeAngleBracketToken = (CloseAngleBracketToken)model.TokenWalker.Match(SyntaxKind.CloseAngleBracketToken);

        model.SyntaxStack.Push(new GenericParametersListingNode(
            openAngleBracketToken,
            mutableGenericParametersListing.ToImmutableArray(),
            closeAngleBracketToken));
    }

    /// <summary>
    /// This method is used for generic type definition such as, 'class List&lt;T&gt; { ... }'
    /// </summary>
    public static void HandleGenericArguments(ParserModel model)
    {
        var openAngleBracketToken = (OpenAngleBracketToken)model.SyntaxStack.Pop();

        if (SyntaxKind.CloseAngleBracketToken == model.TokenWalker.Current.SyntaxKind)
        {
            model.SyntaxStack.Push(new GenericArgumentsListingNode(
                openAngleBracketToken,
                ImmutableArray<GenericArgumentEntryNode>.Empty,
                (CloseAngleBracketToken)model.TokenWalker.Consume()));

            return;
        }

        var mutableGenericArgumentsListing = new List<GenericArgumentEntryNode>();

        while (true)
        {
            // TypeClause
            var typeClauseNode = MatchTypeClause(model);

            if (typeClauseNode.IsFabricated)
                break;

            var genericArgumentEntryNode = new GenericArgumentEntryNode(typeClauseNode);
            mutableGenericArgumentsListing.Add(genericArgumentEntryNode);

            if (SyntaxKind.CommaToken == model.TokenWalker.Current.SyntaxKind)
            {
                var commaToken = (CommaToken)model.TokenWalker.Consume();

                // TODO: Track comma tokens?
                //
                // functionArgumentListing.Add(commaToken);
            }
            else
            {
                break;
            }
        }

        var closeAngleBracketToken = (CloseAngleBracketToken)model.TokenWalker.Match(SyntaxKind.CloseAngleBracketToken);

        model.SyntaxStack.Push(new GenericArgumentsListingNode(
            openAngleBracketToken,
            mutableGenericArgumentsListing.ToImmutableArray(),
            closeAngleBracketToken));
    }

    /// <summary>TODO: Correctly implement this method. For now going to skip until the attribute closing square bracket.</summary>
    public static void HandleAttribute(ParserModel model)
    {
        var openSquareBracketToken = (OpenSquareBracketToken)model.SyntaxStack.Pop();

        ISyntaxToken tokenCurrent;
        var innerTokens = new List<ISyntaxToken>();

        while (true)
        {
            tokenCurrent = model.TokenWalker.Consume();

            if (tokenCurrent.SyntaxKind == SyntaxKind.EndOfFileToken ||
                tokenCurrent.SyntaxKind == SyntaxKind.CloseSquareBracketToken)
            {
                break;
            }
            else
            {
                innerTokens.Add(tokenCurrent);
            }
        }

        if (tokenCurrent.SyntaxKind == SyntaxKind.CloseSquareBracketToken)
        {
            model.SyntaxStack.Push(model.Binder.BindAttributeNode(
                openSquareBracketToken,
                innerTokens,
                (CloseSquareBracketToken)tokenCurrent));
        }
    }

    public static TypeClauseNode MatchTypeClause(ParserModel model)
    {
        ISyntaxToken syntaxToken;

        if (UtilityApi.IsKeywordSyntaxKind(model.TokenWalker.Current.SyntaxKind) &&
                (UtilityApi.IsTypeIdentifierKeywordSyntaxKind(model.TokenWalker.Current.SyntaxKind) ||
                UtilityApi.IsVarContextualKeyword(model, model.TokenWalker.Current.SyntaxKind)))
        {
            syntaxToken = model.TokenWalker.Consume();
        }
        else
        {
            syntaxToken = model.TokenWalker.Match(SyntaxKind.IdentifierToken);
        }

        var typeClauseNode = new TypeClauseNode(
            syntaxToken,
            null,
            null);

        typeClauseNode = model.Binder.BindTypeClauseNode(typeClauseNode);

        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
        {
            var openAngleBracketToken = (OpenAngleBracketToken)model.TokenWalker.Consume();

            model.SyntaxStack.Push(openAngleBracketToken);
            ParseTypes.HandleGenericParameters(model);

            var genericParametersListingNode = (GenericParametersListingNode)model.SyntaxStack.Pop();

            typeClauseNode = new TypeClauseNode(
                typeClauseNode.TypeIdentifier,
                null,
                genericParametersListingNode);
        }

        while (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenSquareBracketToken)
        {
            var openSquareBracketToken = model.TokenWalker.Consume();
            var closeSquareBracketToken = model.TokenWalker.Match(SyntaxKind.CloseSquareBracketToken);

            var arraySyntaxTokenTextSpan = syntaxToken.TextSpan with
            {
                EndingIndexExclusive = closeSquareBracketToken.TextSpan.EndingIndexExclusive
            };

            var arraySyntaxToken = new ArraySyntaxToken(arraySyntaxTokenTextSpan);
            var genericParameterEntryNode = new GenericParameterEntryNode(typeClauseNode);

            var genericParametersListingNode = new GenericParametersListingNode(
                new OpenAngleBracketToken(openSquareBracketToken.TextSpan)
                {
                    IsFabricated = true
                },
                new GenericParameterEntryNode[] { genericParameterEntryNode }.ToImmutableArray(),
                new CloseAngleBracketToken(closeSquareBracketToken.TextSpan)
                {
                    IsFabricated = true
                });

            return new TypeClauseNode(
                arraySyntaxToken,
                null,
                genericParametersListingNode);

            // TODO: Implement multidimensional arrays. This array logic always returns after finding the first array syntax.
        }

        return typeClauseNode;
    }

    /// <summary>TODO: Correctly parse object initialization. For now, just skip over it when parsing.</summary>
    public static void HandleObjectInitialization(ParserModel model)
    {
        var openBraceToken = (OpenBraceToken)model.SyntaxStack.Pop();
        ISyntaxToken shouldBeCloseBraceToken = new BadToken(openBraceToken.TextSpan);

        while (!model.TokenWalker.IsEof)
        {
            shouldBeCloseBraceToken = model.TokenWalker.Consume();

            if (SyntaxKind.EndOfFileToken == shouldBeCloseBraceToken.SyntaxKind ||
                SyntaxKind.CloseBraceToken == shouldBeCloseBraceToken.SyntaxKind)
            {
                break;
            }
        }

        if (SyntaxKind.CloseBraceToken != shouldBeCloseBraceToken.SyntaxKind)
            shouldBeCloseBraceToken = model.TokenWalker.Match(SyntaxKind.CloseBraceToken);

        model.SyntaxStack.Push(new ObjectInitializationNode(
            openBraceToken,
            (CloseBraceToken)shouldBeCloseBraceToken));
    }
}
