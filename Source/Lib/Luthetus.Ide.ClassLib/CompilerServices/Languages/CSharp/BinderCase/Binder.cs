using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Analysis;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.TextEditor.RazorLib.Analysis.GenericLexer.Decoration;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxNodes.Expression;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Symbols;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Expression;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.Facts;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.BinderCase;

public class Binder
{
    private readonly BoundScope _globalScope = CSharpLanguageFacts.Scope.GetInitialGlobalScope();
    private readonly Dictionary<string, BoundNamespaceStatementNode> _boundNamespaceStatementNodes = CSharpLanguageFacts.Namespaces.GetInitialBoundNamespaceStatementNodes();
    /// <summary>The key for _symbolDefinitions is calculated by <see cref="ISymbol.GetSymbolDefinitionId"/></summary>
    private readonly Dictionary<string, SymbolDefinition> _symbolDefinitions = new();
    private readonly LuthetusIdeDiagnosticBag _diagnosticBag = new();

    private List<BoundScope> _boundScopes = new();
    private BoundScope _currentScope;

    public Binder()
    {
        _currentScope = _globalScope;

        _boundScopes.Add(_globalScope);

        _boundScopes = _boundScopes
            .OrderBy(x => x.StartingIndexInclusive)
            .ToList();
    }

    public ResourceUri? CurrentResourceUri { get; set; }

    public ImmutableDictionary<string, BoundNamespaceStatementNode> BoundNamespaceStatementNodes => _boundNamespaceStatementNodes.ToImmutableDictionary();
    public ImmutableArray<ISymbol> Symbols => _symbolDefinitions.Values.SelectMany(x => x.SymbolReferences).Select(x => x.Symbol).ToImmutableArray();
    public Dictionary<string, SymbolDefinition> SymbolDefinitions => _symbolDefinitions;
    public ImmutableArray<BoundScope> BoundScopes => _boundScopes.ToImmutableArray();
    public ImmutableArray<TextEditorDiagnostic> Diagnostics => _diagnosticBag.ToImmutableArray();

    public BoundLiteralExpressionNode BindLiteralExpressionNode(
        LiteralExpressionNode literalExpressionNode)
    {
        var type = literalExpressionNode.LiteralSyntaxToken.SyntaxKind switch
        {
            SyntaxKind.NumericLiteralToken => typeof(int),
            SyntaxKind.StringLiteralToken => typeof(string),
            _ => throw new NotImplementedException(),
        };

        var boundLiteralExpressionNode = new BoundLiteralExpressionNode(
            literalExpressionNode.LiteralSyntaxToken,
            type);

        return boundLiteralExpressionNode;
    }

    public BoundBinaryOperatorNode BindBinaryOperatorNode(
        BoundLiteralExpressionNode leftBoundLiteralExpressionNode,
        ISyntaxToken operatorToken,
        BoundLiteralExpressionNode rightBoundLiteralExpressionNode)
    {
        if (leftBoundLiteralExpressionNode.ResultType == typeof(int) &&
            rightBoundLiteralExpressionNode.ResultType == typeof(int))
        {
            switch (operatorToken.SyntaxKind)
            {
                case SyntaxKind.PlusToken:
                    return new BoundBinaryOperatorNode(
                        leftBoundLiteralExpressionNode.ResultType,
                        operatorToken,
                        rightBoundLiteralExpressionNode.ResultType,
                        typeof(int));
            }
        }
        else if (leftBoundLiteralExpressionNode.ResultType == typeof(string) &&
            rightBoundLiteralExpressionNode.ResultType == typeof(string))
        {
            switch (operatorToken.SyntaxKind)
            {
                case SyntaxKind.PlusToken:
                    return new BoundBinaryOperatorNode(
                        leftBoundLiteralExpressionNode.ResultType,
                        operatorToken,
                        rightBoundLiteralExpressionNode.ResultType,
                        typeof(string));
            }
        }

        throw new NotImplementedException();
    }

