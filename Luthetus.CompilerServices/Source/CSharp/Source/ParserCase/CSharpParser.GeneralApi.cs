using Luthetus.CompilerServices.Lang.CSharp.BinderCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Statement;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Enums;

namespace Luthetus.CompilerServices.Lang.CSharp.ParserCase;

public partial class CSharpParser : IParser
{
    /// <summary>
    /// Open ended
    /// </summary>
    private class GeneralApi
    {
        private readonly CSharpParser _parser;

        public GeneralApi(CSharpParser cSharpParser)
        {
            _parser = cSharpParser;
        }

        /// <summary>TODO: I don't like this <see cref="TokenWalker"/> property. It points to a private field on a different object. But without this property things are incredibly verbose. I need to remember to come back to this and change how I get access to the object because this doesn't feel right.</summary>
        public TokenWalker TokenWalker => _parser._tokenWalker;

        /// <summary>TODO: I don't like this <see cref="TokenWalker"/> property. It points to a private field on a different object. But without this property things are incredibly verbose. I need to remember to come back to this and change how I get access to the object because this doesn't feel right.</summary>
        public CSharpBinder Binder => _parser.Binder;

        /// <summary>TODO: I don't like this <see cref="Utility"/> property. It points to a private field on a different object. But without this property things are incredibly verbose. I need to remember to come back to this and change how I get access to the object because this doesn't feel right.</summary>
        public UtilityApi Utility => _parser._utility;

        /// <summary>TODO: I don't like this <see cref="DiagnosticBag"/> property. It points to a private field on a different object. But without this property things are incredibly verbose. I need to remember to come back to this and change how I get access to the object because this doesn't feel right.</summary>
        public LuthetusDiagnosticBag DiagnosticBag => _parser._diagnosticBag;

        /// <summary>TODO: I don't like this <see cref="Specific"/> property. It points to a private field on a different object. But without this property things are incredibly verbose. I need to remember to come back to this and change how I get access to the object because this doesn't feel right.</summary>
        public SpecificApi Specific => _parser._specific;

        /// <summary>TODO: I don't like this <see cref="ExpressionStack"/> property. It points to a private field on a different object. But without this property things are incredibly verbose. I need to remember to come back to this and change how I get access to the object because this doesn't feel right.</summary>
        public Stack<ISyntax> ExpressionStack => _parser._expressionStack;

        /// <summary>TODO: I don't like this <see cref="NodeRecent"/> property. It points to a private field on a different object. But without this property things are incredibly verbose. I need to remember to come back to this and change how I get access to the object because this doesn't feel right.</summary>
        public ISyntaxNode? NodeRecent
        {
            get => _parser._nodeRecent;
            set => _parser._nodeRecent = value;
        }
        
        /// <summary>TODO: I don't like this <see cref="NodeRecent"/> property. It points to a private field on a different object. But without this property things are incredibly verbose. I need to remember to come back to this and change how I get access to the object because this doesn't feel right.</summary>
        public CodeBlockBuilder CurrentCodeBlockBuilder
        {
            get => _parser._currentCodeBlockBuilder;
            set => _parser._currentCodeBlockBuilder = value;
        }

        public void ParseNumericLiteralToken(NumericLiteralToken numericLiteralToken)
        {
            TokenWalker.Backtrack();

            var completeExpression = Specific.HandleExpression(
                null,
                null,
                null,
                null,
                null,
                null);

            CurrentCodeBlockBuilder.ChildList.Add(completeExpression);
        }

        public void ParseStringLiteralToken(StringLiteralToken stringLiteralToken)
        {
            TokenWalker.Backtrack();

            var completeExpression = Specific.HandleExpression(
                null,
                null,
                null,
                null,
                null,
                null);

            CurrentCodeBlockBuilder.ChildList.Add(completeExpression);
        }

