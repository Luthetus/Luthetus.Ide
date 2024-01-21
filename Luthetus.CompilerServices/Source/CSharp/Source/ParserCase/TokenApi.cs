using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Enums;

namespace Luthetus.CompilerServices.Lang.CSharp.ParserCase;

public static class TokenApi
{
    public static void ParseNumericLiteralToken(ParserModel model)
    {
        var numericLiteralToken = (NumericLiteralToken)model.SyntaxStack.Pop();

        model.TokenWalker.Backtrack();

        SyntaxApi.HandleExpression(
            null,
            null,
            null,
            null,
            null,
            null,
            model);

        var completeExpression = (IExpressionNode)model.SyntaxStack.Pop();

        model.CurrentCodeBlockBuilder.ChildList.Add(completeExpression);
    }

    public static void ParseStringLiteralToken(ParserModel model)
    {
        var stringLiteralToken = (StringLiteralToken)model.SyntaxStack.Pop();

        model.TokenWalker.Backtrack();

        SyntaxApi.HandleExpression(
            null,
            null,
            null,
            null,
            null,
            null,
            model);

        var completeExpression = (IExpressionNode)model.SyntaxStack.Pop();

        model.CurrentCodeBlockBuilder.ChildList.Add(completeExpression);
    }

    public static void ParsePreprocessorDirectiveToken(ParserModel model)
    {
        var preprocessorDirectiveToken = (PreprocessorDirectiveToken)model.SyntaxStack.Pop();

        var consumedToken = model.TokenWalker.Consume();

        if (consumedToken.SyntaxKind == SyntaxKind.LibraryReferenceToken)
        {
            var preprocessorLibraryReferenceStatement = new PreprocessorLibraryReferenceStatementNode(
                preprocessorDirectiveToken,
                consumedToken);

            model.CurrentCodeBlockBuilder.ChildList.Add(preprocessorLibraryReferenceStatement);

            return;
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public static void ParseIdentifierToken(ParserModel model)
    {
        var identifierToken = (IdentifierToken)model.SyntaxStack.Pop();

        if (model.SyntaxStack.TryPeek(out var syntax) && syntax is AmbiguousIdentifierNode)
            ResolveAmbiguousIdentifier(model);

        model.SyntaxStack.Push(identifierToken);

        if (TryParseTypedIdentifier(model))
            return;

        if (TryParseConstructorDefinition(model))
            return;

        if (TryParseGenericTypeOrFunctionInvocation(model))
            return;

        if (TryParseVariableAssignment(model))
            return;

        if (TryParseReference(model))
            return;

        return;
    }

    private static bool TryParseGenericArguments(ParserModel model)
    {
        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
        {
            model.SyntaxStack.Push(model.TokenWalker.Consume());

            SyntaxApi.HandleGenericArguments(model);
            return true;
        }
        else
        {
            return false;
        }
    }

    private static bool TryParseGenericParameters(
        ParserModel model,
        out GenericParametersListingNode? genericParametersListingNode)
    {
        if (SyntaxKind.OpenAngleBracketToken == model.TokenWalker.Current.SyntaxKind)
        {
            model.SyntaxStack.Push(model.TokenWalker.Consume());

            SyntaxApi.HandleGenericParameters(model);
            genericParametersListingNode = (GenericParametersListingNode?)model.SyntaxStack.Pop();

            return true;
        }
        else
        {
            genericParametersListingNode = null;
            return false;
        }
    }

    private static bool TryParseConstructorDefinition(ParserModel model)
    {
        var identifierToken = (IdentifierToken)model.SyntaxStack.Pop();

        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken &&
            model.CurrentCodeBlockBuilder.CodeBlockOwner is not null &&
            model.CurrentCodeBlockBuilder.CodeBlockOwner.SyntaxKind == SyntaxKind.TypeDefinitionNode)
        {
            model.SyntaxStack.Push(identifierToken);

            SyntaxApi.HandleConstructorDefinition(model);

            return true;
        }

        model.SyntaxStack.Push(identifierToken);
        return false;
    }

    private static bool TryParseTypedIdentifier(ParserModel model)
    {
        var identifierToken = (IdentifierToken)model.SyntaxStack.Pop();

        if (model.SyntaxStack.TryPeek(out var syntax) && syntax is TypeClauseNode typeClauseNode)
        {
            model.SyntaxStack.Push(identifierToken);

            var genericArgumentsListingNode = (GenericArgumentsListingNode?)null;

            if (TryParseGenericArguments(model) &&
                model.SyntaxStack.Peek().SyntaxKind == SyntaxKind.GenericArgumentsListingNode)
            {
                genericArgumentsListingNode = (GenericArgumentsListingNode)model.SyntaxStack.Pop();
            }

            if (genericArgumentsListingNode is not null)
                model.SyntaxStack.Push(genericArgumentsListingNode);

            if (TryParseFunctionDefinition(model))
                return true;

            if (TryParseVariableDeclaration(model))
                return true;
        }

        model.SyntaxStack.Push(identifierToken);
        return false;
    }

    private static bool TryParseReference(ParserModel model)
    {
        var identifierToken = (IdentifierToken)model.SyntaxStack.Pop();

        var text = identifierToken.TextSpan.GetText();

        if (model.Binder.NamespaceGroupNodes.TryGetValue(text, out var namespaceGroupNode) &&
            namespaceGroupNode is not null)
        {
            model.SyntaxStack.Push(identifierToken);
            model.SyntaxStack.Push(namespaceGroupNode);

            SyntaxApi.HandleNamespaceReference(model);
            return true;
        }
        else
        {
            if (model.Binder.TryGetVariableDeclarationHierarchically(text, out var variableDeclarationStatementNode) &&
                variableDeclarationStatementNode is not null)
            {
                model.SyntaxStack.Push(identifierToken);
                model.SyntaxStack.Push(variableDeclarationStatementNode);

                SyntaxApi.HandleVariableReference(model);
                return true;
            }
            else
            {
                // 'undeclared-variable reference' OR 'static class identifier'

                if (model.Binder.TryGetTypeDefinitionHierarchically(text, out var typeDefinitionNode) &&
                    typeDefinitionNode is not null)
                {
                    model.SyntaxStack.Push(identifierToken);

                    SyntaxApi.HandleStaticClassIdentifier(model);
                    return true;
                }
                else
                {
                    model.SyntaxStack.Push(identifierToken);

                    SyntaxApi.HandleUndefinedTypeOrNamespaceReference(model);
                    return true;
                }
            }
        }
    }

    private static bool TryParseVariableAssignment(ParserModel model)
    {
        var identifierToken = (IdentifierToken)model.SyntaxStack.Pop();

        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
        {
            model.SyntaxStack.Push(identifierToken);

            SyntaxApi.HandleVariableAssignment(model);
            return true;
        }

        model.SyntaxStack.Push(identifierToken);
        return false;
    }

    private static bool TryParseGenericTypeOrFunctionInvocation(ParserModel model)
    {
        var identifierToken = (IdentifierToken)model.SyntaxStack.Pop();

        if (model.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenParenthesisToken &&
            model.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenAngleBracketToken)
        {
            model.SyntaxStack.Push(identifierToken);
            return false;
        }

        if (TryParseGenericParameters(model, out var genericParametersListingNode))
        {
            if (model.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenParenthesisToken)
            {
                // Generic type
                model.SyntaxStack.Push(new TypeClauseNode(
                    identifierToken,
                    null,
                    genericParametersListingNode));

                return true;
            }
        }

        // Function invocation
        model.SyntaxStack.Push(identifierToken);

        SyntaxApi.HandleFunctionInvocation(genericParametersListingNode, model);
        return true;
    }

    private static bool TryParseFunctionDefinition(ParserModel model)
    {
        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken)
        {
            SyntaxApi.HandleFunctionDefinition(model);
            return true;
        }

        return false;
    }