    /// <summary>TODO: Construct a BoundStringInterpolationExpressionNode and identify the expressions within the string literal. For now I am just making the dollar sign the same color as a string literal.</summary>
    public void BindStringInterpolationExpression(
        DollarSignToken dollarSignToken)
    {
        AddSymbolReference(new StringInterpolationSymbol(dollarSignToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.StringLiteral,
        }));
    }

    /// <summary>If a class definition already exists then false will be returned.</summary>
    public bool TryBindClassDefinitionNode(
        ISyntaxToken typeClauseToken,
        BoundGenericArgumentsNode? boundGenericArgumentsNode,
        out BoundClassDefinitionNode boundClassDefinitionNode,
        bool shouldCreateTypeSymbolReference = true,
        bool shouldReportAlreadyDefinedType = true)
    {
        if (shouldCreateTypeSymbolReference &&
            typeClauseToken.SyntaxKind == SyntaxKind.IdentifierToken)
        {
            AddSymbolReference(new TypeSymbol(typeClauseToken.TextSpan with
            {
                DecorationByte = (byte)GenericDecorationKind.Type
            }));
        }

        boundClassDefinitionNode = new BoundClassDefinitionNode(
            typeClauseToken,
            typeof(void),
            null,
            null,
            null);

        var classIdentifier = typeClauseToken.TextSpan.GetText();

        if (typeClauseToken.SyntaxKind == SyntaxKind.IdentifierToken ||
            typeClauseToken.SyntaxKind == SyntaxKind.KeywordToken ||
            typeClauseToken.SyntaxKind == SyntaxKind.KeywordContextualToken)
        {
            if (TryGetClassDefinitionHierarchically(
                    typeClauseToken,
                    boundGenericArgumentsNode, 
                    out var nullableBoundClassDefinitionNode) &&
                nullableBoundClassDefinitionNode is not null)
            {
                if (shouldReportAlreadyDefinedType)
                {
                    _diagnosticBag.ReportAlreadyDefinedType(
                        typeClauseToken.TextSpan,
                        typeClauseToken.TextSpan.GetText());
                }

                if (typeClauseToken.SyntaxKind != SyntaxKind.KeywordToken &&
                    typeClauseToken.SyntaxKind != SyntaxKind.KeywordContextualToken)
                {
                    // Overwrite the entry if it isn't a keyword
                    _currentScope.ClassDefinitionMap[classIdentifier] = boundClassDefinitionNode;
                }
                else
                {
                    // Take the existing keyword type
                    boundClassDefinitionNode = nullableBoundClassDefinitionNode;
                }

                return false;
            }
        }

        boundClassDefinitionNode = boundClassDefinitionNode with
        {
            IsFabricated = true
        };

        _ = _currentScope.ClassDefinitionMap.TryAdd(
            classIdentifier,
            boundClassDefinitionNode);

        return true;
    }

    public BoundFunctionDefinitionNode BindFunctionDefinitionNode(
        BoundClassReferenceNode boundClassReferenceNode,
        IdentifierToken identifierToken,
        BoundFunctionArgumentsNode boundFunctionArguments,
        BoundGenericArgumentsNode? boundGenericArgumentsNode)
    {
        var functionIdentifier = identifierToken.TextSpan.GetText();

        if (_currentScope.FunctionDefinitionMap.TryGetValue(
            functionIdentifier,
            out var functionDefinitionNode))
        {
            // TODO: The function was already declared, so report a diagnostic?
            // TODO: The function was already declared, so check that the return types match?
            return functionDefinitionNode;
        }

        var boundFunctionDefinitionNode = new BoundFunctionDefinitionNode(
            boundClassReferenceNode,
            identifierToken,
            boundFunctionArguments,
            boundGenericArgumentsNode,
            null);

        var success = _currentScope.FunctionDefinitionMap.TryAdd(
            functionIdentifier,
            boundFunctionDefinitionNode);

        if (!success)
            _currentScope.FunctionDefinitionMap[functionIdentifier] = boundFunctionDefinitionNode;

        AddSymbolDefinition(new FunctionSymbol(identifierToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Function
        }));

        return boundFunctionDefinitionNode;
    }

    /// <summary>TODO: Validate that the returned bound expression node has the same result type as the enclosing scope.</summary>
    public BoundReturnStatementNode BindReturnStatementNode(
        KeywordToken keywordToken,
        IBoundExpressionNode boundExpressionNode)
    {
        _diagnosticBag.ReportReturnStatementsAreStillBeingImplemented(
                keywordToken.TextSpan);

        return new BoundReturnStatementNode(
            keywordToken,
            boundExpressionNode);
    }

    public BoundIfStatementNode BindIfStatementNode(
        KeywordToken ifKeywordToken,
        IBoundExpressionNode boundExpressionNode)
    {
        var boundIfStatementNode = new BoundIfStatementNode(
            ifKeywordToken,
            boundExpressionNode,
            null);

        return boundIfStatementNode;
    }

    public BoundNamespaceStatementNode BindNamespaceStatementNode(
        KeywordToken keywordToken,
        IdentifierToken identifierToken)
    {
        var namespaceIdentifier = identifierToken.TextSpan.GetText();

        if (_boundNamespaceStatementNodes.TryGetValue(
                namespaceIdentifier, 
                out var boundNamespaceStatementNode))
        {
            return boundNamespaceStatementNode;
        }
        else
        {
            boundNamespaceStatementNode = new BoundNamespaceStatementNode(
                keywordToken,
                identifierToken,
                ImmutableArray<BoundNamespaceEntryNode>.Empty);

            var success = _boundNamespaceStatementNodes.TryAdd(
                namespaceIdentifier,
                boundNamespaceStatementNode);

            if (!success)
                _boundNamespaceStatementNodes[namespaceIdentifier] = boundNamespaceStatementNode;

            return boundNamespaceStatementNode;
        }
    }
    
    public BoundNamespaceStatementNode RegisterBoundNamespaceEntryNode(
        BoundNamespaceStatementNode inBoundNamespaceStatementNode,
        CompilationUnit compilationUnit)
    {
        var namespaceIdentifier = inBoundNamespaceStatementNode
            .IdentifierToken.TextSpan.GetText();

        if (_boundNamespaceStatementNodes.TryGetValue(
                namespaceIdentifier, 
                out var existingBoundNamespaceStatementNode))
        {
            var boundNamespaceEntryNode = new BoundNamespaceEntryNode(
                inBoundNamespaceStatementNode.IdentifierToken.TextSpan.ResourceUri,
                compilationUnit);

            var outChildren = existingBoundNamespaceStatementNode.Children
                .Add(boundNamespaceEntryNode)
                .Select(x => (BoundNamespaceEntryNode)x)
                .ToImmutableArray();

            var outBoundNamespaceStatementNode = new BoundNamespaceStatementNode(
                existingBoundNamespaceStatementNode.KeywordToken,
                existingBoundNamespaceStatementNode.IdentifierToken,
                outChildren);

            _boundNamespaceStatementNodes[namespaceIdentifier] = outBoundNamespaceStatementNode;

            return outBoundNamespaceStatementNode;
        }
        else
        {
            throw new NotImplementedException(
                $"The {nameof(inBoundNamespaceStatementNode)}" +
                $" was not found in the {nameof(_boundNamespaceStatementNodes)} dictionary.");
        }
    }
    
    public BoundConstructorInvocationNode BindConstructorInvocationNode(
        KeywordToken keywordToken,
        BoundClassReferenceNode? boundClassReferenceNode,
        BoundFunctionParametersNode? boundFunctionParametersNode,
        BoundObjectInitializationNode? boundObjectInitializationNode)
    {
        if (boundClassReferenceNode is not null)
        {
            AddSymbolReference(new TypeSymbol(boundClassReferenceNode.TypeClauseToken.TextSpan with
            {
                DecorationByte = (byte)GenericDecorationKind.Type
            }));
        }

        return new BoundConstructorInvocationNode(
            keywordToken,
            boundClassReferenceNode,
            boundFunctionParametersNode,
            boundObjectInitializationNode);
    }
    
    public BoundInheritanceStatementNode BindInheritanceStatementNode(
        BoundClassReferenceNode boundClassReferenceNode)
    {
        AddSymbolReference(new TypeSymbol(boundClassReferenceNode.TypeClauseToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Type
        }));

        var boundInheritanceStatementNode = new BoundInheritanceStatementNode(
            boundClassReferenceNode);

        // Do not report undefined class here. This is because by means of having a BoundClassReferenceNode the binder would've already reported this.

        return boundInheritanceStatementNode;
    }

    public BoundVariableDeclarationStatementNode BindVariableDeclarationNode(
        BoundClassReferenceNode boundClassReferenceNode,
        IdentifierToken identifierToken)
    {
        AddSymbolDefinition(new VariableSymbol(identifierToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Variable
        }));
        
        var text = identifierToken.TextSpan.GetText();

        if (_currentScope.VariableDeclarationMap.TryGetValue(
            text,
            out var variableDeclarationNode))
        {
            // TODO: The variable was already declared, so report a diagnostic?
            // TODO: The variable was already declared, so check that the return types match?
            return variableDeclarationNode;
        }

        var boundVariableDeclarationStatementNode = new BoundVariableDeclarationStatementNode(
            boundClassReferenceNode,
            identifierToken,
            false);

        var success = _currentScope.VariableDeclarationMap.TryAdd(
            text,
            boundVariableDeclarationStatementNode);

        if (!success)
            _currentScope.VariableDeclarationMap[text] = boundVariableDeclarationStatementNode;

        return boundVariableDeclarationStatementNode;
    }

    public BoundVariableAssignmentStatementNode BindVariableAssignmentNode(
        IdentifierToken identifierToken,
        IBoundExpressionNode boundExpressionNode)
    {
        AddSymbolReference(new VariableSymbol(identifierToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Variable
        }));

        var text = identifierToken.TextSpan.GetText();

        if (TryGetVariableHierarchically(
                text,
                out var variableDeclarationNode) &&
            variableDeclarationNode is not null)
        {
            if (variableDeclarationNode.IsInitialized)
                return new(identifierToken, boundExpressionNode);

            variableDeclarationNode = variableDeclarationNode with
            {
                IsInitialized = true
            };

            _currentScope.VariableDeclarationMap[text] =
                variableDeclarationNode;

            return new(identifierToken, boundExpressionNode);
        }
        else
        {
            // TODO: The variable was not yet declared, so report a diagnostic?
            return new(identifierToken, boundExpressionNode)
            {
                IsFabricated = true
            };
        }
    }

    public BoundVariableDeclarationStatementNode BindPropertyDeclarationNode(
        BoundClassReferenceNode boundClassReferenceNode,
        IdentifierToken identifierToken)
    {
        AddSymbolDefinition(new PropertySymbol(identifierToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Property
        }));

        var text = identifierToken.TextSpan.GetText();

        if (_currentScope.VariableDeclarationMap.TryGetValue(
            text,
            out var propertyDeclarationNode))
        {
            // TODO: The property was already declared, so report a diagnostic?
            // TODO: The property was already declared, so check that the return types match?
            return propertyDeclarationNode;
        }

        var boundVariableDeclarationStatementNode = new BoundVariableDeclarationStatementNode(
            boundClassReferenceNode,
            identifierToken,
            false);

        var success = _currentScope.VariableDeclarationMap.TryAdd(
            text,
            boundVariableDeclarationStatementNode);

        if (!success)
            _currentScope.VariableDeclarationMap[text] = boundVariableDeclarationStatementNode;

        return boundVariableDeclarationStatementNode;
    }

    public BoundIdentifierReferenceNode BindIdentifierReferenceNode(
        IdentifierToken identifierToken)
    {
        var text = identifierToken.TextSpan.GetText();

        if (TryGetVariableHierarchically(
                text,
                out var variableDeclarationNode) &&
            variableDeclarationNode is not null)
        {
            AddSymbolReference(new VariableSymbol(identifierToken.TextSpan with
            {
                DecorationByte = (byte)GenericDecorationKind.Variable
            }));

            return new BoundIdentifierReferenceNode(
                identifierToken,
                variableDeclarationNode.BoundClassReferenceNode);
        }
        else if (TryGetClassReferenceHierarchically(
                     identifierToken,
                     null,
                     out var boundClassReferenceNode) &&
                 boundClassReferenceNode is not null)
        {
            AddSymbolReference(new TypeSymbol(identifierToken.TextSpan with
            {
                DecorationByte = (byte)GenericDecorationKind.Type
            }));

            return new BoundIdentifierReferenceNode(
                identifierToken,
                boundClassReferenceNode);
        }
        else if (TryGetBoundFunctionDefinitionNodeHierarchically(
                     text,
                     out var boundFunctionDefinitionNode) &&
                 boundFunctionDefinitionNode is not null)
        {
            // TODO: Would this conditional branch be for method groups? @onclick="MethodName"

            AddSymbolReference(new FunctionSymbol(identifierToken.TextSpan with
            {
                DecorationByte = (byte)GenericDecorationKind.Function
            }));

            return new BoundIdentifierReferenceNode(
                identifierToken,
                // TODO: Null is should not be passed in here
                null);
        }
        else
        {
            // TODO: The identifier was not found, so report a diagnostic?
            return new BoundIdentifierReferenceNode(
                identifierToken,
                // TODO: Null is should not be passed in here
                null);
        }
    }

    public BoundFunctionInvocationNode? BindFunctionInvocationNode(
        IdentifierToken identifierToken,
        BoundFunctionParametersNode boundFunctionParametersNode,
        BoundGenericArgumentsNode? genericArguments)
    {
        AddSymbolReference(new FunctionSymbol(identifierToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Function
        }));

        var text = identifierToken.TextSpan.GetText();

        if (TryGetBoundFunctionDefinitionNodeHierarchically(
                text,
                out var boundFunctionDefinitionNode) &&
            boundFunctionDefinitionNode is not null)
        {
            return new(identifierToken, boundFunctionParametersNode, genericArguments);
        }
        else
        {
            _diagnosticBag.ReportUndefinedFunction(
                identifierToken.TextSpan,
                text);

            return new(identifierToken, boundFunctionParametersNode, genericArguments)
            {
                IsFabricated = true
            };
        }
    }

    public BoundUsingStatementNode BindUsingStatementNode(
        KeywordToken usingKeywordToken,
        IdentifierToken namespaceIdentifierToken)
    {
        var namespaceText = namespaceIdentifierToken.TextSpan.GetText();

        if (_boundNamespaceStatementNodes.TryGetValue(
                namespaceText,
                out var boundNamespaceStatementNode))
        {
            AddNamespaceToCurrentScope(boundNamespaceStatementNode);
        }

        return new BoundUsingStatementNode(
            usingKeywordToken,
            namespaceIdentifierToken);
    }

    /// <summary>TODO: Correctly implement this method. For now going to skip until the attribute closing square bracket.</summary>
    public BoundAttributeNode BindAttributeNode(
        OpenSquareBracketToken openSquareBracketToken,
        CloseSquareBracketToken closeSquareBracketToken)
    {
        AddSymbolReference(new TypeSymbol(openSquareBracketToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Type,
            EndingIndexExclusive = closeSquareBracketToken.TextSpan.EndingIndexExclusive
        }));

        return new BoundAttributeNode(
            openSquareBracketToken,
            closeSquareBracketToken);
    }
    
    public BoundGenericArgumentsNode BindGenericArguments(
        OpenAngleBracketToken openAngleBracketToken,
        List<ISyntaxToken> genericArgumentListing,
        CloseAngleBracketToken closeAngleBracketToken)
    {
        var boundGenericArgumentListing = new List<ISyntax>();

        for (var i = 0; i < genericArgumentListing.Count; i++)
        {
            ISyntax syntax;

            if (i % 2 == 1)
            {
                // CommaToken
                syntax = genericArgumentListing[i];
            }
            else
            {
                // Generic Arguments should not be declared?
                var typeClauseToken = genericArgumentListing[i];

                AddSymbolReference(new TypeSymbol(typeClauseToken.TextSpan with
                {
                    DecorationByte = (byte)GenericDecorationKind.Type
                }));

                syntax = typeClauseToken;
            }

            boundGenericArgumentListing.Add(syntax);
        }

        return new BoundGenericArgumentsNode(
            openAngleBracketToken,
            boundGenericArgumentListing,
            closeAngleBracketToken);
    }

    /// <summary>Use this method for function definition, whereas <see cref="BindFunctionParameters"/> should be used for function invocation.</summary>
    public BoundFunctionArgumentsNode BindFunctionArguments(
        OpenParenthesisToken openParenthesisToken,
        List<ISyntaxToken> functionArgumentListing,
        CloseParenthesisToken closeParenthesisToken)
    {
        List<ISyntax> boundFunctionArguments = new();

        // Alternate between reading TypeClause (null), ArgumentIdentifier (true), and a Comma (false)
        bool? canReadComma = null;

        // TODO: Don't make this null
        BoundClassReferenceNode boundClassReferenceNode = null;

        foreach (var syntaxToken in functionArgumentListing)
        {
            if (canReadComma is null)
            {
                _ = TryGetClassReferenceHierarchically(syntaxToken, null, out boundClassReferenceNode!);

                boundFunctionArguments.Add(boundClassReferenceNode);
                canReadComma = false;
            }
            else if (!canReadComma.Value)
            {
                var variableIdentifierToken = (IdentifierToken)syntaxToken;
                var boundVariableDeclaration = BindVariableDeclarationNode(boundClassReferenceNode!, variableIdentifierToken);

                boundFunctionArguments.Add(boundVariableDeclaration);
                canReadComma = true;
            }
            else
            {
                if (syntaxToken.SyntaxKind == SyntaxKind.CommaToken)
                    canReadComma = null;
                else
                    break;
            }
        }

        return new BoundFunctionArgumentsNode(
            openParenthesisToken,
            boundFunctionArguments,
            closeParenthesisToken);
    }

    /// <summary>Use this method for function invocation, whereas <see cref="BindFunctionArguments"/> should be used for function definition.</summary>
    public BoundFunctionParametersNode BindFunctionParameters(
        OpenParenthesisToken openParenthesisToken,
        List<ISyntaxToken> functionArgumentListing,
        CloseParenthesisToken closeParenthesisToken)
    {
        /*
         * out Type
         * out
         * ref
         * 1 + 1
         * "Hello World!"
         * variable
         * Property
         * MethodGroup
         * MethodInvocation
         */

        var boundFunctionParameterListing = new List<ISyntax>();

        // Alternate between reading 'OPTIONAL_KEYWORD[out, ref] Expression/IdentifierParameter' (true), and a single comma (false)
        bool canReadComma = false;

        for (var i = 0; i < functionArgumentListing.Count; i++)
        {
            var functionArgument = functionArgumentListing[i];

            if (!canReadComma)
            {
                if (functionArgument.SyntaxKind == SyntaxKind.KeywordToken)
                {
                    boundFunctionParameterListing.Add(functionArgument);

                    if (functionArgument.TextSpan.GetText() == "out")
                    {
                        if (i + 2 < functionArgumentListing.Count)
                        {
                            var possibleTypeArgument = functionArgumentListing[i + 1];
                            var possibleVariableArgument = functionArgumentListing[i + 2];

                            if (possibleTypeArgument.SyntaxKind != SyntaxKind.CommaToken &&
                                possibleVariableArgument.SyntaxKind != SyntaxKind.CommaToken)
                            {
                                // Take i + 1 to be a Type and then skip forward in the for loop.
                                _ = TryGetClassReferenceHierarchically(possibleTypeArgument, null, out var boundClassReferenceNode);
                                i++;

                                boundFunctionParameterListing.Add(boundClassReferenceNode);
                            }
                        }
                    }

                    // Skip the keyword
                    continue;
                }

                // TODO: Read from Function definition the BoundClassDefinitionNode
                var boundVariableDeclarationNode = BindVariableDeclarationNode(
                    // TODO: Don't pass in null
                    null,
                    (IdentifierToken)functionArgument);

                boundFunctionParameterListing.Add(boundVariableDeclarationNode);

                canReadComma = true;
            }
            else
            {
                canReadComma = false;
            }
        }

        return new BoundFunctionParametersNode(
            openParenthesisToken,
            boundFunctionParameterListing,
            closeParenthesisToken);
    }

    public void RegisterBoundScope(
        Type? scopeReturnType,
        TextEditorTextSpan textEditorTextSpan)
    {
        var boundScope = new BoundScope(
            _currentScope,
            scopeReturnType,
            textEditorTextSpan.StartingIndexInclusive,
            null,
            textEditorTextSpan.ResourceUri,
            new(),
            new(),
            new());

        _boundScopes.Add(boundScope);

        _boundScopes = _boundScopes
            .OrderBy(x => x.StartingIndexInclusive)
            .ToList();

        _currentScope = boundScope;
    }
    
    public void AddNamespaceToCurrentScope(
        BoundNamespaceStatementNode boundNamespaceStatementNode)
    {
        foreach (var namespaceEntry in boundNamespaceStatementNode.Children)
        {
            var boundNamespaceEntryNode = (BoundNamespaceEntryNode)namespaceEntry;

            foreach (var child in boundNamespaceEntryNode.CompilationUnit.Children)
            {
                if (child.SyntaxKind != SyntaxKind.BoundClassDefinitionNode)
                    continue;

                var boundClassDefinitionNode = (BoundClassDefinitionNode)child;

                var identifierText = boundClassDefinitionNode.TypeClauseToken.TextSpan
                    .GetText();

                var success = _currentScope.ClassDefinitionMap.TryAdd(
                    identifierText,
                    boundClassDefinitionNode);

                if (!success)
                {
                    _currentScope.ClassDefinitionMap[identifierText] =
                        boundClassDefinitionNode;
                }
            }
        }
    }

    public void DisposeBoundScope(
        TextEditorTextSpan textEditorTextSpan)
    {
        _currentScope.EndingIndexExclusive = textEditorTextSpan.EndingIndexExclusive;

        if (_currentScope.Parent is not null)
            _currentScope = _currentScope.Parent;
    }

    /// <summary>Search hierarchically through all the scopes, starting at the <see cref="_currentScope"/>.<br/><br/>If a match is found, then set the out parameter to it and return true.<br/><br/>If none of the searched scopes contained a match then set the out parameter to null and return false.</summary>
    public bool TryGetBoundFunctionDefinitionNodeHierarchically(
        string text,
        out BoundFunctionDefinitionNode? boundFunctionDefinitionNode)
    {
        var localScope = _currentScope;

        while (localScope is not null)
        {
            if (localScope.FunctionDefinitionMap.TryGetValue(
                    text,
                    out boundFunctionDefinitionNode))
            {
                return true;
            }

            localScope = localScope.Parent;
        }

        boundFunctionDefinitionNode = null;
        return false;
    }

    /// <summary>Search hierarchically through all the scopes, starting at the <see cref="_currentScope"/>.<br/><br/>If a match is found, then set the out parameter to it and return true.<br/><br/>If none of the searched scopes contained a match then set the out parameter to null and return false.</summary>
    public bool TryGetClassDefinitionHierarchically(
        ISyntaxToken typeClauseToken,
        BoundGenericArgumentsNode? boundGenericArgumentsNode,
        out BoundClassDefinitionNode? boundClassDefinitionNode)
    {
        var localScope = _currentScope;

        while (localScope is not null)
        {
            if (localScope.ClassDefinitionMap.TryGetValue(
                    typeClauseToken.TextSpan.GetText(),
                    out boundClassDefinitionNode))
            {
                return true;
            }

            localScope = localScope.Parent;
        }

        boundClassDefinitionNode = null;
        return false;
    }

    /// <summary>Search hierarchically through all the scopes, starting at the <see cref="_currentScope"/>.<br/><br/>If a match is found, then set the out parameter to it and return true.<br/><br/>If none of the searched scopes contained a match then set the out parameter to a fabricated instance and return false.</summary>
    public bool TryGetClassReferenceHierarchically(
        ISyntaxToken typeClauseToken,
        BoundGenericArgumentsNode? boundGenericArgumentsNode,
        out BoundClassReferenceNode? boundClassReferenceNode,
        bool shouldCreateTypeSymbolReference = true,
        bool shouldReportUndefinedTypeOrNamespace = true,
        bool shouldCreateClassDefinitionIfUndefined = true)
    {
        if (shouldCreateTypeSymbolReference &&
            typeClauseToken.SyntaxKind == SyntaxKind.IdentifierToken)
        {
            AddSymbolReference(new TypeSymbol(typeClauseToken.TextSpan with
            {
                DecorationByte = (byte)GenericDecorationKind.Type
            }));
        }

        var localScope = _currentScope;

        while (localScope is not null)
        {
            if (localScope.ClassDefinitionMap.TryGetValue(
                    typeClauseToken.TextSpan.GetText(),
                    out var existingBoundClassDefinitionNode))
            {
                boundClassReferenceNode = new BoundClassReferenceNode(
                    typeClauseToken,
                    existingBoundClassDefinitionNode.Type,
                    boundGenericArgumentsNode);

                return true;
            }

            localScope = localScope.Parent;
        }

        if (shouldReportUndefinedTypeOrNamespace)
        {
            _diagnosticBag.ReportUndefinedTypeOrNamespace(
                typeClauseToken.TextSpan,
                typeClauseToken.TextSpan.GetText());
        }

        if (shouldCreateClassDefinitionIfUndefined)
        {
            _ = TryBindClassDefinitionNode(
            typeClauseToken,
            boundGenericArgumentsNode,
            out var fabricatedBoundClassDefinitionNode);

            boundClassReferenceNode = new BoundClassReferenceNode(
                typeClauseToken,
                fabricatedBoundClassDefinitionNode.Type,
                boundGenericArgumentsNode);
        }
        else
        {
            boundClassReferenceNode = null;
        }

        return false;
    }

    /// <summary>Search hierarchically through all the scopes, starting at the <see cref="_currentScope"/>.<br/><br/>If a match is found, then set the out parameter to it and return true.<br/><br/>If none of the searched scopes contained a match then set the out parameter to null and return false.</summary>
    public bool TryGetVariableHierarchically(
        string text,
        out BoundVariableDeclarationStatementNode? boundVariableDeclarationStatementNode)
    {
        var localScope = _currentScope;

        while (localScope is not null)
        {
            if (localScope.VariableDeclarationMap.TryGetValue(
                    text,
                    out boundVariableDeclarationStatementNode))
            {
                return true;
            }

            localScope = localScope.Parent;
        }

        boundVariableDeclarationStatementNode = null;
        return false;
    }

    /// <summary>This method will handle the <see cref="SymbolDefinition"/>, but also invoke <see cref="AddSymbolReference"/> because each definition is being treated as a reference itself.</summary>
    private void AddSymbolDefinition(ISymbol symbol)
    {
        var symbolDefinitionId = ISymbol.GetSymbolDefinitionId(
            symbol.TextSpan.GetText(),
            _currentScope.BoundScopeKey);

        var symbolDefinition = new SymbolDefinition(
            _currentScope.BoundScopeKey,
            symbol);

        if (!_symbolDefinitions.TryAdd(
                symbolDefinitionId,
                symbolDefinition))
        {
            var existingSymbolDefinition = _symbolDefinitions[symbolDefinitionId];

            if (existingSymbolDefinition.IsFabricated)
            {
                _symbolDefinitions[symbolDefinitionId] = existingSymbolDefinition with
                {
                    IsFabricated = false
                };
            }
            // TODO: The else branch of this if statement would mean the Symbol definition was found twice, should a diagnostic be reported here?
        }

        AddSymbolReference(symbol);
    }

    private void AddSymbolReference(ISymbol symbol)
    {
        var symbolDefinitionId = ISymbol.GetSymbolDefinitionId(
            symbol.TextSpan.GetText(),
            _currentScope.BoundScopeKey);

        if (!_symbolDefinitions.TryGetValue(
                symbolDefinitionId,
                out var symbolDefinition))
        {
            symbolDefinition = new SymbolDefinition(
                _currentScope.BoundScopeKey,
                symbol)
            {
                IsFabricated = true
            };

            // TODO: Symbol definition was not found, should a diagnostic be reported here?
            var success = _symbolDefinitions.TryAdd(
                symbolDefinitionId,
                symbolDefinition);

            if (!success)
                _symbolDefinitions[symbolDefinitionId] = symbolDefinition;
        }

        symbolDefinition.SymbolReferences.Add(new SymbolReference(
            symbol,
            _currentScope.BoundScopeKey));
    }

    public void ClearStateByResourceUri(ResourceUri resourceUri)
    {
        foreach (var boundNamespaceStatementNode in _boundNamespaceStatementNodes)
        {
            var keep = boundNamespaceStatementNode.Value.Children
                .Where(x => ((BoundNamespaceEntryNode)x).ResourceUri != resourceUri)
                .ToImmutableArray();

            _boundNamespaceStatementNodes[boundNamespaceStatementNode.Key] =
                boundNamespaceStatementNode.Value with
                {
                    Children = keep
                };
        }
        
        foreach (var symbolDefinition in _symbolDefinitions)
        {
            var keep = symbolDefinition.Value.SymbolReferences
                .Where(x => x.Symbol.TextSpan.ResourceUri != resourceUri)
                .ToList();

            _symbolDefinitions[symbolDefinition.Key] =
                symbolDefinition.Value with
                {
                    SymbolReferences = keep
                };
        }

        _boundScopes = _boundScopes
            .Where(x => x.ResourceUri != resourceUri)
            .ToList();

        _diagnosticBag.ClearByResourceUri(resourceUri);
    }
}
