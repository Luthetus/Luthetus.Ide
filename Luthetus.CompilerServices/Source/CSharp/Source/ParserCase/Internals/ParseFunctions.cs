using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;

namespace Luthetus.CompilerServices.Lang.CSharp.ParserCase.Internals;

public class ParseFunctions
{
    public static void HandleFunctionInvocation(
        GenericParametersListingNode? genericParametersListingNode,
        ParserModel model)
    {
        var identifierToken = (IdentifierToken)model.SyntaxStack.Pop();

        // TODO: (2023-06-04) I believe this if block will run for '<' mathematical operator.

        model.SyntaxStack.Push((OpenParenthesisToken)model.TokenWalker.Match(SyntaxKind.OpenParenthesisToken));
        HandleFunctionParameters(model);

        var functionParametersListingNode = (FunctionParametersListingNode)model.SyntaxStack.Pop();

        var functionInvocationNode = new FunctionInvocationNode(
            identifierToken,
            null,
            genericParametersListingNode,
            functionParametersListingNode);

        model.Binder.BindFunctionInvocationNode(functionInvocationNode);
        model.CurrentCodeBlockBuilder.ChildList.Add(functionInvocationNode);
    }

    public static void HandleFunctionDefinition(ParserModel model)
    {
        GenericArgumentsListingNode? genericArgumentsListingNode =
            model.SyntaxStack.Peek().SyntaxKind == SyntaxKind.GenericArgumentsListingNode
                ? (GenericArgumentsListingNode)model.SyntaxStack.Pop()
                : null;

        var identifierToken = (IdentifierToken)model.SyntaxStack.Pop();
        var typeClauseNode = (TypeClauseNode)model.SyntaxStack.Pop();

        if (model.TokenWalker.Current.SyntaxKind != SyntaxKind.OpenParenthesisToken)
            return;

        model.SyntaxStack.Push(model.TokenWalker.Consume());
        HandleFunctionArguments(model);

        var functionArgumentsListingNode = (FunctionArgumentsListingNode)model.SyntaxStack.Pop();

        var functionDefinitionNode = new FunctionDefinitionNode(
            typeClauseNode,
            identifierToken,
            genericArgumentsListingNode,
            functionArgumentsListingNode,
            null,
            null);

        model.Binder.BindFunctionDefinitionNode(functionDefinitionNode);
        model.SyntaxStack.Push(functionDefinitionNode);

        if (model.CurrentCodeBlockBuilder.CodeBlockOwner is TypeDefinitionNode typeDefinitionNode &&
            typeDefinitionNode.IsInterface)
        {
            // TODO: Would method constraints break this code? "public T Aaa<T>() where T : OtherClass"
            var statementDelimiterToken = model.TokenWalker.Match(SyntaxKind.StatementDelimiterToken);

            // TODO: Fabricating an OpenBraceToken in order to not duplicate the logic within 'ParseOpenBraceToken(...)' seems silly. This likely should be changed
            model.SyntaxStack.Push(new OpenBraceToken(statementDelimiterToken.TextSpan)
            {
                IsFabricated = true
            });
            ParseTokens.ParseOpenBraceToken(model);

            // TODO: Fabricating a CloseBraceToken in order to not duplicate the logic within 'ParseOpenBraceToken(...)' seems silly. This likely should be changed
            model.SyntaxStack.Push(new CloseBraceToken(statementDelimiterToken.TextSpan)
            {
                IsFabricated = true
            });
            ParseTokens.ParseCloseBraceToken(model);
        }
    }