    private static bool TryParseVariableDeclaration(ParserModel model)
    {
        GenericArgumentsListingNode? genericArgumentsListingNode =
            model.SyntaxStack.Peek() is GenericArgumentsListingNode temporaryNode
                ? temporaryNode
                : null;

        var identifierToken = (IdentifierToken)model.SyntaxStack.Pop();
        var typeClauseNode = (TypeClauseNode)model.SyntaxStack.Pop();

        var isLocalOrField = model.TokenWalker.Current.SyntaxKind == SyntaxKind.StatementDelimiterToken ||
                             model.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken;

        var isLambda = model.TokenWalker.Next.SyntaxKind == SyntaxKind.CloseAngleBracketToken;

        model.SyntaxStack.Push(typeClauseNode);
        model.SyntaxStack.Push(identifierToken);

        if (genericArgumentsListingNode is not null)
            model.SyntaxStack.Push(genericArgumentsListingNode);

        var variableKind = (VariableKind?)null;

        if (isLocalOrField && !isLambda)
        {
            if (model.CurrentCodeBlockBuilder.CodeBlockOwner is TypeDefinitionNode)
                variableKind = VariableKind.Field;
            else
                variableKind = VariableKind.Local;
        }
        else if (isLambda)
        {
            // Property (expression bound)
            variableKind = VariableKind.Property;
        }
        else if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
        {
            // Property
            variableKind = VariableKind.Property;
        }

        if (variableKind is not null)
        {
            SyntaxApi.HandleVariableDeclaration(
                variableKind.Value,
                model);

            return true;
        }
        else
        {
            return false;
        }
    }

