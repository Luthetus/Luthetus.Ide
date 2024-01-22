using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Enums;

namespace Luthetus.CompilerServices.Lang.CSharp.ParserCase.Internals;

public static class ParseTokens
{
    public static void ParseNumericLiteralToken(ParserModel model)
    {
        var numericLiteralToken = (NumericLiteralToken)model.SyntaxStack.Pop();

        // The handle expression won't see this token unless backtracked.
        model.TokenWalker.Backtrack();
        ParseOthers.HandleExpression(
            null,
            null,
            null,
            null,
            null,
            null,
            model);

        model.CurrentCodeBlockBuilder.ChildList.Add(
            (IExpressionNode)model.SyntaxStack.Pop());
    }

    public static void ParseStringLiteralToken(ParserModel model)
    {
        var stringLiteralToken = (StringLiteralToken)model.SyntaxStack.Pop();

        // The handle expression won't see this token unless backtracked.
        model.TokenWalker.Backtrack();
        ParseOthers.HandleExpression(
            null,
            null,
            null,
            null,
            null,
            null,
            model);

        model.CurrentCodeBlockBuilder.ChildList.Add(
            (IExpressionNode)model.SyntaxStack.Pop());
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

        if (TryParseVariableAssignment(model))
            return;

        if (TryParseGenericTypeOrFunctionInvocation(model))
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
            ParseTypes.HandleGenericArguments(model);
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
            ParseTypes.HandleGenericParameters(model);
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
            ParseFunctions.HandleConstructorDefinition(model);
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

            ParseOthers.HandleNamespaceReference(model);
            return true;
        }
        else
        {
            if (model.Binder.TryGetVariableDeclarationHierarchically(text, out var variableDeclarationStatementNode) &&
                variableDeclarationStatementNode is not null)
            {
                model.SyntaxStack.Push(identifierToken);
                model.SyntaxStack.Push(variableDeclarationStatementNode);
                ParseVariables.HandleVariableReference(model);
                return true;
            }
            else
            {
                // 'undeclared-variable reference' OR 'static class identifier'

                if (model.Binder.TryGetTypeDefinitionHierarchically(text, out var typeDefinitionNode) &&
                    typeDefinitionNode is not null)
                {
                    model.SyntaxStack.Push(identifierToken);
                    ParseTypes.HandleStaticClassIdentifier(model);
                    return true;
                }
                else
                {
                    model.SyntaxStack.Push(identifierToken);
                    ParseTypes.HandleUndefinedTypeOrNamespaceReference(model);
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
            ParseVariables.HandleVariableAssignment(model);
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
                var typeClauseNode = new TypeClauseNode(
                    identifierToken,
                    null,
                    genericParametersListingNode);

                model.Binder.BindTypeClauseNode(typeClauseNode);
                model.SyntaxStack.Push(typeClauseNode);
                return true;
            }
        }

        // Function invocation
        model.SyntaxStack.Push(identifierToken);
        ParseFunctions.HandleFunctionInvocation(genericParametersListingNode, model);
        return true;
    }

    private static bool TryParseFunctionDefinition(ParserModel model)
    {
        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken)
        {
            ParseFunctions.HandleFunctionDefinition(model);
            return true;
        }