        public IStatementNode ParsePreprocessorDirectiveToken(PreprocessorDirectiveToken preprocessorDirectiveToken)
        {
            var consumedToken = TokenWalker.Consume();

            if (consumedToken.SyntaxKind == SyntaxKind.LibraryReferenceToken)
            {
                var preprocessorLibraryReferenceStatement = new PreprocessorLibraryReferenceStatementNode(
                    preprocessorDirectiveToken,
                    consumedToken);

                CurrentCodeBlockBuilder.ChildList.Add(preprocessorLibraryReferenceStatement);

                return preprocessorLibraryReferenceStatement;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void ParseIdentifierToken(IdentifierToken identifierToken)
        {
            if (NodeRecent is not null && NodeRecent.SyntaxKind == SyntaxKind.AmbiguousIdentifierNode)
            {
                var identifierReferenceNode = (AmbiguousIdentifierNode)NodeRecent;

                var expectingTypeClause = false;

                if (TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
                    expectingTypeClause = true;

                if (TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken)
                    expectingTypeClause = true;

                if (TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken ||
                    TokenWalker.Current.SyntaxKind == SyntaxKind.StatementDelimiterToken)
                {
                    expectingTypeClause = true;
                }

                if (expectingTypeClause)
                {
                    if (!Binder.TryGetTypeDefinitionHierarchically(
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

                        Binder.BindTypeDefinitionNode(fabricateTypeDefinition);

                        typeDefinitionNode = fabricateTypeDefinition;
                    }

                    NodeRecent = typeDefinitionNode.ToTypeClause();
                }
            }

            if (NodeRecent is not null && NodeRecent.SyntaxKind == SyntaxKind.TypeClauseNode)
            {
                GenericArgumentsListingNode? genericArgumentsListingNode = null;

                if (SyntaxKind.OpenAngleBracketToken == TokenWalker.Current.SyntaxKind)
                {
                    var openAngleBracketToken = (OpenAngleBracketToken)TokenWalker.Consume();
                    genericArgumentsListingNode = Specific.HandleGenericArguments(openAngleBracketToken);
                }

                if (TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken)
                {
                    Specific.HandleFunctionDefinition(
                        (TypeClauseNode)NodeRecent,
                        identifierToken,
                        genericArgumentsListingNode);

                    return;
                }
                else if (TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken ||
                         TokenWalker.Current.SyntaxKind == SyntaxKind.StatementDelimiterToken)
                {
                    if (TokenWalker.Next.SyntaxKind == SyntaxKind.CloseAngleBracketToken)
                    {
                        Specific.HandleVariableDeclaration(
                            (TypeClauseNode)NodeRecent,
                            identifierToken,
                            VariableKind.Property);

                        return;
                    }
                    else
                    {
                        var variableKind = VariableKind.Local;

                        if (CurrentCodeBlockBuilder.CodeBlockOwner is TypeDefinitionNode)
                            variableKind = VariableKind.Field;

                        Specific.HandleVariableDeclaration(
                            (TypeClauseNode)NodeRecent,
                            identifierToken,
                            variableKind);

                        return;
                    }
                }
                else if (TokenWalker.Current.SyntaxKind == SyntaxKind.OpenBraceToken)
                {
                    Specific.HandleVariableDeclaration(
                        (TypeClauseNode)NodeRecent,
                        identifierToken,
                        VariableKind.Property);

                    return;
                }
            }
            else if (TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken ||
                    TokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
            {
                if (TokenWalker.Current.SyntaxKind == SyntaxKind.OpenParenthesisToken &&
                    CurrentCodeBlockBuilder.CodeBlockOwner is not null &&
                    CurrentCodeBlockBuilder.CodeBlockOwner.SyntaxKind == SyntaxKind.TypeDefinitionNode)
                {
                    // TODO: Don't repeat an if statement that checks if current syntax token is OpenParenthesisToken
                    Specific.HandleConstructorDefinition(identifierToken);
                    return;
                }

                if (NodeRecent is not null &&
                    NodeRecent.SyntaxKind == SyntaxKind.AmbiguousIdentifierNode)
                {
                    var identifierReferenceNode = (AmbiguousIdentifierNode)NodeRecent;

                    // The unknown identifier reference can now be understood to be the return Type of a function.
                    var typeClauseNode = new TypeClauseNode(identifierReferenceNode.IdentifierToken, null, null);
                    NodeRecent = typeClauseNode;

                    // Re-invoke ParseIdentifierToken now that _cSharpParser._nodeRecent is known to be a Type identifier
                    // and that identifierToken is a function identifier
                    {
                        ParseIdentifierToken(identifierToken);
                        return;
                    }
                }

                // BUG: List<int> thinks 'List' is a generic method (2024-01-17)
                {
                }

                Specific.HandleFunctionInvocation(identifierToken);
            }
            else if (TokenWalker.Current.SyntaxKind == SyntaxKind.EqualsToken)
            {
                Specific.HandleVariableAssignment(identifierToken);
                return;
            }
            else
            {
                var text = identifierToken.TextSpan.GetText();

                if (Binder.BoundNamespaceStatementNodes.TryGetValue(text, out var boundNamespaceStatementNode) &&
                    boundNamespaceStatementNode is not null)
                {
                    Specific.HandleNamespaceReference(identifierToken, boundNamespaceStatementNode);
                    return;
                }
                else
                {
                    if (Binder.TryGetVariableDeclarationHierarchically(text, out var variableDeclarationStatementNode) &&
                        variableDeclarationStatementNode is not null)
                    {
                        Specific.HandleVariableReference(identifierToken, variableDeclarationStatementNode);
                        return;
                    }
                    else
                    {
                        // 'undeclared-variable reference' OR 'static class identifier'

                        if (Binder.TryGetTypeDefinitionHierarchically(text, out var typeDefinitionNode) &&
                            typeDefinitionNode is not null)
                        {
                            Specific.HandleStaticClassIdentifier(identifierToken);
                            return;
                        }
                        else
                        {
                            Specific.HandleUndefinedTypeOrNamespaceReference(identifierToken);
                            return;
                        }
                    }
                }
            }
        }

        public void ParsePlusToken(PlusToken plusToken)
        {
            var localNodeRecent = NodeRecent;

            if (localNodeRecent is not IExpressionNode leftExpressionNode)
                throw new NotImplementedException();

            var rightExpressionNode = Specific.HandleExpression(
                null,
                null,
                null,
                null,
                null,
                null);

            var binaryOperatorNode = Binder.BindBinaryOperatorNode(
                leftExpressionNode,
                plusToken,
                rightExpressionNode);

            var binaryExpressionNode = new BinaryExpressionNode(
                leftExpressionNode,
                binaryOperatorNode,
                rightExpressionNode);

            NodeRecent = binaryExpressionNode;
        }
        
        public void ParsePlusPlusToken(PlusPlusToken plusPlusToken)
        {
            if (NodeRecent is VariableReferenceNode variableReferenceNode)
            {
                var unaryOperatorNode = new UnaryOperatorNode(
                    variableReferenceNode.ResultTypeClauseNode,
                    plusPlusToken,
                    variableReferenceNode.ResultTypeClauseNode);

                var unaryExpressionNode = new UnaryExpressionNode(
                    variableReferenceNode,
                    unaryOperatorNode);

                CurrentCodeBlockBuilder.ChildList.Add(unaryExpressionNode);
            }
        }

        public void ParseMinusToken(MinusToken minusToken)
        {
            Specific.HandleExpression(
                null,
                null,
                null,
                null,
                null,
                null);
        }

        public void ParseStarToken(StarToken starToken)
        {
            Specific.HandleExpression(
                null,
                null,
                null,
                null,
                null,
                null);
        }

        public void ParseDollarSignToken(DollarSignToken dollarSignToken)
        {
            TokenWalker.Backtrack();

            var completeExpression = Specific.HandleExpression(
                null,
                null,
                null,
                null,
                null,
                null);

            CurrentCodeBlockBuilder.ChildList.Add(completeExpression);
        }

        public void ParseColonToken(ColonToken colonToken)
        {
            if (NodeRecent is not null && NodeRecent.SyntaxKind == SyntaxKind.TypeDefinitionNode)
            {
                var typeDefinitionNode = (TypeDefinitionNode)NodeRecent;

                var inheritedTypeClauseNode = Utility.MatchTypeClause();

                Binder.BindTypeClauseNode(inheritedTypeClauseNode);

                NodeRecent = new TypeDefinitionNode(
                    typeDefinitionNode.TypeIdentifier,
                    typeDefinitionNode.ValueType,
                    typeDefinitionNode.GenericArgumentsListingNode,
                    inheritedTypeClauseNode,
                    typeDefinitionNode.TypeBodyCodeBlockNode);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void ParseOpenBraceToken(OpenBraceToken openBraceToken)
        {
            var closureCurrentCodeBlockBuilder = CurrentCodeBlockBuilder;
            ISyntaxNode? nextCodeBlockOwner = null;
            TypeClauseNode? scopeReturnTypeClauseNode = null;

            if (NodeRecent is not null && NodeRecent.SyntaxKind == SyntaxKind.NamespaceStatementNode)
            {
                var boundNamespaceStatementNode = (NamespaceStatementNode)NodeRecent;
                nextCodeBlockOwner = boundNamespaceStatementNode;

                _parser._finalizeCodeBlockNodeActionStack.Push(codeBlockNode =>
                {
                    boundNamespaceStatementNode = Binder.RegisterBoundNamespaceEntryNode(
                        boundNamespaceStatementNode,
                        codeBlockNode);

                    closureCurrentCodeBlockBuilder.ChildList.Add(boundNamespaceStatementNode);
                });
            }
            else if (NodeRecent is not null && NodeRecent.SyntaxKind == SyntaxKind.TypeDefinitionNode)
            {
                var typeDefinitionNode = (TypeDefinitionNode)NodeRecent;
                nextCodeBlockOwner = typeDefinitionNode;

                _parser._finalizeCodeBlockNodeActionStack.Push(codeBlockNode =>
                {
                    typeDefinitionNode = new TypeDefinitionNode(
                        typeDefinitionNode.TypeIdentifier,
                        typeDefinitionNode.ValueType,
                        typeDefinitionNode.GenericArgumentsListingNode,
                        typeDefinitionNode.InheritedTypeClauseNode,
                        codeBlockNode);

                    Binder.BindTypeDefinitionNode(typeDefinitionNode, true);

                    closureCurrentCodeBlockBuilder.ChildList.Add(typeDefinitionNode);
                });
            }
            else if (NodeRecent is not null && NodeRecent.SyntaxKind == SyntaxKind.FunctionDefinitionNode)
            {
                var functionDefinitionNode = (FunctionDefinitionNode)NodeRecent;
                nextCodeBlockOwner = functionDefinitionNode;

                scopeReturnTypeClauseNode = functionDefinitionNode.ReturnTypeClauseNode;

                _parser._finalizeCodeBlockNodeActionStack.Push(codeBlockNode =>
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
            }
            else if (NodeRecent is not null && NodeRecent.SyntaxKind == SyntaxKind.ConstructorDefinitionNode)
            {
                var constructorDefinitionNode = (ConstructorDefinitionNode)NodeRecent;
                nextCodeBlockOwner = constructorDefinitionNode;

                scopeReturnTypeClauseNode = constructorDefinitionNode.ReturnTypeClauseNode;

                _parser._finalizeCodeBlockNodeActionStack.Push(codeBlockNode =>
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
            }
            else if (NodeRecent is not null && NodeRecent.SyntaxKind == SyntaxKind.IfStatementNode)
            {
                var ifStatementNode = (IfStatementNode)NodeRecent;
                nextCodeBlockOwner = ifStatementNode;

                _parser._finalizeCodeBlockNodeActionStack.Push(codeBlockNode =>
                {
                    ifStatementNode = new IfStatementNode(
                        ifStatementNode.KeywordToken,
                        ifStatementNode.ExpressionNode,
                        codeBlockNode);

                    closureCurrentCodeBlockBuilder.ChildList.Add(ifStatementNode);
                });
            }
            else
            {
                nextCodeBlockOwner = closureCurrentCodeBlockBuilder.CodeBlockOwner;

                _parser._finalizeCodeBlockNodeActionStack.Push(codeBlockNode =>
                {
                    closureCurrentCodeBlockBuilder.ChildList
                        .Add(codeBlockNode);
                });
            }

            Binder.RegisterBoundScope(
                scopeReturnTypeClauseNode,
                openBraceToken.TextSpan);

            if (NodeRecent is not null && NodeRecent.SyntaxKind == SyntaxKind.NamespaceStatementNode)
                Binder.AddNamespaceToCurrentScope((NamespaceStatementNode)NodeRecent);

            CurrentCodeBlockBuilder = new(CurrentCodeBlockBuilder, nextCodeBlockOwner);
        }

        public void ParseCloseBraceToken(CloseBraceToken closeBraceToken)
        {
            Binder.DisposeBoundScope(closeBraceToken.TextSpan);

            if (CurrentCodeBlockBuilder.Parent is not null && _parser._finalizeCodeBlockNodeActionStack.Any())
            {
                _parser._finalizeCodeBlockNodeActionStack
                    .Pop()
                    .Invoke(CurrentCodeBlockBuilder.Build());

                CurrentCodeBlockBuilder = CurrentCodeBlockBuilder.Parent;
            }
        }

        public void ParseOpenParenthesisToken(OpenParenthesisToken openParenthesisToken)
        {
            TokenWalker.Backtrack();

            var parenthesizedExpression = Specific.HandleExpression(
                null,
                null,
                null,
                null,
                null,
                null);

            // Example: (3 + 4) * 3
            //
            // Complete expression would be binary multiplication.
            var completeExpression = Specific.HandleExpression(
                parenthesizedExpression,
                parenthesizedExpression,
                null,
                null,
                null,
                null);

            CurrentCodeBlockBuilder.ChildList.Add(completeExpression);
        }

        public void ParseCloseParenthesisToken(CloseParenthesisToken closeParenthesisToken)
        {
        }

        public void ParseOpenAngleBracketToken(OpenAngleBracketToken openAngleBracketToken)
        {
            if (NodeRecent is not null)
            {
                if (NodeRecent.SyntaxKind == SyntaxKind.LiteralExpressionNode ||
                    NodeRecent.SyntaxKind == SyntaxKind.LiteralExpressionNode ||
                    NodeRecent.SyntaxKind == SyntaxKind.BinaryExpressionNode ||
                    /* Prefer the enum comparison. Will short circuit. This "is" cast is for fallback in case someone in the future adds for expression syntax kinds but does not update this if statement TODO: Check if node ends with "ExpressionNode"? */
                    NodeRecent is IExpressionNode)
                {
                    // Mathematical angle bracket
                    throw new NotImplementedException();
                }
                else
                {
                    // Generic Arguments
                    var boundGenericArguments = Specific.HandleGenericArguments(openAngleBracketToken);

                    if (NodeRecent.SyntaxKind == SyntaxKind.TypeDefinitionNode)
                    {
                        var typeDefinitionNode = (TypeDefinitionNode)NodeRecent;

                        // TODO: Fix boundClassDefinitionNode, it broke on (2023-07-26)
                        //
                        // _cSharpParser._nodeRecent = boundClassDefinitionNode with
                        // {
                        //     BoundGenericArgumentsNode = boundGenericArguments
                        // };
                    }
                }
            }
        }

        public void ParseCloseAngleBracketToken(CloseAngleBracketToken closeAngleBracketToken)
        {
            // if one: throw new NotImplementedException();
            // then: lambdas will no longer work. So I'm keeping this method empty.
        }

        public void ParseOpenSquareBracketToken(OpenSquareBracketToken openSquareBracketToken)
        {
            if (NodeRecent is not null)
            {
                if (NodeRecent.SyntaxKind == SyntaxKind.LiteralExpressionNode ||
                    NodeRecent.SyntaxKind == SyntaxKind.LiteralExpressionNode ||
                    NodeRecent.SyntaxKind == SyntaxKind.BinaryExpressionNode ||
                    /* Prefer the enum comparison. Will short circuit. This "is" cast is for fallback in case someone in the future adds for expression syntax kinds but does not update this if statement TODO: Check if node ends with "ExpressionNode"? */
                    NodeRecent is IExpressionNode)
                {
                    // Mathematical square bracket
                    throw new NotImplementedException();
                }
                else
                {
                    // Attribute
                    NodeRecent = Specific.HandleAttribute(openSquareBracketToken);
                }
            }
        }

        public void ParseCloseSquareBracketToken(CloseSquareBracketToken closeSquareBracketToken)
        {
            return;
        }

        public void ParseMemberAccessToken(MemberAccessToken memberAccessToken)
        {
            if (NodeRecent is null)
                throw new NotImplementedException($"_cSharpParser._handle.Handle the case where a {nameof(MemberAccessToken)} is used without a valid preceeding node.");

            switch (NodeRecent.SyntaxKind)
            {
                case SyntaxKind.VariableReferenceNode:
                    {
                        var variableReferenceNode = (VariableReferenceNode)NodeRecent;

                        if (variableReferenceNode.VariableDeclarationNode.IsFabricated)
                        {
                            // Undeclared variable, so the Type is unknown.
                        }

                        break;
                    }
            }
        }

        public void ParseStatementDelimiterToken(StatementDelimiterToken statementDelimiterToken)
        {
            if (NodeRecent is not null && NodeRecent.SyntaxKind == SyntaxKind.NamespaceStatementNode)
            {
                var closureCurrentCompilationUnitBuilder = CurrentCodeBlockBuilder;
                ISyntaxNode? nextCodeBlockOwner = null;
                TypeClauseNode? scopeReturnTypeClauseNode = null;

                var boundNamespaceStatementNode = (NamespaceStatementNode)NodeRecent;
                nextCodeBlockOwner = boundNamespaceStatementNode;

                _parser._finalizeNamespaceFileScopeCodeBlockNodeAction = codeBlockNode =>
                {
                    boundNamespaceStatementNode = Binder.RegisterBoundNamespaceEntryNode(
                        boundNamespaceStatementNode,
                        codeBlockNode);

                    closureCurrentCompilationUnitBuilder.ChildList.Add(boundNamespaceStatementNode);
                };

                Binder.RegisterBoundScope(
                    scopeReturnTypeClauseNode,
                    statementDelimiterToken.TextSpan);

                Binder.AddNamespaceToCurrentScope((NamespaceStatementNode)NodeRecent);

                CurrentCodeBlockBuilder = new(CurrentCodeBlockBuilder, nextCodeBlockOwner);
            }
        }

        public void ParseKeywordToken(KeywordToken keywordToken)
        {
            // 'return', 'if', 'get', etc...
            switch (keywordToken.SyntaxKind)
            {
                case SyntaxKind.AsTokenKeyword:
                    Specific.HandleAsTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.BaseTokenKeyword:
                    Specific.HandleBaseTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.BoolTokenKeyword:
                    Specific.HandleBoolTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.BreakTokenKeyword:
                    Specific.HandleBreakTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.ByteTokenKeyword:
                    Specific.HandleByteTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.CaseTokenKeyword:
                    Specific.HandleCaseTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.CatchTokenKeyword:
                    Specific.HandleCatchTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.CharTokenKeyword:
                    Specific.HandleCharTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.CheckedTokenKeyword:
                    Specific.HandleCheckedTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.ConstTokenKeyword:
                    Specific.HandleConstTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.ContinueTokenKeyword:
                    Specific.HandleContinueTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.DecimalTokenKeyword:
                    Specific.HandleDecimalTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.DefaultTokenKeyword:
                    Specific.HandleDefaultTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.DelegateTokenKeyword:
                    Specific.HandleDelegateTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.DoTokenKeyword:
                    Specific.HandleDoTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.DoubleTokenKeyword:
                    Specific.HandleDoubleTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.ElseTokenKeyword:
                    Specific.HandleElseTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.EnumTokenKeyword:
                    Specific.HandleEnumTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.EventTokenKeyword:
                    Specific.HandleEventTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.ExplicitTokenKeyword:
                    Specific.HandleExplicitTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.ExternTokenKeyword:
                    Specific.HandleExternTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.FalseTokenKeyword:
                    Specific.HandleFalseTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.FinallyTokenKeyword:
                    Specific.HandleFinallyTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.FixedTokenKeyword:
                    Specific.HandleFixedTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.FloatTokenKeyword:
                    Specific.HandleFloatTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.ForTokenKeyword:
                    Specific.HandleForTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.ForeachTokenKeyword:
                    Specific.HandleForeachTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.GotoTokenKeyword:
                    Specific.HandleGotoTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.ImplicitTokenKeyword:
                    Specific.HandleImplicitTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.InTokenKeyword:
                    Specific.HandleInTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.IntTokenKeyword:
                    Specific.HandleIntTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.IsTokenKeyword:
                    Specific.HandleIsTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.LockTokenKeyword:
                    Specific.HandleLockTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.LongTokenKeyword:
                    Specific.HandleLongTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.NullTokenKeyword:
                    Specific.HandleNullTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.ObjectTokenKeyword:
                    Specific.HandleObjectTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.OperatorTokenKeyword:
                    Specific.HandleOperatorTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.OutTokenKeyword:
                    Specific.HandleOutTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.ParamsTokenKeyword:
                    Specific.HandleParamsTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.ProtectedTokenKeyword:
                    Specific.HandleProtectedTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.ReadonlyTokenKeyword:
                    Specific.HandleReadonlyTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.RefTokenKeyword:
                    Specific.HandleRefTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.SbyteTokenKeyword:
                    Specific.HandleSbyteTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.ShortTokenKeyword:
                    Specific.HandleShortTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.SizeofTokenKeyword:
                    Specific.HandleSizeofTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.StackallocTokenKeyword:
                    Specific.HandleStackallocTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.StringTokenKeyword:
                    Specific.HandleStringTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.StructTokenKeyword:
                    Specific.HandleStructTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.SwitchTokenKeyword:
                    Specific.HandleSwitchTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.ThisTokenKeyword:
                    Specific.HandleThisTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.ThrowTokenKeyword:
                    Specific.HandleThrowTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.TrueTokenKeyword:
                    Specific.HandleTrueTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.TryTokenKeyword:
                    Specific.HandleTryTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.TypeofTokenKeyword:
                    Specific.HandleTypeofTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.UintTokenKeyword:
                    Specific.HandleUintTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.UlongTokenKeyword:
                    Specific.HandleUlongTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.UncheckedTokenKeyword:
                    Specific.HandleUncheckedTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.UnsafeTokenKeyword:
                    Specific.HandleUnsafeTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.UshortTokenKeyword:
                    Specific.HandleUshortTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.VoidTokenKeyword:
                    Specific.HandleVoidTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.VolatileTokenKeyword:
                    Specific.HandleVolatileTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.WhileTokenKeyword:
                    Specific.HandleWhileTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.UnrecognizedTokenKeyword:
                    Specific.HandleUnrecognizedTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.ReturnTokenKeyword:
                    Specific.HandleReturnTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.NamespaceTokenKeyword:
                    Specific.HandleNamespaceTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.ClassTokenKeyword:
                    Specific.HandleClassTokenKeyword();
                    break;
                case SyntaxKind.InterfaceTokenKeyword:
                    Specific.HandleInterfaceTokenKeyword();
                    break;
                case SyntaxKind.UsingTokenKeyword:
                    Specific.HandleUsingTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.PublicTokenKeyword:
                    Specific.HandlePublicTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.InternalTokenKeyword:
                    Specific.HandleInternalTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.PrivateTokenKeyword:
                    Specific.HandlePrivateTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.StaticTokenKeyword:
                    Specific.HandleStaticTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.OverrideTokenKeyword:
                    Specific.HandleOverrideTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.VirtualTokenKeyword:
                    Specific.HandleVirtualTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.AbstractTokenKeyword:
                    Specific.HandleAbstractTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.SealedTokenKeyword:
                    Specific.HandleSealedTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.IfTokenKeyword:
                    Specific.HandleIfTokenKeyword(keywordToken);
                    break;
                case SyntaxKind.NewTokenKeyword:
                    Specific.HandleNewTokenKeyword();
                    break;
                default:
                    Specific.HandleDefault(keywordToken);
                    break;
            }
        }

        public void ParseKeywordContextualToken(KeywordContextualToken contextualKeywordToken)
        {
            switch (contextualKeywordToken.SyntaxKind)
            {
                case SyntaxKind.VarTokenContextualKeyword:
                    Specific.HandleVarTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.PartialTokenContextualKeyword:
                    Specific.HandlePartialTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.AddTokenContextualKeyword:
                    Specific.HandleAddTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.AndTokenContextualKeyword:
                    Specific.HandleAndTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.AliasTokenContextualKeyword:
                    Specific.HandleAliasTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.AscendingTokenContextualKeyword:
                    Specific.HandleAscendingTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.ArgsTokenContextualKeyword:
                    Specific.HandleArgsTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.AsyncTokenContextualKeyword:
                    Specific.HandleAsyncTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.AwaitTokenContextualKeyword:
                    Specific.HandleAwaitTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.ByTokenContextualKeyword:
                    Specific.HandleByTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.DescendingTokenContextualKeyword:
                    Specific.HandleDescendingTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.DynamicTokenContextualKeyword:
                    Specific.HandleDynamicTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.EqualsTokenContextualKeyword:
                    Specific.HandleEqualsTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.FileTokenContextualKeyword:
                    Specific.HandleFileTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.FromTokenContextualKeyword:
                    Specific.HandleFromTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.GetTokenContextualKeyword:
                    Specific.HandleGetTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.GlobalTokenContextualKeyword:
                    Specific.HandleGlobalTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.GroupTokenContextualKeyword:
                    Specific.HandleGroupTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.InitTokenContextualKeyword:
                    Specific.HandleInitTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.IntoTokenContextualKeyword:
                    Specific.HandleIntoTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.JoinTokenContextualKeyword:
                    Specific.HandleJoinTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.LetTokenContextualKeyword:
                    Specific.HandleLetTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.ManagedTokenContextualKeyword:
                    Specific.HandleManagedTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.NameofTokenContextualKeyword:
                    Specific.HandleNameofTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.NintTokenContextualKeyword:
                    Specific.HandleNintTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.NotTokenContextualKeyword:
                    Specific.HandleNotTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.NotnullTokenContextualKeyword:
                    Specific.HandleNotnullTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.NuintTokenContextualKeyword:
                    Specific.HandleNuintTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.OnTokenContextualKeyword:
                    Specific.HandleOnTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.OrTokenContextualKeyword:
                    Specific.HandleOrTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.OrderbyTokenContextualKeyword:
                    Specific.HandleOrderbyTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.RecordTokenContextualKeyword:
                    Specific.HandleRecordTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.RemoveTokenContextualKeyword:
                    Specific.HandleRemoveTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.RequiredTokenContextualKeyword:
                    Specific.HandleRequiredTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.ScopedTokenContextualKeyword:
                    Specific.HandleScopedTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.SelectTokenContextualKeyword:
                    Specific.HandleSelectTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.SetTokenContextualKeyword:
                    Specific.HandleSetTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.UnmanagedTokenContextualKeyword:
                    Specific.HandleUnmanagedTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.ValueTokenContextualKeyword:
                    Specific.HandleValueTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.WhenTokenContextualKeyword:
                    Specific.HandleWhenTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.WhereTokenContextualKeyword:
                    Specific.HandleWhereTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.WithTokenContextualKeyword:
                    Specific.HandleWithTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.YieldTokenContextualKeyword:
                    Specific.HandleYieldTokenContextualKeyword(contextualKeywordToken);
                    break;
                case SyntaxKind.UnrecognizedTokenContextualKeyword:
                    Specific.HandleUnrecognizedTokenContextualKeyword(contextualKeywordToken);
                    break;
                default:
                    throw new NotImplementedException($"Implement the {contextualKeywordToken.SyntaxKind.ToString()} contextual keyword.");
            }
        }
    }
}