    public static void ResolveAmbiguousIdentifier(ParserModel model)
    {
        var identifierReferenceNode = (AmbiguousIdentifierNode)model.SyntaxStack.Pop();

        var expectingTypeClause = false;

        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
            expectingTypeClause = true;

        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken)
            expectingTypeClause = true;

        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken ||
            model.TokenWalker.Current.SyntaxKind == SyntaxKind.StatementDelimiterToken)
        {
            expectingTypeClause = true;
        }

        if (expectingTypeClause)
        {
            if (!model.Binder.TryGetTypeDefinitionHierarchically(
                    identifierReferenceNode.IdentifierToken.TextSpan.GetText(),
                    out var typeDefinitionNode)
                || typeDefinitionNode is null)
            {
                var fabricateTypeDefinition = new TypeDefinitionNode(
                    identifierReferenceNode.IdentifierToken,
                    null,
                    null,
                    null,
                    null)
                {
                    IsFabricated = true
                };

                model.Binder.BindTypeDefinitionNode(fabricateTypeDefinition);

                typeDefinitionNode = fabricateTypeDefinition;
            }

            model.SyntaxStack.Push(typeDefinitionNode.ToTypeClause());
        }
    }

    public static void ParsePlusToken(ParserModel model)
    {
        var plusToken = (PlusToken)model.SyntaxStack.Pop();
        var localNodeRecent = model.SyntaxStack.Pop();

        if (localNodeRecent is not IExpressionNode leftExpressionNode)
            throw new NotImplementedException();

        SyntaxApi.HandleExpression(
            null,
            null,
            null,
            null,
            null,
            null, 
            model);

        var rightExpressionNode = (IExpressionNode)model.SyntaxStack.Pop();

        var binaryOperatorNode = model.Binder.BindBinaryOperatorNode(
            leftExpressionNode,
            plusToken,
            rightExpressionNode);

        var binaryExpressionNode = new BinaryExpressionNode(
            leftExpressionNode,
            binaryOperatorNode,
            rightExpressionNode);

        model.SyntaxStack.Push(binaryExpressionNode);
    }

    public static void ParsePlusPlusToken(ParserModel model)
    {
        var plusPlusToken = (PlusPlusToken)model.SyntaxStack.Pop();

        if (model.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind == SyntaxKind.VariableReferenceNode)
        {
            var variableReferenceNode = (VariableReferenceNode)model.SyntaxStack.Pop();

            var unaryOperatorNode = new UnaryOperatorNode(
                variableReferenceNode.ResultTypeClauseNode,
                plusPlusToken,
                variableReferenceNode.ResultTypeClauseNode);

            var unaryExpressionNode = new UnaryExpressionNode(
                variableReferenceNode,
                unaryOperatorNode);

            model.CurrentCodeBlockBuilder.ChildList.Add(unaryExpressionNode);
        }
    }

    public static void ParseMinusToken(ParserModel model)
    {
        var minusToken = (MinusToken)model.SyntaxStack.Pop();

        SyntaxApi.HandleExpression(
            null,
            null,
            null,
            null,
            null,
            null, 
            model);
    }

    public static void ParseStarToken(ParserModel model)
    {
        var starToken = (StarToken)model.SyntaxStack.Pop();

        SyntaxApi.HandleExpression(
            null,
            null,
            null,
            null,
            null,
            null, 
            model);
    }

    public static void ParseDollarSignToken(ParserModel model)
    {
        var dollarSignToken = (DollarSignToken)model.SyntaxStack.Pop();

        model.TokenWalker.Backtrack();

        SyntaxApi.HandleExpression(
            null,
            null,
            null,
            null,
            null,
            null, 
            model);

        var completeExpression = (IExpressionNode)model.SyntaxStack.Pop();

        model.CurrentCodeBlockBuilder.ChildList.Add(completeExpression);
    }

    public static void ParseColonToken(ParserModel model)
    {
        var colonToken = (ColonToken)model.SyntaxStack.Pop();

        if (model.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind == SyntaxKind.TypeDefinitionNode)
        {
            var typeDefinitionNode = (TypeDefinitionNode)model.SyntaxStack.Pop();
            var inheritedTypeClauseNode = UtilityApi.MatchTypeClause(model);

            model.Binder.BindTypeClauseNode(inheritedTypeClauseNode);

            model.SyntaxStack.Push(new TypeDefinitionNode(
                typeDefinitionNode.TypeIdentifier,
                typeDefinitionNode.ValueType,
                typeDefinitionNode.GenericArgumentsListingNode,
                inheritedTypeClauseNode,
                typeDefinitionNode.TypeBodyCodeBlockNode));
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public static void ParseOpenBraceToken(ParserModel model)
    {
        var openBraceToken = (OpenBraceToken)model.SyntaxStack.Pop();

        var closureCurrentCodeBlockBuilder = model.CurrentCodeBlockBuilder;
        ISyntaxNode? nextCodeBlockOwner = null;
        TypeClauseNode? scopeReturnTypeClauseNode = null;

        if (model.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind == SyntaxKind.NamespaceStatementNode)
        {
            var namespaceStatementNode = (NamespaceStatementNode)model.SyntaxStack.Pop();
            nextCodeBlockOwner = namespaceStatementNode;

            model.FinalizeCodeBlockNodeActionStack.Push(codeBlockNode =>
            {
                namespaceStatementNode = new NamespaceStatementNode(
                    namespaceStatementNode.KeywordToken,
                    namespaceStatementNode.IdentifierToken,
                    codeBlockNode);

                closureCurrentCodeBlockBuilder.ChildList.Add(namespaceStatementNode);
                model.Binder.BindNamespaceStatementNode(namespaceStatementNode);
            });

            model.SyntaxStack.Push(namespaceStatementNode);
        }
        else if (model.SyntaxStack.TryPeek(out syntax) && syntax.SyntaxKind == SyntaxKind.TypeDefinitionNode)
        {
            var typeDefinitionNode = (TypeDefinitionNode)model.SyntaxStack.Pop();
            nextCodeBlockOwner = typeDefinitionNode;

            model.FinalizeCodeBlockNodeActionStack.Push(codeBlockNode =>
            {
                typeDefinitionNode = new TypeDefinitionNode(
                    typeDefinitionNode.TypeIdentifier,
                    typeDefinitionNode.ValueType,
                    typeDefinitionNode.GenericArgumentsListingNode,
                    typeDefinitionNode.InheritedTypeClauseNode,
                    codeBlockNode);

                model.Binder.BindTypeDefinitionNode(typeDefinitionNode, true);
                closureCurrentCodeBlockBuilder.ChildList.Add(typeDefinitionNode);
            });

            model.SyntaxStack.Push(typeDefinitionNode);
        }
        else if (model.SyntaxStack.TryPeek(out syntax) && syntax.SyntaxKind == SyntaxKind.FunctionDefinitionNode)
        {
            var functionDefinitionNode = (FunctionDefinitionNode)model.SyntaxStack.Pop();
            nextCodeBlockOwner = functionDefinitionNode;

            scopeReturnTypeClauseNode = functionDefinitionNode.ReturnTypeClauseNode;

            model.FinalizeCodeBlockNodeActionStack.Push(codeBlockNode =>
            {
                functionDefinitionNode = new FunctionDefinitionNode(
                    functionDefinitionNode.ReturnTypeClauseNode,
                    functionDefinitionNode.FunctionIdentifier,
                    functionDefinitionNode.GenericArgumentsListingNode,
                    functionDefinitionNode.FunctionArgumentsListingNode,
                    codeBlockNode,
                    functionDefinitionNode.ConstraintNode);

                closureCurrentCodeBlockBuilder.ChildList.Add(functionDefinitionNode);
            });

            model.SyntaxStack.Push(functionDefinitionNode);
        }
        else if (model.SyntaxStack.TryPeek(out syntax) && syntax.SyntaxKind == SyntaxKind.ConstructorDefinitionNode)
        {
            var constructorDefinitionNode = (ConstructorDefinitionNode)model.SyntaxStack.Pop();
            nextCodeBlockOwner = constructorDefinitionNode;

            scopeReturnTypeClauseNode = constructorDefinitionNode.ReturnTypeClauseNode;

            model.FinalizeCodeBlockNodeActionStack.Push(codeBlockNode =>
            {
                constructorDefinitionNode = new ConstructorDefinitionNode(
                    constructorDefinitionNode.ReturnTypeClauseNode,
                    constructorDefinitionNode.FunctionIdentifier,
                    constructorDefinitionNode.GenericArgumentsListingNode,
                    constructorDefinitionNode.FunctionArgumentsListingNode,
                    codeBlockNode,
                    constructorDefinitionNode.ConstraintNode);

                closureCurrentCodeBlockBuilder.ChildList.Add(constructorDefinitionNode);
            });

            model.SyntaxStack.Push(constructorDefinitionNode);
        }
        else if (model.SyntaxStack.TryPeek(out syntax) && syntax.SyntaxKind == SyntaxKind.IfStatementNode)
        {
            var ifStatementNode = (IfStatementNode)model.SyntaxStack.Pop();
            nextCodeBlockOwner = ifStatementNode;

            model.FinalizeCodeBlockNodeActionStack.Push(codeBlockNode =>
            {
                ifStatementNode = new IfStatementNode(
                    ifStatementNode.KeywordToken,
                    ifStatementNode.ExpressionNode,
                    codeBlockNode);

                closureCurrentCodeBlockBuilder.ChildList.Add(ifStatementNode);
            });

            model.SyntaxStack.Push(ifStatementNode);
        }
        else
        {
            nextCodeBlockOwner = closureCurrentCodeBlockBuilder.CodeBlockOwner;

            model.FinalizeCodeBlockNodeActionStack.Push(codeBlockNode =>
            {
                closureCurrentCodeBlockBuilder.ChildList.Add(codeBlockNode);
            });
        }

        model.Binder.RegisterBoundScope(
            scopeReturnTypeClauseNode,
            openBraceToken.TextSpan);

        if (model.SyntaxStack.TryPeek(out syntax) && syntax.SyntaxKind == SyntaxKind.NamespaceStatementNode)
        {
            var namespaceStatementNode = (NamespaceStatementNode)model.SyntaxStack.Pop();

            var namespaceString = namespaceStatementNode
                .IdentifierToken
                .TextSpan
                .GetText();

            model.Binder.AddNamespaceToCurrentScope(namespaceString);
            model.SyntaxStack.Push(namespaceStatementNode);
        }

        model.CurrentCodeBlockBuilder = new(model.CurrentCodeBlockBuilder, nextCodeBlockOwner);
    }

    public static void ParseCloseBraceToken(ParserModel model)
    {
        var closeBraceToken = (CloseBraceToken)model.SyntaxStack.Pop();

        model.Binder.DisposeBoundScope(closeBraceToken.TextSpan);

        if (model.CurrentCodeBlockBuilder.Parent is not null && model.FinalizeCodeBlockNodeActionStack.Any())
        {
            model.FinalizeCodeBlockNodeActionStack
                .Pop()
                .Invoke(model.CurrentCodeBlockBuilder.Build());

            model.CurrentCodeBlockBuilder = model.CurrentCodeBlockBuilder.Parent;
        }
    }

    public static void ParseOpenParenthesisToken(ParserModel model)
    {
        var openParenthesisToken = (OpenParenthesisToken)model.SyntaxStack.Pop();

        model.TokenWalker.Backtrack();

        SyntaxApi.HandleExpression(
            null,
            null,
            null,
            null,
            null,
            null,
            model);

        var parenthesizedExpression = (IExpressionNode)model.SyntaxStack.Pop();

        // Example: (3 + 4) * 3
        //
        // Complete expression would be binary multiplication.
        SyntaxApi.HandleExpression(
            parenthesizedExpression,
            parenthesizedExpression,
            null,
            null,
            null,
            null,
            model);

        var completeExpression = (IExpressionNode)model.SyntaxStack.Pop();

        model.CurrentCodeBlockBuilder.ChildList.Add(completeExpression);
    }

    public static void ParseCloseParenthesisToken(ParserModel model)
    {
        var closeParenthesisToken = (CloseParenthesisToken)model.SyntaxStack.Pop();
    }

    public static void ParseOpenAngleBracketToken(ParserModel model)
    {
        var openAngleBracketToken = (OpenAngleBracketToken)model.SyntaxStack.Pop();

        if (model.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind == SyntaxKind.LiteralExpressionNode ||
            model.SyntaxStack.TryPeek(out syntax) && syntax.SyntaxKind == SyntaxKind.LiteralExpressionNode ||
            model.SyntaxStack.TryPeek(out syntax) && syntax.SyntaxKind == SyntaxKind.BinaryExpressionNode ||
            /* Prefer the enum comparison. Will short circuit. This "is" cast is for fallback in case someone in the future adds for expression syntax kinds but does not update this if statement TODO: Check if node ends with "ExpressionNode"? */
            model.SyntaxStack.TryPeek(out syntax) && syntax is IExpressionNode)
        {
            // Mathematical angle bracket
            throw new NotImplementedException();
        }
        else
        {
            // Generic Arguments
            model.SyntaxStack.Push(openAngleBracketToken);

            SyntaxApi.HandleGenericArguments(model);

            if (model.SyntaxStack.TryPeek(out syntax) && syntax.SyntaxKind == SyntaxKind.TypeDefinitionNode)
            {
                var typeDefinitionNode = (TypeDefinitionNode)model.SyntaxStack.Pop();

                // TODO: Fix boundClassDefinitionNode, it broke on (2023-07-26)
                //
                // _cSharpParser._nodeRecent = boundClassDefinitionNode with
                // {
                //     BoundGenericArgumentsNode = boundGenericArguments
                // };
            }
        }
    }

    public static void ParseCloseAngleBracketToken(ParserModel model)
    {
        var closeAngleBracketToken = (CloseAngleBracketToken)model.SyntaxStack.Pop();
        // if one: throw new NotImplementedException();
        // then: lambdas will no longer work. So I'm keeping this method empty.
    }

    public static void ParseOpenSquareBracketToken(ParserModel model)
    {
        var openSquareBracketToken = (OpenSquareBracketToken)model.SyntaxStack.Pop();

        if (model.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind == SyntaxKind.LiteralExpressionNode ||
            model.SyntaxStack.TryPeek(out syntax) && syntax.SyntaxKind == SyntaxKind.LiteralExpressionNode ||
            model.SyntaxStack.TryPeek(out syntax) && syntax.SyntaxKind == SyntaxKind.BinaryExpressionNode ||
            /* Prefer the enum comparison. Will short circuit. This "is" cast is for fallback in case someone in the future adds for expression syntax kinds but does not update this if statement TODO: Check if node ends with "ExpressionNode"? */
            model.SyntaxStack.TryPeek(out syntax) && syntax is IExpressionNode)
        {
            // Mathematical square bracket
            throw new NotImplementedException();
        }
        else
        {
            // Attribute
            model.SyntaxStack.Push(openSquareBracketToken);
            SyntaxApi.HandleAttribute(model);
        }
    }

    public static void ParseCloseSquareBracketToken(ParserModel model)
    {
        var closeSquareBracketToken = (CloseSquareBracketToken)model.SyntaxStack.Pop();
    }

    public static void ParseMemberAccessToken(ParserModel model)
    {
        var memberAccessToken = (MemberAccessToken)model.SyntaxStack.Pop();

        if (model.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind == SyntaxKind.EmptyNode)
            throw new NotImplementedException($"_cSharpParser._handle.Handle the case where a {nameof(MemberAccessToken)} is used without a valid preceeding node.");

        switch (model.SyntaxStack.Peek().SyntaxKind)
        {
            case SyntaxKind.VariableReferenceNode:
                var variableReferenceNode = (VariableReferenceNode)model.SyntaxStack.Pop();

                if (variableReferenceNode.VariableDeclarationNode.IsFabricated)
                {
                    // Undeclared variable, so the Type is unknown.
                }

                break;
        }
    }

    public static void StatementDelimiterToken(ParserModel model)
    {
        var statementDelimiterToken = (StatementDelimiterToken)model.SyntaxStack.Pop();

        if (model.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind == SyntaxKind.NamespaceStatementNode)
        {
            var closureCurrentCompilationUnitBuilder = model.CurrentCodeBlockBuilder;
            ISyntaxNode? nextCodeBlockOwner = null;
            TypeClauseNode? scopeReturnTypeClauseNode = null;

            var namespaceStatementNode = (NamespaceStatementNode)model.SyntaxStack.Pop();
            nextCodeBlockOwner = namespaceStatementNode;

            model.FinalizeNamespaceFileScopeCodeBlockNodeAction = codeBlockNode =>
                {
                    namespaceStatementNode = new NamespaceStatementNode(
                        namespaceStatementNode.KeywordToken,
                        namespaceStatementNode.IdentifierToken,
                        codeBlockNode);

                    closureCurrentCompilationUnitBuilder.ChildList.Add(namespaceStatementNode);
                    model.Binder.BindNamespaceStatementNode(namespaceStatementNode);
                };

            model.Binder.RegisterBoundScope(
                scopeReturnTypeClauseNode,
                statementDelimiterToken.TextSpan);

            model.Binder.AddNamespaceToCurrentScope(
                namespaceStatementNode.IdentifierToken.TextSpan.GetText());

            model.CurrentCodeBlockBuilder = new(model.CurrentCodeBlockBuilder, nextCodeBlockOwner);
        }
    }

    public static void ParseKeywordToken(ParserModel model)
    {
        var keywordToken = model.SyntaxStack.Peek();

        // 'return', 'if', 'get', etc...
        switch (keywordToken.SyntaxKind)
        {
            case SyntaxKind.AsTokenKeyword:
                SyntaxApi.HandleAsTokenKeyword(model);
                break;
            case SyntaxKind.BaseTokenKeyword:
                SyntaxApi.HandleBaseTokenKeyword(model);
                break;
            case SyntaxKind.BoolTokenKeyword:
                SyntaxApi.HandleBoolTokenKeyword(model);
                break;
            case SyntaxKind.BreakTokenKeyword:
                SyntaxApi.HandleBreakTokenKeyword(model);
                break;
            case SyntaxKind.ByteTokenKeyword:
                SyntaxApi.HandleByteTokenKeyword(model);
                break;
            case SyntaxKind.CaseTokenKeyword:
                SyntaxApi.HandleCaseTokenKeyword(model);
                break;
            case SyntaxKind.CatchTokenKeyword:
                SyntaxApi.HandleCatchTokenKeyword(model);
                break;
            case SyntaxKind.CharTokenKeyword:
                SyntaxApi.HandleCharTokenKeyword(model);
                break;
            case SyntaxKind.CheckedTokenKeyword:
                SyntaxApi.HandleCheckedTokenKeyword(model);
                break;
            case SyntaxKind.ConstTokenKeyword:
                SyntaxApi.HandleConstTokenKeyword(model);
                break;
            case SyntaxKind.ContinueTokenKeyword:
                SyntaxApi.HandleContinueTokenKeyword(model);
                break;
            case SyntaxKind.DecimalTokenKeyword:
                SyntaxApi.HandleDecimalTokenKeyword(model);
                break;
            case SyntaxKind.DefaultTokenKeyword:
                SyntaxApi.HandleDefaultTokenKeyword(model);
                break;
            case SyntaxKind.DelegateTokenKeyword:
                SyntaxApi.HandleDelegateTokenKeyword(model);
                break;
            case SyntaxKind.DoTokenKeyword:
                SyntaxApi.HandleDoTokenKeyword(model);
                break;
            case SyntaxKind.DoubleTokenKeyword:
                SyntaxApi.HandleDoubleTokenKeyword(model);
                break;
            case SyntaxKind.ElseTokenKeyword:
                SyntaxApi.HandleElseTokenKeyword(model);
                break;
            case SyntaxKind.EnumTokenKeyword:
                SyntaxApi.HandleEnumTokenKeyword(model);
                break;
            case SyntaxKind.EventTokenKeyword:
                SyntaxApi.HandleEventTokenKeyword(model);
                break;
            case SyntaxKind.ExplicitTokenKeyword:
                SyntaxApi.HandleExplicitTokenKeyword(model);
                break;
            case SyntaxKind.ExternTokenKeyword:
                SyntaxApi.HandleExternTokenKeyword(model);
                break;
            case SyntaxKind.FalseTokenKeyword:
                SyntaxApi.HandleFalseTokenKeyword(model);
                break;
            case SyntaxKind.FinallyTokenKeyword:
                SyntaxApi.HandleFinallyTokenKeyword(model);
                break;
            case SyntaxKind.FixedTokenKeyword:
                SyntaxApi.HandleFixedTokenKeyword(model);
                break;
            case SyntaxKind.FloatTokenKeyword:
                SyntaxApi.HandleFloatTokenKeyword(model);
                break;
            case SyntaxKind.ForTokenKeyword:
                SyntaxApi.HandleForTokenKeyword(model);
                break;
            case SyntaxKind.ForeachTokenKeyword:
                SyntaxApi.HandleForeachTokenKeyword(model);
                break;
            case SyntaxKind.GotoTokenKeyword:
                SyntaxApi.HandleGotoTokenKeyword(model);
                break;
            case SyntaxKind.ImplicitTokenKeyword:
                SyntaxApi.HandleImplicitTokenKeyword(model);
                break;
            case SyntaxKind.InTokenKeyword:
                SyntaxApi.HandleInTokenKeyword(model);
                break;
            case SyntaxKind.IntTokenKeyword:
                SyntaxApi.HandleIntTokenKeyword(model);
                break;
            case SyntaxKind.IsTokenKeyword:
                SyntaxApi.HandleIsTokenKeyword(model);
                break;
            case SyntaxKind.LockTokenKeyword:
                SyntaxApi.HandleLockTokenKeyword(model);
                break;
            case SyntaxKind.LongTokenKeyword:
                SyntaxApi.HandleLongTokenKeyword(model);
                break;
            case SyntaxKind.NullTokenKeyword:
                SyntaxApi.HandleNullTokenKeyword(model);
                break;
            case SyntaxKind.ObjectTokenKeyword:
                SyntaxApi.HandleObjectTokenKeyword(model);
                break;
            case SyntaxKind.OperatorTokenKeyword:
                SyntaxApi.HandleOperatorTokenKeyword(model);
                break;
            case SyntaxKind.OutTokenKeyword:
                SyntaxApi.HandleOutTokenKeyword(model);
                break;
            case SyntaxKind.ParamsTokenKeyword:
                SyntaxApi.HandleParamsTokenKeyword(model);
                break;
            case SyntaxKind.ProtectedTokenKeyword:
                SyntaxApi.HandleProtectedTokenKeyword(model);
                break;
            case SyntaxKind.ReadonlyTokenKeyword:
                SyntaxApi.HandleReadonlyTokenKeyword(model);
                break;
            case SyntaxKind.RefTokenKeyword:
                SyntaxApi.HandleRefTokenKeyword(model);
                break;
            case SyntaxKind.SbyteTokenKeyword:
                SyntaxApi.HandleSbyteTokenKeyword(model);
                break;
            case SyntaxKind.ShortTokenKeyword:
                SyntaxApi.HandleShortTokenKeyword(model);
                break;
            case SyntaxKind.SizeofTokenKeyword:
                SyntaxApi.HandleSizeofTokenKeyword(model);
                break;
            case SyntaxKind.StackallocTokenKeyword:
                SyntaxApi.HandleStackallocTokenKeyword(model);
                break;
            case SyntaxKind.StringTokenKeyword:
                SyntaxApi.HandleStringTokenKeyword(model);
                break;
            case SyntaxKind.StructTokenKeyword:
                SyntaxApi.HandleStructTokenKeyword(model);
                break;
            case SyntaxKind.SwitchTokenKeyword:
                SyntaxApi.HandleSwitchTokenKeyword(model);
                break;
            case SyntaxKind.ThisTokenKeyword:
                SyntaxApi.HandleThisTokenKeyword(model);
                break;
            case SyntaxKind.ThrowTokenKeyword:
                SyntaxApi.HandleThrowTokenKeyword(model);
                break;
            case SyntaxKind.TrueTokenKeyword:
                SyntaxApi.HandleTrueTokenKeyword(model);
                break;
            case SyntaxKind.TryTokenKeyword:
                SyntaxApi.HandleTryTokenKeyword(model);
                break;
            case SyntaxKind.TypeofTokenKeyword:
                SyntaxApi.HandleTypeofTokenKeyword(model);
                break;
            case SyntaxKind.UintTokenKeyword:
                SyntaxApi.HandleUintTokenKeyword(model);
                break;
            case SyntaxKind.UlongTokenKeyword:
                SyntaxApi.HandleUlongTokenKeyword(model);
                break;
            case SyntaxKind.UncheckedTokenKeyword:
                SyntaxApi.HandleUncheckedTokenKeyword(model);
                break;
            case SyntaxKind.UnsafeTokenKeyword:
                SyntaxApi.HandleUnsafeTokenKeyword(model);
                break;
            case SyntaxKind.UshortTokenKeyword:
                SyntaxApi.HandleUshortTokenKeyword(model);
                break;
            case SyntaxKind.VoidTokenKeyword:
                SyntaxApi.HandleVoidTokenKeyword(model);
                break;
            case SyntaxKind.VolatileTokenKeyword:
                SyntaxApi.HandleVolatileTokenKeyword(model);
                break;
            case SyntaxKind.WhileTokenKeyword:
                SyntaxApi.HandleWhileTokenKeyword(model);
                break;
            case SyntaxKind.UnrecognizedTokenKeyword:
                SyntaxApi.HandleUnrecognizedTokenKeyword(model);
                break;
            case SyntaxKind.ReturnTokenKeyword:
                SyntaxApi.HandleReturnTokenKeyword(model);
                break;
            case SyntaxKind.NamespaceTokenKeyword:
                SyntaxApi.HandleNamespaceTokenKeyword(model);
                break;
            case SyntaxKind.ClassTokenKeyword:
                SyntaxApi.HandleClassTokenKeyword(model);
                break;
            case SyntaxKind.InterfaceTokenKeyword:
                SyntaxApi.HandleInterfaceTokenKeyword(model);
                break;
            case SyntaxKind.UsingTokenKeyword:
                SyntaxApi.HandleUsingTokenKeyword(model);
                break;
            case SyntaxKind.PublicTokenKeyword:
                SyntaxApi.HandlePublicTokenKeyword(model);
                break;
            case SyntaxKind.InternalTokenKeyword:
                SyntaxApi.HandleInternalTokenKeyword(model);
                break;
            case SyntaxKind.PrivateTokenKeyword:
                SyntaxApi.HandlePrivateTokenKeyword(model);
                break;
            case SyntaxKind.StaticTokenKeyword:
                SyntaxApi.HandleStaticTokenKeyword(model);
                break;
            case SyntaxKind.OverrideTokenKeyword:
                SyntaxApi.HandleOverrideTokenKeyword(model);
                break;
            case SyntaxKind.VirtualTokenKeyword:
                SyntaxApi.HandleVirtualTokenKeyword(model);
                break;
            case SyntaxKind.AbstractTokenKeyword:
                SyntaxApi.HandleAbstractTokenKeyword(model);
                break;
            case SyntaxKind.SealedTokenKeyword:
                SyntaxApi.HandleSealedTokenKeyword(model);
                break;
            case SyntaxKind.IfTokenKeyword:
                SyntaxApi.HandleIfTokenKeyword(model);
                break;
            case SyntaxKind.NewTokenKeyword:
                SyntaxApi.HandleNewTokenKeyword(model);
                break;
            default:
                SyntaxApi.HandleDefault(model);
                break;
        }
    }

    public static void ParseKeywordContextualToken(ParserModel model)
    {
        var contextualKeywordToken = model.SyntaxStack.Peek();

        switch (contextualKeywordToken.SyntaxKind)
        {
            case SyntaxKind.VarTokenContextualKeyword:
                SyntaxApi.HandleVarTokenContextualKeyword(model);
                break;
            case SyntaxKind.PartialTokenContextualKeyword:
                SyntaxApi.HandlePartialTokenContextualKeyword(model);
                break;
            case SyntaxKind.AddTokenContextualKeyword:
                SyntaxApi.HandleAddTokenContextualKeyword(model);
                break;
            case SyntaxKind.AndTokenContextualKeyword:
                SyntaxApi.HandleAndTokenContextualKeyword(model);
                break;
            case SyntaxKind.AliasTokenContextualKeyword:
                SyntaxApi.HandleAliasTokenContextualKeyword(model);
                break;
            case SyntaxKind.AscendingTokenContextualKeyword:
                SyntaxApi.HandleAscendingTokenContextualKeyword(model);
                break;
            case SyntaxKind.ArgsTokenContextualKeyword:
                SyntaxApi.HandleArgsTokenContextualKeyword(model);
                break;
            case SyntaxKind.AsyncTokenContextualKeyword:
                SyntaxApi.HandleAsyncTokenContextualKeyword(model);
                break;
            case SyntaxKind.AwaitTokenContextualKeyword:
                SyntaxApi.HandleAwaitTokenContextualKeyword(model);
                break;
            case SyntaxKind.ByTokenContextualKeyword:
                SyntaxApi.HandleByTokenContextualKeyword(model);
                break;
            case SyntaxKind.DescendingTokenContextualKeyword:
                SyntaxApi.HandleDescendingTokenContextualKeyword(model);
                break;
            case SyntaxKind.DynamicTokenContextualKeyword:
                SyntaxApi.HandleDynamicTokenContextualKeyword(model);
                break;
            case SyntaxKind.EqualsTokenContextualKeyword:
                SyntaxApi.HandleEqualsTokenContextualKeyword(model);
                break;
            case SyntaxKind.FileTokenContextualKeyword:
                SyntaxApi.HandleFileTokenContextualKeyword(model);
                break;
            case SyntaxKind.FromTokenContextualKeyword:
                SyntaxApi.HandleFromTokenContextualKeyword(model);
                break;
            case SyntaxKind.GetTokenContextualKeyword:
                SyntaxApi.HandleGetTokenContextualKeyword(model);
                break;
            case SyntaxKind.GlobalTokenContextualKeyword:
                SyntaxApi.HandleGlobalTokenContextualKeyword(model);
                break;
            case SyntaxKind.GroupTokenContextualKeyword:
                SyntaxApi.HandleGroupTokenContextualKeyword(model);
                break;
            case SyntaxKind.InitTokenContextualKeyword:
                SyntaxApi.HandleInitTokenContextualKeyword(model);
                break;
            case SyntaxKind.IntoTokenContextualKeyword:
                SyntaxApi.HandleIntoTokenContextualKeyword(model);
                break;
            case SyntaxKind.JoinTokenContextualKeyword:
                SyntaxApi.HandleJoinTokenContextualKeyword(model);
                break;
            case SyntaxKind.LetTokenContextualKeyword:
                SyntaxApi.HandleLetTokenContextualKeyword(model);
                break;
            case SyntaxKind.ManagedTokenContextualKeyword:
                SyntaxApi.HandleManagedTokenContextualKeyword(model);
                break;
            case SyntaxKind.NameofTokenContextualKeyword:
                SyntaxApi.HandleNameofTokenContextualKeyword(model);
                break;
            case SyntaxKind.NintTokenContextualKeyword:
                SyntaxApi.HandleNintTokenContextualKeyword(model);
                break;
            case SyntaxKind.NotTokenContextualKeyword:
                SyntaxApi.HandleNotTokenContextualKeyword(model);
                break;
            case SyntaxKind.NotnullTokenContextualKeyword:
                SyntaxApi.HandleNotnullTokenContextualKeyword(model);
                break;
            case SyntaxKind.NuintTokenContextualKeyword:
                SyntaxApi.HandleNuintTokenContextualKeyword(model);
                break;
            case SyntaxKind.OnTokenContextualKeyword:
                SyntaxApi.HandleOnTokenContextualKeyword(model);
                break;
            case SyntaxKind.OrTokenContextualKeyword:
                SyntaxApi.HandleOrTokenContextualKeyword(model);
                break;
            case SyntaxKind.OrderbyTokenContextualKeyword:
                SyntaxApi.HandleOrderbyTokenContextualKeyword(model);
                break;
            case SyntaxKind.RecordTokenContextualKeyword:
                SyntaxApi.HandleRecordTokenContextualKeyword(model);
                break;
            case SyntaxKind.RemoveTokenContextualKeyword:
                SyntaxApi.HandleRemoveTokenContextualKeyword(model);
                break;
            case SyntaxKind.RequiredTokenContextualKeyword:
                SyntaxApi.HandleRequiredTokenContextualKeyword(model);
                break;
            case SyntaxKind.ScopedTokenContextualKeyword:
                SyntaxApi.HandleScopedTokenContextualKeyword(model);
                break;
            case SyntaxKind.SelectTokenContextualKeyword:
                SyntaxApi.HandleSelectTokenContextualKeyword(model);
                break;
            case SyntaxKind.SetTokenContextualKeyword:
                SyntaxApi.HandleSetTokenContextualKeyword(model);
                break;
            case SyntaxKind.UnmanagedTokenContextualKeyword:
                SyntaxApi.HandleUnmanagedTokenContextualKeyword(model);
                break;
            case SyntaxKind.ValueTokenContextualKeyword:
                SyntaxApi.HandleValueTokenContextualKeyword(model);
                break;
            case SyntaxKind.WhenTokenContextualKeyword:
                SyntaxApi.HandleWhenTokenContextualKeyword(model);
                break;
            case SyntaxKind.WhereTokenContextualKeyword:
                SyntaxApi.HandleWhereTokenContextualKeyword(model);
                break;
            case SyntaxKind.WithTokenContextualKeyword:
                SyntaxApi.HandleWithTokenContextualKeyword(model);
                break;
            case SyntaxKind.YieldTokenContextualKeyword:
                SyntaxApi.HandleYieldTokenContextualKeyword(model);
                break;
            case SyntaxKind.UnrecognizedTokenContextualKeyword:
                SyntaxApi.HandleUnrecognizedTokenContextualKeyword(model);
                break;
            default:
                throw new NotImplementedException($"Implement the {contextualKeywordToken.SyntaxKind.ToString()} contextual keyword.");
        }
    }
}