        return false;
    }

    private static bool TryParseVariableDeclaration(ParserModel model)
    {
        GenericArgumentsListingNode? genericArgumentsListingNode =
            model.SyntaxStack.Peek().SyntaxKind == SyntaxKind.GenericArgumentsListingNode
                ? (GenericArgumentsListingNode)model.SyntaxStack.Pop()
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
            ParseVariables.HandleVariableDeclaration(variableKind.Value, model);
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

        // The handle expression won't see this token unless backtracked.
        model.TokenWalker.Backtrack();
        ParseOthers.HandleExpression(
            null,
            null,
            null,
            null,
            null,
            null,
            model);

        model.CurrentCodeBlockBuilder.ChildList.Add(
            (IExpressionNode)model.SyntaxStack.Pop());
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

        // The handle expression won't see this token unless backtracked.
        model.TokenWalker.Backtrack();
        ParseOthers.HandleExpression(
            null,
            null,
            null,
            null,
            null,
            null,
            model);

        model.CurrentCodeBlockBuilder.ChildList.Add(
            (IExpressionNode)model.SyntaxStack.Pop());
    }

    public static void ParseStarToken(ParserModel model)
    {
        var starToken = (StarToken)model.SyntaxStack.Pop();

        // The handle expression won't see this token unless backtracked.
        model.TokenWalker.Backtrack();
        ParseOthers.HandleExpression(
            null,
            null,
            null,
            null,
            null,
            null,
            model);

        model.CurrentCodeBlockBuilder.ChildList.Add(
            (IExpressionNode)model.SyntaxStack.Pop());
    }

    public static void ParseDollarSignToken(ParserModel model)
    {
        var dollarSignToken = (DollarSignToken)model.SyntaxStack.Pop();

        // The handle expression won't see this token unless backtracked.
        model.TokenWalker.Backtrack();
        ParseOthers.HandleExpression(
            null,
            null,
            null,
            null,
            null,
            null,
            model);

        model.CurrentCodeBlockBuilder.ChildList.Add(
            (IExpressionNode)model.SyntaxStack.Pop());
    }

    public static void ParseColonToken(ParserModel model)
    {
        var colonToken = (ColonToken)model.SyntaxStack.Pop();

        if (model.SyntaxStack.TryPeek(out var syntax) && syntax.SyntaxKind == SyntaxKind.TypeDefinitionNode)
        {
            var typeDefinitionNode = (TypeDefinitionNode)model.SyntaxStack.Pop();
            var inheritedTypeClauseNode = model.TokenWalker.MatchTypeClauseNode(model);

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
            model.DiagnosticBag.ReportTodoException(colonToken.TextSpan, "Colon is in unexpected place.");
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
                    functionDefinitionNode.FunctionIdentifierToken,
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

        model.Binder.RegisterBoundScope(scopeReturnTypeClauseNode, openBraceToken.TextSpan);

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

        // The handle expression won't see this token unless backtracked.
        model.TokenWalker.Backtrack();
        ParseOthers.HandleExpression(
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
        ParseOthers.HandleExpression(
            parenthesizedExpression,
            parenthesizedExpression,
            null,
            null,
            null,
            null,
            model);

        model.CurrentCodeBlockBuilder.ChildList.Add(
            (IExpressionNode)model.SyntaxStack.Pop());
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
            ParseTypes.HandleGenericArguments(model);

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
            ParseTypes.HandleAttribute(model);
        }
    }

    public static void ParseCloseSquareBracketToken(ParserModel model)
    {
        var closeSquareBracketToken = (CloseSquareBracketToken)model.SyntaxStack.Pop();
    }

    public static void ParseMemberAccessToken(ParserModel model)
    {
        var memberAccessToken = (MemberAccessToken)model.SyntaxStack.Pop();
        model.SyntaxStack.Push(memberAccessToken);

        var isValidMemberAccessToken = true;

        if (model.SyntaxStack.TryPeek(out var syntax) && syntax is not null)
        {
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
        else
        {
            isValidMemberAccessToken = false;
        }

        if (!isValidMemberAccessToken)
            model.DiagnosticBag.ReportTodoException(memberAccessToken.TextSpan, "MemberAccessToken needs further implementation.");
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
                ParseDefaultKeywords.HandleAsTokenKeyword(model);
                break;
            case SyntaxKind.BaseTokenKeyword:
                ParseDefaultKeywords.HandleBaseTokenKeyword(model);
                break;
            case SyntaxKind.BoolTokenKeyword:
                ParseDefaultKeywords.HandleBoolTokenKeyword(model);
                break;
            case SyntaxKind.BreakTokenKeyword:
                ParseDefaultKeywords.HandleBreakTokenKeyword(model);
                break;
            case SyntaxKind.ByteTokenKeyword:
                ParseDefaultKeywords.HandleByteTokenKeyword(model);
                break;
            case SyntaxKind.CaseTokenKeyword:
                ParseDefaultKeywords.HandleCaseTokenKeyword(model);
                break;
            case SyntaxKind.CatchTokenKeyword:
                ParseDefaultKeywords.HandleCatchTokenKeyword(model);
                break;
            case SyntaxKind.CharTokenKeyword:
                ParseDefaultKeywords.HandleCharTokenKeyword(model);
                break;
            case SyntaxKind.CheckedTokenKeyword:
                ParseDefaultKeywords.HandleCheckedTokenKeyword(model);
                break;
            case SyntaxKind.ConstTokenKeyword:
                ParseDefaultKeywords.HandleConstTokenKeyword(model);
                break;
            case SyntaxKind.ContinueTokenKeyword:
                ParseDefaultKeywords.HandleContinueTokenKeyword(model);
                break;
            case SyntaxKind.DecimalTokenKeyword:
                ParseDefaultKeywords.HandleDecimalTokenKeyword(model);
                break;
            case SyntaxKind.DefaultTokenKeyword:
                ParseDefaultKeywords.HandleDefaultTokenKeyword(model);
                break;
            case SyntaxKind.DelegateTokenKeyword:
                ParseDefaultKeywords.HandleDelegateTokenKeyword(model);
                break;
            case SyntaxKind.DoTokenKeyword:
                ParseDefaultKeywords.HandleDoTokenKeyword(model);
                break;
            case SyntaxKind.DoubleTokenKeyword:
                ParseDefaultKeywords.HandleDoubleTokenKeyword(model);
                break;
            case SyntaxKind.ElseTokenKeyword:
                ParseDefaultKeywords.HandleElseTokenKeyword(model);
                break;
            case SyntaxKind.EnumTokenKeyword:
                ParseDefaultKeywords.HandleEnumTokenKeyword(model);
                break;
            case SyntaxKind.EventTokenKeyword:
                ParseDefaultKeywords.HandleEventTokenKeyword(model);
                break;
            case SyntaxKind.ExplicitTokenKeyword:
                ParseDefaultKeywords.HandleExplicitTokenKeyword(model);
                break;
            case SyntaxKind.ExternTokenKeyword:
                ParseDefaultKeywords.HandleExternTokenKeyword(model);
                break;
            case SyntaxKind.FalseTokenKeyword:
                ParseDefaultKeywords.HandleFalseTokenKeyword(model);
                break;
            case SyntaxKind.FinallyTokenKeyword:
                ParseDefaultKeywords.HandleFinallyTokenKeyword(model);
                break;
            case SyntaxKind.FixedTokenKeyword:
                ParseDefaultKeywords.HandleFixedTokenKeyword(model);
                break;
            case SyntaxKind.FloatTokenKeyword:
                ParseDefaultKeywords.HandleFloatTokenKeyword(model);
                break;
            case SyntaxKind.ForTokenKeyword:
                ParseDefaultKeywords.HandleForTokenKeyword(model);
                break;
            case SyntaxKind.ForeachTokenKeyword:
                ParseDefaultKeywords.HandleForeachTokenKeyword(model);
                break;
            case SyntaxKind.GotoTokenKeyword:
                ParseDefaultKeywords.HandleGotoTokenKeyword(model);
                break;
            case SyntaxKind.ImplicitTokenKeyword:
                ParseDefaultKeywords.HandleImplicitTokenKeyword(model);
                break;
            case SyntaxKind.InTokenKeyword:
                ParseDefaultKeywords.HandleInTokenKeyword(model);
                break;
            case SyntaxKind.IntTokenKeyword:
                ParseDefaultKeywords.HandleIntTokenKeyword(model);
                break;
            case SyntaxKind.IsTokenKeyword:
                ParseDefaultKeywords.HandleIsTokenKeyword(model);
                break;
            case SyntaxKind.LockTokenKeyword:
                ParseDefaultKeywords.HandleLockTokenKeyword(model);
                break;
            case SyntaxKind.LongTokenKeyword:
                ParseDefaultKeywords.HandleLongTokenKeyword(model);
                break;
            case SyntaxKind.NullTokenKeyword:
                ParseDefaultKeywords.HandleNullTokenKeyword(model);
                break;
            case SyntaxKind.ObjectTokenKeyword:
                ParseDefaultKeywords.HandleObjectTokenKeyword(model);
                break;
            case SyntaxKind.OperatorTokenKeyword:
                ParseDefaultKeywords.HandleOperatorTokenKeyword(model);
                break;
            case SyntaxKind.OutTokenKeyword:
                ParseDefaultKeywords.HandleOutTokenKeyword(model);
                break;
            case SyntaxKind.ParamsTokenKeyword:
                ParseDefaultKeywords.HandleParamsTokenKeyword(model);
                break;
            case SyntaxKind.ProtectedTokenKeyword:
                ParseDefaultKeywords.HandleProtectedTokenKeyword(model);
                break;
            case SyntaxKind.ReadonlyTokenKeyword:
                ParseDefaultKeywords.HandleReadonlyTokenKeyword(model);
                break;
            case SyntaxKind.RefTokenKeyword:
                ParseDefaultKeywords.HandleRefTokenKeyword(model);
                break;
            case SyntaxKind.SbyteTokenKeyword:
                ParseDefaultKeywords.HandleSbyteTokenKeyword(model);
                break;
            case SyntaxKind.ShortTokenKeyword:
                ParseDefaultKeywords.HandleShortTokenKeyword(model);
                break;
            case SyntaxKind.SizeofTokenKeyword:
                ParseDefaultKeywords.HandleSizeofTokenKeyword(model);
                break;
            case SyntaxKind.StackallocTokenKeyword:
                ParseDefaultKeywords.HandleStackallocTokenKeyword(model);
                break;
            case SyntaxKind.StringTokenKeyword:
                ParseDefaultKeywords.HandleStringTokenKeyword(model);
                break;
            case SyntaxKind.StructTokenKeyword:
                ParseDefaultKeywords.HandleStructTokenKeyword(model);
                break;
            case SyntaxKind.SwitchTokenKeyword:
                ParseDefaultKeywords.HandleSwitchTokenKeyword(model);
                break;
            case SyntaxKind.ThisTokenKeyword:
                ParseDefaultKeywords.HandleThisTokenKeyword(model);
                break;
            case SyntaxKind.ThrowTokenKeyword:
                ParseDefaultKeywords.HandleThrowTokenKeyword(model);
                break;
            case SyntaxKind.TrueTokenKeyword:
                ParseDefaultKeywords.HandleTrueTokenKeyword(model);
                break;
            case SyntaxKind.TryTokenKeyword:
                ParseDefaultKeywords.HandleTryTokenKeyword(model);
                break;
            case SyntaxKind.TypeofTokenKeyword:
                ParseDefaultKeywords.HandleTypeofTokenKeyword(model);
                break;
            case SyntaxKind.UintTokenKeyword:
                ParseDefaultKeywords.HandleUintTokenKeyword(model);
                break;
            case SyntaxKind.UlongTokenKeyword:
                ParseDefaultKeywords.HandleUlongTokenKeyword(model);
                break;
            case SyntaxKind.UncheckedTokenKeyword:
                ParseDefaultKeywords.HandleUncheckedTokenKeyword(model);
                break;
            case SyntaxKind.UnsafeTokenKeyword:
                ParseDefaultKeywords.HandleUnsafeTokenKeyword(model);
                break;
            case SyntaxKind.UshortTokenKeyword:
                ParseDefaultKeywords.HandleUshortTokenKeyword(model);
                break;
            case SyntaxKind.VoidTokenKeyword:
                ParseDefaultKeywords.HandleVoidTokenKeyword(model);
                break;
            case SyntaxKind.VolatileTokenKeyword:
                ParseDefaultKeywords.HandleVolatileTokenKeyword(model);
                break;
            case SyntaxKind.WhileTokenKeyword:
                ParseDefaultKeywords.HandleWhileTokenKeyword(model);
                break;
            case SyntaxKind.UnrecognizedTokenKeyword:
                ParseDefaultKeywords.HandleUnrecognizedTokenKeyword(model);
                break;
            case SyntaxKind.ReturnTokenKeyword:
                ParseDefaultKeywords.HandleReturnTokenKeyword(model);
                break;
            case SyntaxKind.NamespaceTokenKeyword:
                ParseDefaultKeywords.HandleNamespaceTokenKeyword(model);
                break;
            case SyntaxKind.ClassTokenKeyword:
                ParseDefaultKeywords.HandleClassTokenKeyword(model);
                break;
            case SyntaxKind.InterfaceTokenKeyword:
                ParseDefaultKeywords.HandleInterfaceTokenKeyword(model);
                break;
            case SyntaxKind.UsingTokenKeyword:
                ParseDefaultKeywords.HandleUsingTokenKeyword(model);
                break;
            case SyntaxKind.PublicTokenKeyword:
                ParseDefaultKeywords.HandlePublicTokenKeyword(model);
                break;
            case SyntaxKind.InternalTokenKeyword:
                ParseDefaultKeywords.HandleInternalTokenKeyword(model);
                break;
            case SyntaxKind.PrivateTokenKeyword:
                ParseDefaultKeywords.HandlePrivateTokenKeyword(model);
                break;
            case SyntaxKind.StaticTokenKeyword:
                ParseDefaultKeywords.HandleStaticTokenKeyword(model);
                break;
            case SyntaxKind.OverrideTokenKeyword:
                ParseDefaultKeywords.HandleOverrideTokenKeyword(model);
                break;
            case SyntaxKind.VirtualTokenKeyword:
                ParseDefaultKeywords.HandleVirtualTokenKeyword(model);
                break;
            case SyntaxKind.AbstractTokenKeyword:
                ParseDefaultKeywords.HandleAbstractTokenKeyword(model);
                break;
            case SyntaxKind.SealedTokenKeyword:
                ParseDefaultKeywords.HandleSealedTokenKeyword(model);
                break;
            case SyntaxKind.IfTokenKeyword:
                ParseDefaultKeywords.HandleIfTokenKeyword(model);
                break;
            case SyntaxKind.NewTokenKeyword:
                ParseDefaultKeywords.HandleNewTokenKeyword(model);
                break;
            default:
                ParseDefaultKeywords.HandleDefault(model);
                break;
        }
    }

    public static void ParseKeywordContextualToken(ParserModel model)
    {
        var contextualKeywordToken = model.SyntaxStack.Peek();

        switch (contextualKeywordToken.SyntaxKind)
        {
            case SyntaxKind.VarTokenContextualKeyword:
                ParseContextualKeywords.HandleVarTokenContextualKeyword(model);
                break;
            case SyntaxKind.PartialTokenContextualKeyword:
                ParseContextualKeywords.HandlePartialTokenContextualKeyword(model);
                break;
            case SyntaxKind.AddTokenContextualKeyword:
                ParseContextualKeywords.HandleAddTokenContextualKeyword(model);
                break;
            case SyntaxKind.AndTokenContextualKeyword:
                ParseContextualKeywords.HandleAndTokenContextualKeyword(model);
                break;
            case SyntaxKind.AliasTokenContextualKeyword:
                ParseContextualKeywords.HandleAliasTokenContextualKeyword(model);
                break;
            case SyntaxKind.AscendingTokenContextualKeyword:
                ParseContextualKeywords.HandleAscendingTokenContextualKeyword(model);
                break;
            case SyntaxKind.ArgsTokenContextualKeyword:
                ParseContextualKeywords.HandleArgsTokenContextualKeyword(model);
                break;
            case SyntaxKind.AsyncTokenContextualKeyword:
                ParseContextualKeywords.HandleAsyncTokenContextualKeyword(model);
                break;
            case SyntaxKind.AwaitTokenContextualKeyword:
                ParseContextualKeywords.HandleAwaitTokenContextualKeyword(model);
                break;
            case SyntaxKind.ByTokenContextualKeyword:
                ParseContextualKeywords.HandleByTokenContextualKeyword(model);
                break;
            case SyntaxKind.DescendingTokenContextualKeyword:
                ParseContextualKeywords.HandleDescendingTokenContextualKeyword(model);
                break;
            case SyntaxKind.DynamicTokenContextualKeyword:
                ParseContextualKeywords.HandleDynamicTokenContextualKeyword(model);
                break;
            case SyntaxKind.EqualsTokenContextualKeyword:
                ParseContextualKeywords.HandleEqualsTokenContextualKeyword(model);
                break;
            case SyntaxKind.FileTokenContextualKeyword:
                ParseContextualKeywords.HandleFileTokenContextualKeyword(model);
                break;
            case SyntaxKind.FromTokenContextualKeyword:
                ParseContextualKeywords.HandleFromTokenContextualKeyword(model);
                break;
            case SyntaxKind.GetTokenContextualKeyword:
                ParseContextualKeywords.HandleGetTokenContextualKeyword(model);
                break;
            case SyntaxKind.GlobalTokenContextualKeyword:
                ParseContextualKeywords.HandleGlobalTokenContextualKeyword(model);
                break;
            case SyntaxKind.GroupTokenContextualKeyword:
                ParseContextualKeywords.HandleGroupTokenContextualKeyword(model);
                break;
            case SyntaxKind.InitTokenContextualKeyword:
                ParseContextualKeywords.HandleInitTokenContextualKeyword(model);
                break;
            case SyntaxKind.IntoTokenContextualKeyword:
                ParseContextualKeywords.HandleIntoTokenContextualKeyword(model);
                break;
            case SyntaxKind.JoinTokenContextualKeyword:
                ParseContextualKeywords.HandleJoinTokenContextualKeyword(model);
                break;
            case SyntaxKind.LetTokenContextualKeyword:
                ParseContextualKeywords.HandleLetTokenContextualKeyword(model);
                break;
            case SyntaxKind.ManagedTokenContextualKeyword:
                ParseContextualKeywords.HandleManagedTokenContextualKeyword(model);
                break;
            case SyntaxKind.NameofTokenContextualKeyword:
                ParseContextualKeywords.HandleNameofTokenContextualKeyword(model);
                break;
            case SyntaxKind.NintTokenContextualKeyword:
                ParseContextualKeywords.HandleNintTokenContextualKeyword(model);
                break;
            case SyntaxKind.NotTokenContextualKeyword:
                ParseContextualKeywords.HandleNotTokenContextualKeyword(model);
                break;
            case SyntaxKind.NotnullTokenContextualKeyword:
                ParseContextualKeywords.HandleNotnullTokenContextualKeyword(model);
                break;
            case SyntaxKind.NuintTokenContextualKeyword:
                ParseContextualKeywords.HandleNuintTokenContextualKeyword(model);
                break;
            case SyntaxKind.OnTokenContextualKeyword:
                ParseContextualKeywords.HandleOnTokenContextualKeyword(model);
                break;
            case SyntaxKind.OrTokenContextualKeyword:
                ParseContextualKeywords.HandleOrTokenContextualKeyword(model);
                break;
            case SyntaxKind.OrderbyTokenContextualKeyword:
                ParseContextualKeywords.HandleOrderbyTokenContextualKeyword(model);
                break;
            case SyntaxKind.RecordTokenContextualKeyword:
                ParseContextualKeywords.HandleRecordTokenContextualKeyword(model);
                break;
            case SyntaxKind.RemoveTokenContextualKeyword:
                ParseContextualKeywords.HandleRemoveTokenContextualKeyword(model);
                break;
            case SyntaxKind.RequiredTokenContextualKeyword:
                ParseContextualKeywords.HandleRequiredTokenContextualKeyword(model);
                break;
            case SyntaxKind.ScopedTokenContextualKeyword:
                ParseContextualKeywords.HandleScopedTokenContextualKeyword(model);
                break;
            case SyntaxKind.SelectTokenContextualKeyword:
                ParseContextualKeywords.HandleSelectTokenContextualKeyword(model);
                break;
            case SyntaxKind.SetTokenContextualKeyword:
                ParseContextualKeywords.HandleSetTokenContextualKeyword(model);
                break;
            case SyntaxKind.UnmanagedTokenContextualKeyword:
                ParseContextualKeywords.HandleUnmanagedTokenContextualKeyword(model);
                break;
            case SyntaxKind.ValueTokenContextualKeyword:
                ParseContextualKeywords.HandleValueTokenContextualKeyword(model);
                break;
            case SyntaxKind.WhenTokenContextualKeyword:
                ParseContextualKeywords.HandleWhenTokenContextualKeyword(model);
                break;
            case SyntaxKind.WhereTokenContextualKeyword:
                ParseContextualKeywords.HandleWhereTokenContextualKeyword(model);
                break;
            case SyntaxKind.WithTokenContextualKeyword:
                ParseContextualKeywords.HandleWithTokenContextualKeyword(model);
                break;
            case SyntaxKind.YieldTokenContextualKeyword:
                ParseContextualKeywords.HandleYieldTokenContextualKeyword(model);
                break;
            case SyntaxKind.UnrecognizedTokenContextualKeyword:
                ParseContextualKeywords.HandleUnrecognizedTokenContextualKeyword(model);
                break;
            default:
                throw new NotImplementedException($"Implement the {contextualKeywordToken.SyntaxKind.ToString()} contextual keyword.");
        }
    }
}