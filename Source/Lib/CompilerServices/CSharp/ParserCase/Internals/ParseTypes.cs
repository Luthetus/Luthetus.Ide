using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.CompilerServices.CSharp.ParserCase.Internals;

public static class ParseTypes
{
    public static void HandleStaticClassIdentifier(
        IdentifierToken consumedIdentifierToken,
        CSharpParserModel model)
    {
        // The identifier token was already consumed, so a Backtrack() is needed.
        model.TokenWalker.Backtrack();
        model.SyntaxStack.Push(MatchTypeClause(model));
    }

    public static void HandleUndefinedTypeOrNamespaceReference(
        IdentifierToken consumedIdentifierToken,
        CSharpParserModel model)
    {
        var identifierReferenceNode = new AmbiguousIdentifierNode(consumedIdentifierToken);

        model.Binder.BindTypeIdentifier(consumedIdentifierToken, model);

        model.DiagnosticBag.ReportUndefinedTypeOrNamespace(
            consumedIdentifierToken.TextSpan,
            consumedIdentifierToken.TextSpan.GetText());

        model.SyntaxStack.Push(identifierReferenceNode);
    }

    public static void HandleTypeReference(
        IdentifierToken consumedIdentifierToken,
        TypeDefinitionNode consumedTypeDefinitionNode,
        CSharpParserModel model)
    {
        model.Binder.BindTypeIdentifier(consumedIdentifierToken, model);

        var memberAccessToken = (MemberAccessToken)model.TokenWalker.Match(SyntaxKind.MemberAccessToken);

        if (memberAccessToken.IsFabricated)
            throw new NotImplementedException("Implement a static class being member accessed, but the statement ends there --- it is incomplete.");

        var identifierToken = (IdentifierToken)model.TokenWalker.Match(SyntaxKind.IdentifierToken);

        if (identifierToken.IsFabricated)
            throw new NotImplementedException("Implement a static class being member accessed, but the statement ends there --- it is incomplete.");

        var matchingFunctionDefinitionNodes = consumedTypeDefinitionNode
            .GetFunctionDefinitionNodes()
            .Where(fd =>
                fd.FunctionIdentifierToken.TextSpan.GetText() == identifierToken.TextSpan.GetText())
            .ToImmutableArray();

        ParseFunctions.HandleFunctionReferences(identifierToken, matchingFunctionDefinitionNodes, model);
    }

    /// <summary>
    /// This method is used for generic type usage such as, 'var words = new List&lt;string&gt;;'
    /// </summary>
    public static void HandleGenericParameters(
        OpenAngleBracketToken consumedOpenAngleBracketToken,
        CSharpParserModel model)
    {
        if (SyntaxKind.CloseAngleBracketToken == model.TokenWalker.Current.SyntaxKind)
        {
            model.SyntaxStack.Push(new GenericParametersListingNode(
                consumedOpenAngleBracketToken,
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
            consumedOpenAngleBracketToken,
            mutableGenericParametersListing.ToImmutableArray(),
            closeAngleBracketToken));
    }

    /// <summary>
    /// This method is used for generic type definition such as, 'class List&lt;T&gt; { ... }'
    /// </summary>
    public static void HandleGenericArguments(
        OpenAngleBracketToken consumedOpenAngleBracketToken,
        CSharpParserModel model)
    {
        if (SyntaxKind.CloseAngleBracketToken == model.TokenWalker.Current.SyntaxKind)
        {
            model.SyntaxStack.Push(new GenericArgumentsListingNode(
                consumedOpenAngleBracketToken,
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
            consumedOpenAngleBracketToken,
            mutableGenericArgumentsListing.ToImmutableArray(),
            closeAngleBracketToken));
    }

    public static void HandleAttribute(
        OpenSquareBracketToken consumedOpenSquareBracketToken,
        CSharpParserModel model)
    {
        // Suppress unused variable warning
        _ = consumedOpenSquareBracketToken;

        if (SyntaxKind.CloseSquareBracketToken == model.TokenWalker.Current.SyntaxKind)
        {
            var closeSquareBracketToken = (CloseSquareBracketToken)model.TokenWalker.Consume();

            model.DiagnosticBag.ReportTodoException(
                closeSquareBracketToken.TextSpan,
                "An identifier was expected.");

            return;
        }

        while (true)
        {
            var identifierToken = (IdentifierToken)model.TokenWalker.Match(SyntaxKind.IdentifierToken);
            model.Binder.BindTypeIdentifier(identifierToken, model);

            if (identifierToken.IsFabricated && SyntaxKind.CommaToken != model.TokenWalker.Current.SyntaxKind)
                break;

            if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken)
            {
                var commaToken = (CommaToken)model.TokenWalker.Consume();
                // TODO: Track comma tokens?
            }
            else
            {
                break;
            }
        }
    }

    public static TypeClauseNode MatchTypeClause(CSharpParserModel model)
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

        typeClauseNode = model.Binder.BindTypeClauseNode(typeClauseNode, model);

        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
        {
            var openAngleBracketToken = (OpenAngleBracketToken)model.TokenWalker.Consume();

            model.SyntaxStack.Push(openAngleBracketToken);
            ParseTypes.HandleGenericParameters(openAngleBracketToken, model);

            var genericParametersListingNode = (GenericParametersListingNode)model.SyntaxStack.Pop();

            typeClauseNode = new TypeClauseNode(
                typeClauseNode.TypeIdentifierToken,
                null,
                genericParametersListingNode);
        }
        
        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.QuestionMarkToken)
        {
        	typeClauseNode.HasQuestionMark = true;
        	_ = model.TokenWalker.Consume();
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
    public static void HandleObjectInitialization(
        OpenBraceToken consumedOpenBraceToken,
        CSharpParserModel model)
    {
        ISyntaxToken shouldBeCloseBraceToken = new BadToken(consumedOpenBraceToken.TextSpan);

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
            consumedOpenBraceToken,
            (CloseBraceToken)shouldBeCloseBraceToken));
    }

    public static void HandlePrimaryConstructorDefinition(
        TypeDefinitionNode typeDefinitionNode,
        OpenParenthesisToken consumedOpenParenthesisToken,
        CSharpParserModel model)
    {
        ParseFunctions.HandleFunctionArguments(consumedOpenParenthesisToken, model);
        var functionArgumentsListingNode = (FunctionArgumentsListingNode)model.SyntaxStack.Pop();

        typeDefinitionNode = new TypeDefinitionNode(
            typeDefinitionNode.AccessModifierKind,
            typeDefinitionNode.HasPartialModifier,
            typeDefinitionNode.StorageModifierKind,
            typeDefinitionNode.TypeIdentifierToken,
            typeDefinitionNode.ValueType,
            typeDefinitionNode.GenericArgumentsListingNode,
            functionArgumentsListingNode,
            typeDefinitionNode.InheritedTypeClauseNode,
            typeDefinitionNode.OpenBraceToken,
            typeDefinitionNode.TypeBodyCodeBlockNode);

        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
            model.SyntaxStack.Push(typeDefinitionNode);
        else
            model.CurrentCodeBlockBuilder.ChildList.Add(typeDefinitionNode);
    }
}