    public static void HandleConstructorDefinition(ParserModel model)
    {
        var identifierToken = (IdentifierToken)model.SyntaxStack.Pop();

        model.SyntaxStack.Push((OpenParenthesisToken)model.TokenWalker.Consume());
        HandleFunctionArguments(model);

        var functionArgumentsListingNode = (FunctionArgumentsListingNode)model.SyntaxStack.Pop();

        if (model.CurrentCodeBlockBuilder.CodeBlockOwner is not TypeDefinitionNode typeDefinitionNode)
        {
            model.DiagnosticBag.ReportConstructorsNeedToBeWithinTypeDefinition(identifierToken.TextSpan);
            typeDefinitionNode = Facts.CSharpFacts.Types.Void;
        }

        var typeClauseNode = new TypeClauseNode(
            typeDefinitionNode.TypeIdentifier,
            null,
            null);

        var constructorDefinitionNode = new ConstructorDefinitionNode(
            typeClauseNode,
            identifierToken,
            null,
            functionArgumentsListingNode,
            null,
            null);

        model.Binder.BindConstructorDefinitionIdentifierToken(identifierToken);
        model.SyntaxStack.Push(constructorDefinitionNode);

        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.ColonToken)
        {
            // Constructor invokes some other constructor as well

            while (!model.TokenWalker.IsEof)
            {
                if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken ||
                    model.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
                {
                    break;
                }

                _ = model.TokenWalker.Consume();
            }
        }
    }

    public static void HandleConstructorInvocation(ParserModel model)
    {
        var newKeywordToken = model.TokenWalker.Consume();

        TypeClauseNode typeClauseNode;

        if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken)
        {
            typeClauseNode = Facts.CSharpFacts.Types.Var.ToTypeClause();
        }
        else
        {
            typeClauseNode = model.TokenWalker.MatchTypeClauseNode(model);

            if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken)
            {
                model.SyntaxStack.Push(model.TokenWalker.Consume());
                HandleFunctionParameters(model);
                // Discard the function parameters result for now, until further implementation is done.
                _ = model.SyntaxStack.Pop();
            }
        }

        var constructorInvocationExpressionNode = new ConstructorInvocationExpressionNode(
            (KeywordToken)newKeywordToken,
            typeClauseNode);

        model.SyntaxStack.Push(constructorInvocationExpressionNode);
    }

    /// <summary>
    /// As of (2023-08-04), when one enters this method,
    /// they have just finished parsing the function's identifier.
    /// <br/><br/>
    /// As a result, this method takes in an ImmutableArray of <see cref="FunctionDefinitionNode"/>
    /// where each entry is a function overload.
    /// <br/><br/>
    /// It has yet to be determined, which overload one is referring to.
    /// This method must compare the actual text's parameter types
    /// again the available overloads and determine which to use specifically.
    /// </summary>
    public static void HandleFunctionReferences(
        ImmutableArray<FunctionDefinitionNode> functionDefinitionNodes,
        ParserModel model)
    {
        var functionInvocationIdentifierToken = (IdentifierToken)model.SyntaxStack.Pop();

        var concatenatedGetTextResults = string.Join(
            '\n',
            functionDefinitionNodes.Select(fd => fd.GetTextRecursively()));

        if (SyntaxKind.OpenParenthesisToken == model.TokenWalker.Current.SyntaxKind)
        {
            model.SyntaxStack.Push((OpenParenthesisToken)model.TokenWalker.Consume());
            HandleFunctionParameters(model);

            var functionParametersListingNode = (FunctionParametersListingNode)model.SyntaxStack.Pop();
            FunctionDefinitionNode? matchingOverload = null;

            foreach (var functionDefinitionNode in functionDefinitionNodes)
            {
                if (functionParametersListingNode.SatisfiesArguments(
                        functionDefinitionNode.FunctionArgumentsListingNode))
                {
                    matchingOverload = functionDefinitionNode;
                    break;
                }
            }

            if (matchingOverload is null)
            {
                model.DiagnosticBag.ReportTodoException(
                    functionInvocationIdentifierToken.TextSpan,
                    "TODO: Handle case where none of the function overloads match the input.");
            }

            // TODO: Don't assume GenericParametersListingNode to be null
            var functionInvocationNode = new FunctionInvocationNode(
                functionInvocationIdentifierToken,
                matchingOverload,
                null,
                functionParametersListingNode);

            model.Binder.BindFunctionInvocationNode(functionInvocationNode);

            if (SyntaxKind.StatementDelimiterToken == model.TokenWalker.Current.SyntaxKind)
            {
                _ = model.TokenWalker.Consume();
                model.CurrentCodeBlockBuilder.ChildList.Add(functionInvocationNode);
            }
            else
            {
                model.SyntaxStack.Push(functionInvocationNode);
            }
        }
    }

    /// <summary>Use this method for function invocation, whereas <see cref="HandleFunctionArguments"/> should be used for function definition.</summary>
    public static void HandleFunctionParameters(ParserModel model)
    {
        var openParenthesisToken = (OpenParenthesisToken)model.SyntaxStack.Pop();

        if (SyntaxKind.CloseParenthesisToken == model.TokenWalker.Current.SyntaxKind)
        {
            model.SyntaxStack.Push(new FunctionParametersListingNode(
                openParenthesisToken,
                ImmutableArray<FunctionParameterEntryNode>.Empty,
                (CloseParenthesisToken)model.TokenWalker.Consume()));

            return;
        }

        var mutableFunctionParametersListing = new List<FunctionParameterEntryNode>();

        while (true)
        {
            var hasOutKeyword = false;
            var hasInKeyword = false;
            var hasRefKeyword = false;

            // Check for keywords: { 'out', 'in', 'ref', }
            {
                // TODO: Erroneously putting an assortment of the keywords: { 'out', 'in', 'ref', }
                if (SyntaxKind.OutTokenKeyword == model.TokenWalker.Current.SyntaxKind)
                {
                    _ = model.TokenWalker.Consume();
                    hasOutKeyword = true;

                    if (model.TokenWalker.Peek(1).SyntaxKind == SyntaxKind.IdentifierToken)
                    {
                        var outVariableTypeClause = model.TokenWalker.MatchTypeClauseNode(model);
                        var outVariableIdentifier = (IdentifierToken)model.TokenWalker.Peek(0);

                        model.SyntaxStack.Push(outVariableTypeClause);
                        model.SyntaxStack.Push(outVariableIdentifier);

                        ParseVariables.HandleVariableDeclaration(
                            VariableKind.Local,
                            model);
                    }
                }
                else if (SyntaxKind.InTokenKeyword == model.TokenWalker.Current.SyntaxKind)
                {
                    _ = model.TokenWalker.Consume();
                    hasInKeyword = true;
                }
                else if (SyntaxKind.RefTokenKeyword == model.TokenWalker.Current.SyntaxKind)
                {
                    _ = model.TokenWalker.Consume();
                    hasRefKeyword = true;
                }
            }

            IExpressionNode expression;

            if (SyntaxKind.IdentifierToken == model.TokenWalker.Current.SyntaxKind)
            {
                var variableIdentifierToken = (IdentifierToken)model.TokenWalker.Consume();

                if (!model.Binder.TryGetVariableDeclarationHierarchically(
                        variableIdentifierToken.TextSpan.GetText(),
                        out var variableDeclarationNode)
                    || variableDeclarationNode is null)
                {
                    variableDeclarationNode = new(
                        Facts.CSharpFacts.Types.Void.ToTypeClause(),
                        variableIdentifierToken,
                        VariableKind.Local,
                        false)
                    {
                        IsFabricated = true
                    };

                    model.Binder.BindVariableDeclarationStatementNode(variableDeclarationNode);
                }

                var variableReferenceNode = new VariableReferenceNode(
                    variableIdentifierToken,
                    variableDeclarationNode);

                variableReferenceNode = model.Binder.BindVariableReferenceNode(variableReferenceNode);
                expression = variableReferenceNode;
            }
            else
            {
                ParseOthers.HandleExpression(
                    null,
                    null,
                    null,
                    null,
                    null,
                    new[]
                    {
                        new ExpressionDelimiter(null, SyntaxKind.CommaToken, null, null),
                        new ExpressionDelimiter(
                            SyntaxKind.OpenParenthesisToken,
                            SyntaxKind.CloseParenthesisToken,
                            null,
                            null)
                    },
                    model);

                expression = (IExpressionNode)model.SyntaxStack.Pop();
            }

            var functionParameterEntryNode = new FunctionParameterEntryNode(
                expression,
                hasOutKeyword,
                hasInKeyword,
                hasRefKeyword);

            mutableFunctionParametersListing.Add(functionParameterEntryNode);

            if (expression.IsFabricated && SyntaxKind.CommaToken != model.TokenWalker.Current.SyntaxKind)
            {
                break;
            }

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

        var closeParenthesisToken = (CloseParenthesisToken)model.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);

        model.SyntaxStack.Push(new FunctionParametersListingNode(
            openParenthesisToken,
            mutableFunctionParametersListing.ToImmutableArray(),
            closeParenthesisToken));
    }

    /// <summary>Use this method for function definition, whereas <see cref="HandleFunctionParameters"/> should be used for function invocation.</summary>
    public static void HandleFunctionArguments(ParserModel model)
    {
        var openParenthesisToken = (OpenParenthesisToken)model.SyntaxStack.Pop();

        if (SyntaxKind.CloseParenthesisToken == model.TokenWalker.Peek(0).SyntaxKind)
        {
            model.SyntaxStack.Push(new FunctionArgumentsListingNode(
                openParenthesisToken,
                ImmutableArray<FunctionArgumentEntryNode>.Empty,
                (CloseParenthesisToken)model.TokenWalker.Consume()));

            return;
        }

        var mutableFunctionArgumentListing = new List<FunctionArgumentEntryNode>();

        while (true)
        {
            var hasOutKeyword = false;
            var hasInKeyword = false;
            var hasRefKeyword = false;

            // Check for keywords: { 'out', 'in', 'ref', }
            {
                // TODO: Erroneously putting an assortment of the keywords: { 'out', 'in', 'ref', }
                if (SyntaxKind.OutTokenKeyword == model.TokenWalker.Current.SyntaxKind)
                {
                    _ = model.TokenWalker.Consume();
                    hasOutKeyword = true;
                }
                else if (SyntaxKind.InTokenKeyword == model.TokenWalker.Current.SyntaxKind)
                {
                    _ = model.TokenWalker.Consume();
                    hasInKeyword = true;
                }
                else if (SyntaxKind.RefTokenKeyword == model.TokenWalker.Current.SyntaxKind)
                {
                    _ = model.TokenWalker.Consume();
                    hasRefKeyword = true;
                }
            }

            // TypeClause
            var typeClauseNode = model.TokenWalker.MatchTypeClauseNode(model);

            if (typeClauseNode.IsFabricated)
                break;

            // Identifier
            var variableIdentifierToken = (IdentifierToken)model.TokenWalker.Match(SyntaxKind.IdentifierToken);

            if (variableIdentifierToken.IsFabricated)
                break;

            var variableDeclarationStatementNode = new VariableDeclarationNode(
                typeClauseNode,
                variableIdentifierToken,
                VariableKind.Local,
                false
            );

            model.Binder.BindVariableDeclarationStatementNode(variableDeclarationStatementNode);

            var functionArgumentEntryNode = new FunctionArgumentEntryNode(
                variableDeclarationStatementNode,
                false,
                hasOutKeyword,
                hasInKeyword,
                hasRefKeyword);

            if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
            {
                var equalsToken = (EqualsToken)model.TokenWalker.Consume();

                var compileTimeConstantAbleSyntaxKinds = new[]
                {
                    SyntaxKind.StringLiteralToken,
                    SyntaxKind.NumericLiteralToken,
                };

                var compileTimeConstantToken = model.TokenWalker.MatchRange(
                    compileTimeConstantAbleSyntaxKinds,
                    SyntaxKind.BadToken);

                functionArgumentEntryNode = model.Binder.BindFunctionOptionalArgument(
                    functionArgumentEntryNode,
                    compileTimeConstantToken,
                    hasOutKeyword,
                    hasInKeyword,
                    hasRefKeyword);
            }

            mutableFunctionArgumentListing.Add(functionArgumentEntryNode);

            if (model.TokenWalker.Current.SyntaxKind == SyntaxKind.CommaToken)
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

        var closeParenthesisToken = (CloseParenthesisToken)model.TokenWalker.Match(SyntaxKind.CloseParenthesisToken);

        model.SyntaxStack.Push(new FunctionArgumentsListingNode(
            openParenthesisToken,
            mutableFunctionArgumentListing.ToImmutableArray(),
            closeParenthesisToken));
    }
}
