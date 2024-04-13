using System.Collections.Immutable;
using Luthetus.CompilerServices.Lang.CSharp.Facts;
using Luthetus.CompilerServices.Lang.CSharp.ParserCase;
using Luthetus.CompilerServices.Lang.CSharp.ParserCase.Internals;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Expression;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.CSharp.BinderCase;

public partial class CSharpBinder : ILuthBinder
{
    private readonly Dictionary<string, NamespaceGroupNode> _namespaceGroupNodeMap = CSharpFacts.Namespaces.GetInitialBoundNamespaceStatementNodes();
    /// <summary>
    /// The key for _symbolDefinitions is calculated by <see cref="ISymbol.GetSymbolDefinitionId"/>
    /// </summary>
    private readonly Dictionary<string, SymbolDefinition> _symbolDefinitions = new();
    /// <summary>
    /// All of the type definitions should be maintainted in this dictionary as they are
    /// found via parsing. Then, when one types an ambiguous identifier, perhaps they
    /// wanted a type, and a lookup in this map can be done, and a using statement
    /// inserted for the user if they decide to use that autocomplete option.
    /// </summary>
    private readonly Dictionary<NamespaceAndTypeIdentifiers, TypeDefinitionNode> _allTypeDefinitions = new();
    private readonly LuthDiagnosticBag _diagnosticBag = new();
    private readonly CSharpBoundScope _globalScope = CSharpFacts.Scope.GetInitialGlobalScope();
    private readonly NamespaceStatementNode _topLevelNamespaceStatementNode = CSharpFacts.Namespaces.GetTopLevelNamespaceStatementNode();

    private List<CSharpBoundScope> _boundScopes = new();

    public CSharpBinder()
    {
        _boundScopes.Add(_globalScope);

        _boundScopes = _boundScopes
            .OrderBy(x => x.StartingIndexInclusive)
            .ToList();
    }

    public ImmutableDictionary<string, NamespaceGroupNode> NamespaceGroupNodes => _namespaceGroupNodeMap.ToImmutableDictionary();
    public ImmutableArray<ISymbol> Symbols => _symbolDefinitions.Values.SelectMany(x => x.SymbolReferences).Select(x => x.Symbol).ToImmutableArray();
    public Dictionary<string, SymbolDefinition> SymbolDefinitions => _symbolDefinitions;
    public ImmutableArray<CSharpBoundScope> BoundScopes => _boundScopes.ToImmutableArray();
    public ImmutableDictionary<NamespaceAndTypeIdentifiers, TypeDefinitionNode> AllTypeDefinitions => _allTypeDefinitions.ToImmutableDictionary();
    public ImmutableArray<TextEditorDiagnostic> DiagnosticsList => _diagnosticBag.ToImmutableArray();

    ImmutableArray<ITextEditorSymbol> ILuthBinder.SymbolsList => Symbols
        .Select(s => (ITextEditorSymbol)s)
        .ToImmutableArray();

    public ILuthBinderSession ConstructBinderSession(ResourceUri resourceUri)
    {
        return new CSharpBinderSession(
            resourceUri,
            _globalScope,
            _topLevelNamespaceStatementNode,
            this);
    }

    public LiteralExpressionNode BindLiteralExpressionNode(
        LiteralExpressionNode literalExpressionNode,
        ParserModel model)
    {
        var typeClauseNode = literalExpressionNode.LiteralSyntaxToken.SyntaxKind switch
        {
            SyntaxKind.NumericLiteralToken => CSharpFacts.Types.Int.ToTypeClause(),
            SyntaxKind.StringLiteralToken => CSharpFacts.Types.String.ToTypeClause(),
            _ => throw new NotImplementedException(),
        };

        return new LiteralExpressionNode(
            literalExpressionNode.LiteralSyntaxToken,
            typeClauseNode);
    }

    public BinaryOperatorNode BindBinaryOperatorNode(
        IExpressionNode leftExpressionNode,
        ISyntaxToken operatorToken,
        IExpressionNode rightExpressionNode,
        ParserModel parserModel)
    {
        var problematicTextSpan = (TextEditorTextSpan?)null;

        if (leftExpressionNode.ResultTypeClauseNode.ValueType == typeof(int))
        {
            if (rightExpressionNode.ResultTypeClauseNode.ValueType == typeof(int))
            {
                switch (operatorToken.SyntaxKind)
                {
                    case SyntaxKind.PlusToken:
                    case SyntaxKind.MinusToken:
                    case SyntaxKind.StarToken:
                    case SyntaxKind.DivisionToken:
                        return new BinaryOperatorNode(
                            leftExpressionNode.ResultTypeClauseNode,
                            operatorToken,
                            rightExpressionNode.ResultTypeClauseNode,
                            CSharpFacts.Types.Int.ToTypeClause());
                }
            }
            else
            {
                problematicTextSpan = rightExpressionNode.ConstructTextSpanRecursively();
            }
        }
        else if (leftExpressionNode.ResultTypeClauseNode.ValueType == typeof(string))
        {
            if (rightExpressionNode.ResultTypeClauseNode.ValueType == typeof(string))
            {
                switch (operatorToken.SyntaxKind)
                {
                    case SyntaxKind.PlusToken:
                        return new BinaryOperatorNode(
                            leftExpressionNode.ResultTypeClauseNode,
                            operatorToken,
                            rightExpressionNode.ResultTypeClauseNode,
                            CSharpFacts.Types.String.ToTypeClause());
                }
            }
            else
            {
                problematicTextSpan = rightExpressionNode.ConstructTextSpanRecursively();
            }
        }
        else
        {
            problematicTextSpan = leftExpressionNode.ConstructTextSpanRecursively();
        }

        if (problematicTextSpan is not null)
        {
            var errorMessage = $"Operator: {operatorToken.TextSpan.GetText()} is not defined" +
                $" for types: {leftExpressionNode.ConstructTextSpanRecursively().GetText()}" +
                $" and {rightExpressionNode.ConstructTextSpanRecursively().GetText()}";

            parserModel.DiagnosticBag.ReportTodoException(problematicTextSpan, errorMessage);
        }

        return new BinaryOperatorNode(
            leftExpressionNode.ResultTypeClauseNode,
            operatorToken,
            rightExpressionNode.ResultTypeClauseNode,
            CSharpFacts.Types.Void.ToTypeClause());
    }

    /// <summary>TODO: Construct a BoundStringInterpolationExpressionNode and identify the expressions within the string literal. For now I am just making the dollar sign the same color as a string literal.</summary>
    public void BindStringInterpolationExpression(
        DollarSignToken dollarSignToken,
        ParserModel model)
    {
        AddSymbolReference(new StringInterpolationSymbol(dollarSignToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.StringLiteral,
        }), model);
    }

    public void BindFunctionDefinitionNode(
        FunctionDefinitionNode functionDefinitionNode,
        ParserModel model)
    {
        var functionIdentifierText = functionDefinitionNode.FunctionIdentifierToken.TextSpan.GetText();

        var functionSymbol = new FunctionSymbol(functionDefinitionNode.FunctionIdentifierToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Function
        });

        AddSymbolDefinition(functionSymbol, model);

        if (!model.BinderSession.CurrentScope.FunctionDefinitionMap.TryAdd(
                functionIdentifierText,
                functionDefinitionNode))
        {
            _diagnosticBag.ReportAlreadyDefinedFunction(
                functionDefinitionNode.FunctionIdentifierToken.TextSpan,
                functionIdentifierText);
        }
    }

    public FunctionArgumentEntryNode BindFunctionOptionalArgument(
        FunctionArgumentEntryNode functionArgumentEntryNode,
        ISyntaxToken compileTimeConstantToken,
        bool hasOutKeyword,
        bool hasInKeyword,
        bool hasRefKeyword,
        ParserModel model)
    {
        var argumentTypeClauseNode = functionArgumentEntryNode.VariableDeclarationNode.TypeClauseNode;

        if (TryGetTypeDefinitionHierarchically(
                argumentTypeClauseNode.TypeIdentifierToken.TextSpan.GetText(),
                model.BinderSession.CurrentScope,
                out var typeDefinitionNode)
            || typeDefinitionNode is null)
        {
            typeDefinitionNode = CSharpFacts.Types.Void;
        }

        var literalExpressionNode = new LiteralExpressionNode(
            compileTimeConstantToken,
            typeDefinitionNode.ToTypeClause());

        literalExpressionNode = BindLiteralExpressionNode(literalExpressionNode, model);

        if (literalExpressionNode.ResultTypeClauseNode.ValueType is null ||
            literalExpressionNode.ResultTypeClauseNode.ValueType != functionArgumentEntryNode.VariableDeclarationNode.TypeClauseNode.ValueType)
        {
            var optionalArgumentTextSpan = functionArgumentEntryNode.VariableDeclarationNode.TypeClauseNode.TypeIdentifierToken.TextSpan with
            {
                EndingIndexExclusive = functionArgumentEntryNode.VariableDeclarationNode.IdentifierToken.TextSpan.EndingIndexExclusive
            };

            _diagnosticBag.ReportBadFunctionOptionalArgumentDueToMismatchInType(
                optionalArgumentTextSpan,
                functionArgumentEntryNode.VariableDeclarationNode.IdentifierToken.TextSpan.GetText(),
                functionArgumentEntryNode.VariableDeclarationNode.TypeClauseNode.ValueType?.Name ?? "null",
                literalExpressionNode.ResultTypeClauseNode.ValueType?.Name ?? "null");
        }

        return new FunctionArgumentEntryNode(
            functionArgumentEntryNode.VariableDeclarationNode,
            true,
            hasOutKeyword,
            hasInKeyword,
            hasRefKeyword);
    }

    /// <summary>TODO: Validate that the returned bound expression node has the same result type as the enclosing scope.</summary>
    public ReturnStatementNode BindReturnStatementNode(
        KeywordToken keywordToken,
        IExpressionNode expressionNode)
    {
        return new ReturnStatementNode(
            keywordToken,
            expressionNode);
    }

    public IfStatementNode BindIfStatementNode(
        KeywordToken ifKeywordToken,
        IExpressionNode expressionNode)
    {
        var boundIfStatementNode = new IfStatementNode(
            ifKeywordToken,
            expressionNode,
            null);

        return boundIfStatementNode;
    }

    public void SetCurrentNamespaceStatementNode(
        NamespaceStatementNode namespaceStatementNode,
        ParserModel model)
    {
        model.BinderSession.CurrentNamespaceStatementNode = namespaceStatementNode;
    }

    public void BindNamespaceStatementNode(
        NamespaceStatementNode namespaceStatementNode,
        ParserModel model)
    {
        var namespaceString = namespaceStatementNode.IdentifierToken.TextSpan.GetText();
        AddSymbolReference(new NamespaceSymbol(namespaceStatementNode.IdentifierToken.TextSpan), model);

        if (_namespaceGroupNodeMap.TryGetValue(namespaceString, out var inNamespaceGroupNode))
        {
            var outNamespaceStatementNodeList = inNamespaceGroupNode.NamespaceStatementNodeList
                .Add(namespaceStatementNode)
                .ToImmutableArray();

            var outNamespaceGroupNode = new NamespaceGroupNode(
                inNamespaceGroupNode.NamespaceString,
                outNamespaceStatementNodeList);

            _namespaceGroupNodeMap[namespaceString] = outNamespaceGroupNode;
        }
        else
        {
            _namespaceGroupNodeMap.Add(namespaceString, new NamespaceGroupNode(
                namespaceString,
                new NamespaceStatementNode[] { namespaceStatementNode }.ToImmutableArray()));
        }
    }

    public void BindConstructorInvocationNode()
    {
        // Deleted what was in this method because it was nonsense, and causing errors. (2023-08-06)
    }

    public InheritanceStatementNode BindInheritanceStatementNode(
        TypeClauseNode typeClauseNode,
        ParserModel model)
    {
        AddSymbolReference(new TypeSymbol(typeClauseNode.TypeIdentifierToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Type
        }), model);

        model.DiagnosticBag.ReportTodoException(
            typeClauseNode.TypeIdentifierToken.TextSpan,
            $"Implement {nameof(BindInheritanceStatementNode)}");

        return new InheritanceStatementNode(typeClauseNode);
    }

    public void BindVariableDeclarationNode(
        VariableDeclarationNode variableDeclarationNode,
        ParserModel model)
    {
        CreateVariableSymbol(variableDeclarationNode.IdentifierToken, variableDeclarationNode.VariableKind, model);
        var text = variableDeclarationNode.IdentifierToken.TextSpan.GetText();

        if (model.BinderSession.CurrentScope.VariableDeclarationMap.ContainsKey(text))
        {
            var existingVariableDeclarationNode = model.BinderSession.CurrentScope.VariableDeclarationMap[text];

            if (existingVariableDeclarationNode.IsFabricated)
            {
                // Overwrite the fabricated definition with a real one
                //
                // TODO: Track one or many declarations?...
                // (if there is an error where something is defined twice for example)
                model.BinderSession.CurrentScope.VariableDeclarationMap[text] = variableDeclarationNode;
            }

            _diagnosticBag.ReportAlreadyDefinedVariable(
                variableDeclarationNode.IdentifierToken.TextSpan,
                text);
        }
        else
        {
            model.BinderSession.CurrentScope.VariableDeclarationMap.Add(text, variableDeclarationNode);
        }
    }

    public VariableReferenceNode ConstructAndBindVariableReferenceNode(
        IdentifierToken variableIdentifierToken,
        ParserModel model)
    {
        var text = variableIdentifierToken.TextSpan.GetText();
        VariableReferenceNode? variableReferenceNode;

        if (TryGetVariableDeclarationHierarchically(
                text,
                model.BinderSession.CurrentScope,
                out var variableDeclarationNode)
            && variableDeclarationNode is not null)
        {
            variableReferenceNode = new VariableReferenceNode(
                variableIdentifierToken,
                variableDeclarationNode);
        }
        else
        {
            variableDeclarationNode = new VariableDeclarationNode(
                CSharpFacts.Types.Var.ToTypeClause(),
                variableIdentifierToken,
                VariableKind.Local,
                false)
            {
                IsFabricated = true,
            };

            variableReferenceNode = new VariableReferenceNode(
                variableIdentifierToken,
                variableDeclarationNode);

            _diagnosticBag.ReportUndefinedVariable(
                variableIdentifierToken.TextSpan,
                text);
        }

        CreateVariableSymbol(variableReferenceNode.VariableIdentifierToken, variableDeclarationNode.VariableKind, model);
        return variableReferenceNode;
    }

    public void BindVariableAssignmentExpressionNode(
        VariableAssignmentExpressionNode variableAssignmentExpressionNode,
        ParserModel model)
    {
        var text = variableAssignmentExpressionNode.VariableIdentifierToken.TextSpan.GetText();
        VariableKind variableKind = VariableKind.Local;

        if (TryGetVariableDeclarationHierarchically(
                text,
                model.BinderSession.CurrentScope,
                out var variableDeclarationNode)
            && variableDeclarationNode is not null)
        {
            variableKind = variableDeclarationNode.VariableKind;

            // TODO: Remove the setter from 'VariableDeclarationNode'...
            // ...and set IsInitialized to true by overwriting the VariableDeclarationMap.
            variableDeclarationNode.IsInitialized = true;
        }
        else
        {
            if (UtilityApi.IsContextualKeywordSyntaxKind(text))
            {
                _diagnosticBag.TheNameDoesNotExistInTheCurrentContext(
                    variableAssignmentExpressionNode.VariableIdentifierToken.TextSpan,
                    text);
            }
            else
            {
                _diagnosticBag.ReportUndefinedVariable(
                    variableAssignmentExpressionNode.VariableIdentifierToken.TextSpan,
                    text);
            }
        }

        CreateVariableSymbol(variableAssignmentExpressionNode.VariableIdentifierToken, variableKind, model);
    }

    public void BindConstructorDefinitionIdentifierToken(
        IdentifierToken identifierToken,
        ParserModel model)
    {
        var constructorSymbol = new ConstructorSymbol(identifierToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Type
        });

        AddSymbolDefinition(constructorSymbol, model);
    }

    public void BindFunctionInvocationNode(
        FunctionInvocationNode functionInvocationNode,
        ParserModel model)
    {
        var functionInvocationIdentifierText = functionInvocationNode
            .FunctionInvocationIdentifierToken.TextSpan.GetText();

        var functionSymbol = new FunctionSymbol(functionInvocationNode.FunctionInvocationIdentifierToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Function
        });

        AddSymbolReference(functionSymbol, model);

        if (TryGetFunctionHierarchically(
                functionInvocationIdentifierText,
                model.BinderSession.CurrentScope,
                out var functionDefinitionNode) &&
            functionDefinitionNode is not null)
        {
            return;
        }
        else
        {
            _diagnosticBag.ReportUndefinedFunction(
                functionInvocationNode.FunctionInvocationIdentifierToken.TextSpan,
                functionInvocationIdentifierText);
        }
    }

    public void BindNamespaceReference(
        IdentifierToken namespaceIdentifierToken,
        ParserModel model)
    {
        var namespaceSymbol = new NamespaceSymbol(namespaceIdentifierToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.None
        });

        AddSymbolReference(namespaceSymbol, model);
    }

    public TypeClauseNode BindTypeClauseNode(
        TypeClauseNode typeClauseNode,
        ParserModel model)
    {
        if (typeClauseNode.TypeIdentifierToken.SyntaxKind == SyntaxKind.IdentifierToken)
        {
            var typeSymbol = new TypeSymbol(typeClauseNode.TypeIdentifierToken.TextSpan with
            {
                DecorationByte = (byte)GenericDecorationKind.Type
            });

            AddSymbolReference(typeSymbol, model);
        }

        var matchingTypeDefintionNode = CSharpFacts.Types.TypeDefinitionNodes.SingleOrDefault(
            x => x.TypeIdentifierToken.TextSpan.GetText() == typeClauseNode.TypeIdentifierToken.TextSpan.GetText());

        if (matchingTypeDefintionNode is not null)
        {
            return new TypeClauseNode(
                typeClauseNode.TypeIdentifierToken,
                matchingTypeDefintionNode.ValueType,
                typeClauseNode.GenericParametersListingNode);
        }

        return typeClauseNode;
    }

    public void BindTypeIdentifier(
        IdentifierToken identifierToken,
        ParserModel model)
    {
        if (identifierToken.SyntaxKind == SyntaxKind.IdentifierToken)
        {
            var typeSymbol = new TypeSymbol(identifierToken.TextSpan with
            {
                DecorationByte = (byte)GenericDecorationKind.Type
            });

            AddSymbolReference(typeSymbol, model);
        }
    }

    public UsingStatementNode BindUsingStatementNode(
        KeywordToken usingKeywordToken,
        IdentifierToken namespaceIdentifierToken,
        ParserModel model)
    {
        AddSymbolReference(new NamespaceSymbol(namespaceIdentifierToken.TextSpan), model);

        var usingStatementNode = new UsingStatementNode(
            usingKeywordToken,
            namespaceIdentifierToken);

        model.BinderSession.CurrentUsingStatementNodeList.Add(usingStatementNode);
        AddNamespaceToCurrentScope(namespaceIdentifierToken.TextSpan.GetText(), model);

        return usingStatementNode;
    }

    /// <summary>TODO: Correctly implement this method. For now going to skip until the attribute closing square bracket.</summary>
    public AttributeNode BindAttributeNode(
        OpenSquareBracketToken openSquareBracketToken,
        List<ISyntaxToken> innerTokens,
        CloseSquareBracketToken closeSquareBracketToken,
        ParserModel model)
    {
        AddSymbolReference(new TypeSymbol(openSquareBracketToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Type,
            EndingIndexExclusive = closeSquareBracketToken.TextSpan.EndingIndexExclusive
        }), model);

        return new AttributeNode(
            openSquareBracketToken,
            innerTokens,
            closeSquareBracketToken);
    }

    public void RegisterBoundScope(
        TypeClauseNode? scopeReturnTypeClauseNode,
        TextEditorTextSpan textSpan,
        ParserModel model)
    {
        var boundScope = new CSharpBoundScope(
            model.BinderSession.CurrentScope,
            scopeReturnTypeClauseNode,
            textSpan.StartingIndexInclusive,
            null,
            textSpan.ResourceUri,
            new(),
            new(),
            new(),
            model.BinderSession.CurrentNamespaceStatementNode,
            model.BinderSession.CurrentUsingStatementNodeList);

        _boundScopes.Add(boundScope);

        _boundScopes = _boundScopes
            .OrderBy(x => x.StartingIndexInclusive)
            .ToList();

        model.BinderSession.CurrentScope = boundScope;
    }

    public void AddNamespaceToCurrentScope(
        string namespaceString,
        ParserModel model)
    {
        if (_namespaceGroupNodeMap.TryGetValue(namespaceString, out var namespaceGroupNode) &&
            namespaceGroupNode is not null)
        {
            var typeDefinitionNodes = namespaceGroupNode.GetTopLevelTypeDefinitionNodes();

            foreach (var typeDefinitionNode in typeDefinitionNodes)
            {
                _ = model.BinderSession.CurrentScope.TypeDefinitionMap.TryAdd(typeDefinitionNode.TypeIdentifierToken.TextSpan.GetText(), typeDefinitionNode);
            }
        }
    }

    public void DisposeBoundScope(
        TextEditorTextSpan textSpan,
        ParserModel model)
    {
        model.BinderSession.CurrentScope.EndingIndexExclusive = textSpan.EndingIndexExclusive;

        if (model.BinderSession.CurrentScope.Parent is not null)
            model.BinderSession.CurrentScope = model.BinderSession.CurrentScope.Parent;
    }

    public void BindTypeDefinitionNode(
        TypeDefinitionNode typeDefinitionNode,
        ParserModel model,
        bool shouldOverwrite = false)
    {
        var typeIdentifierText = typeDefinitionNode.TypeIdentifierToken.TextSpan.GetText();
        var currentNamespaceStatementText = model.BinderSession.CurrentNamespaceStatementNode.IdentifierToken.TextSpan.GetText();

        var namespaceAndTypeIdentifiers = new NamespaceAndTypeIdentifiers(currentNamespaceStatementText, typeIdentifierText);

        typeDefinitionNode.EncompassingNamespaceIdentifierString = currentNamespaceStatementText;

        var success = model.BinderSession.CurrentScope.TypeDefinitionMap.TryAdd(typeIdentifierText, typeDefinitionNode);
        if (!success && shouldOverwrite)
            model.BinderSession.CurrentScope.TypeDefinitionMap[typeIdentifierText] = typeDefinitionNode;

        success = _allTypeDefinitions.TryAdd(namespaceAndTypeIdentifiers, typeDefinitionNode);
        if (!success && shouldOverwrite)
            _allTypeDefinitions[namespaceAndTypeIdentifiers] = typeDefinitionNode;
    }

    /// <summary>This method will handle the <see cref="SymbolDefinition"/>, but also invoke <see cref="AddSymbolReference"/> because each definition is being treated as a reference itself.</summary>
    private void AddSymbolDefinition(
        ISymbol symbol,
        ParserModel model)
    {
        var symbolDefinitionId = ISymbol.GetSymbolDefinitionId(
            symbol.TextSpan.GetText(),
            model.BinderSession.CurrentScope.BoundScopeKey);

        var symbolDefinition = new SymbolDefinition(
            model.BinderSession.CurrentScope.BoundScopeKey,
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

        AddSymbolReference(symbol, model);
    }

    private void AddSymbolReference(ISymbol symbol, ParserModel model)
    {
        var symbolDefinitionId = ISymbol.GetSymbolDefinitionId(
            symbol.TextSpan.GetText(),
            model.BinderSession.CurrentScope.BoundScopeKey);

        if (!_symbolDefinitions.TryGetValue(
                symbolDefinitionId,
                out var symbolDefinition))
        {
            symbolDefinition = new SymbolDefinition(
                model.BinderSession.CurrentScope.BoundScopeKey,
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
            model.BinderSession.CurrentScope.BoundScopeKey));
    }

    public void CreateVariableSymbol(
        IdentifierToken identifierToken,
        VariableKind variableKind,
        ParserModel model)
    {
        switch (variableKind)
        {
            case VariableKind.Field:
                AddSymbolDefinition(new FieldSymbol(identifierToken.TextSpan with
                {
                    DecorationByte = (byte)GenericDecorationKind.Field
                }), model);
                break;
            case VariableKind.Property:
                AddSymbolDefinition(new PropertySymbol(identifierToken.TextSpan with
                {
                    DecorationByte = (byte)GenericDecorationKind.Property
                }), model);
                break;
            case VariableKind.Local:
                goto default;
            case VariableKind.Closure:
                goto default;
            default:
                AddSymbolDefinition(new VariableSymbol(identifierToken.TextSpan with
                {
                    DecorationByte = (byte)GenericDecorationKind.Variable
                }), model);
                break;
        }
    }

    public void ClearStateByResourceUri(ResourceUri resourceUri)
    {
        foreach (var namespaceGroupNodeKvp in _namespaceGroupNodeMap)
        {
            var keepStatements = namespaceGroupNodeKvp.Value.NamespaceStatementNodeList
                .Where(x => x.IdentifierToken.TextSpan.ResourceUri != resourceUri)
                .ToImmutableArray();

            _namespaceGroupNodeMap[namespaceGroupNodeKvp.Key] =
                new NamespaceGroupNode(
                    namespaceGroupNodeKvp.Value.NamespaceString,
                    keepStatements);
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

        foreach (var functionKvp in _globalScope.FunctionDefinitionMap)
        {
            if (functionKvp.Value.FunctionIdentifierToken.TextSpan.ResourceUri == resourceUri)
                _globalScope.FunctionDefinitionMap.Remove(functionKvp.Key);
        }

        foreach (var variableKvp in _globalScope.VariableDeclarationMap)
        {
            if (variableKvp.Value.IdentifierToken.TextSpan.ResourceUri == resourceUri)
                _globalScope.VariableDeclarationMap.Remove(variableKvp.Key);
        }

        foreach (var typeKvp in _globalScope.TypeDefinitionMap)
        {
            if (typeKvp.Value.TypeIdentifierToken.TextSpan.ResourceUri == resourceUri)
                _globalScope.TypeDefinitionMap.Remove(typeKvp.Key);
        }

        _diagnosticBag.ClearByResourceUri(resourceUri);
    }

    /// <summary>
    /// Search hierarchically through all the scopes, starting at the <see cref="initialScope"/>.<br/><br/>
    /// If a match is found, then set the out parameter to it and return true.<br/><br/>
    /// If none of the searched scopes contained a match then set the out parameter to null and return false.
    /// </summary>
    public bool TryGetFunctionHierarchically(
        string text,
        CSharpBoundScope? initialScope,
        out FunctionDefinitionNode? functionDefinitionNode)
    {
        var localScope = initialScope;

        while (localScope is not null)
        {
            if (localScope.FunctionDefinitionMap.TryGetValue(
                    text,
                    out functionDefinitionNode))
            {
                return true;
            }

            localScope = localScope.Parent;
        }

        functionDefinitionNode = null;
        return false;
    }

    /// <summary>
    /// Search hierarchically through all the scopes, starting at the <see cref="initialScope"/>.<br/><br/>
    /// If a match is found, then set the out parameter to it and return true.<br/><br/>
    /// If none of the searched scopes contained a match then set the out parameter to null and return false.
    /// </summary>
    public bool TryGetTypeDefinitionHierarchically(
        string text,
        CSharpBoundScope? initialScope,
        out TypeDefinitionNode? typeDefinitionNode)
    {
        var localScope = initialScope;

        while (localScope is not null)
        {
            if (localScope.TypeDefinitionMap.TryGetValue(
                    text,
                    out typeDefinitionNode))
            {
                return true;
            }

            localScope = localScope.Parent;
        }

        typeDefinitionNode = null;
        return false;
    }

    /// <summary>
    /// Search hierarchically through all the scopes, starting at the <see cref="_currentScope"/>.<br/><br/>
    /// If a match is found, then set the out parameter to it and return true.<br/><br/>
    /// If none of the searched scopes contained a match then set the out parameter to null and return false.
    /// </summary>
    public bool TryGetVariableDeclarationHierarchically(
        string text,
        CSharpBoundScope? initialScope,
        out VariableDeclarationNode? variableDeclarationStatementNode)
    {
        var localScope = initialScope;

        while (localScope is not null)
        {
            if (localScope.VariableDeclarationMap.TryGetValue(
                    text,
                    out variableDeclarationStatementNode))
            {
                return true;
            }

            localScope = localScope.Parent;
        }

        variableDeclarationStatementNode = null;
        return false;
    }

    public IBoundScope? GetBoundScope(TextEditorTextSpan textSpan)
    {
        var possibleScopes = _boundScopes
            .Where(x => x.ResourceUri == textSpan.ResourceUri || x.ResourceUri.Value == string.Empty)
            .Where(x =>
            {
                return x.StartingIndexInclusive <= textSpan.StartingIndexInclusive &&
                       (x.EndingIndexExclusive is null || // Global Scope awkwardly has a null ending index exclusive (2023-10-15)
                            x.EndingIndexExclusive >= textSpan.StartingIndexInclusive);
            });

        return possibleScopes.MinBy(
            x => textSpan.StartingIndexInclusive - x.StartingIndexInclusive);
    }

    public TextEditorTextSpan? GetDefinition(TextEditorTextSpan textSpan)
    {
        var boundScope = GetBoundScope(textSpan) as CSharpBoundScope;

        if (TryGetVariableDeclarationHierarchically(
                textSpan.GetText(),
                boundScope,
                out var variableDeclarationStatementNode)
            && variableDeclarationStatementNode is not null)
        {
            return variableDeclarationStatementNode.IdentifierToken.TextSpan;
        }
        else if (TryGetFunctionHierarchically(
                     textSpan.GetText(),
                     boundScope,
                     out var functionDefinitionNode)
                 && functionDefinitionNode is not null)
        {
            return functionDefinitionNode.FunctionIdentifierToken.TextSpan;
        }
        else if (TryGetTypeDefinitionHierarchically(
                     textSpan.GetText(),
                     boundScope,
                     out var typeDefinitionNode)
                 && typeDefinitionNode is not null)
        {
            return typeDefinitionNode.TypeIdentifierToken.TextSpan;
        }

        return null;
    }

    public ISyntaxNode? GetSyntaxNode(int positionIndex, CompilationUnit compilationUnit)
    {
        // First attempt at writing this, will be to start from the root of the compilation unit,
        // then traverse the syntax tree where the position index is within bounds.

        return RecursiveGetSyntaxNode(positionIndex, compilationUnit.RootCodeBlockNode);

        ISyntaxNode? RecursiveGetSyntaxNode(int positionIndex, ISyntaxNode targetNode)
        {
            foreach (var child in targetNode.ChildList)
            {
                if (child is ISyntaxNode syntaxNode)
                {
                    var innerResult = RecursiveGetSyntaxNode(positionIndex, syntaxNode);

                    if (innerResult is not null)
                        return innerResult;
                }
                else if (child is ISyntaxToken syntaxToken)
                {
                    if (syntaxToken.TextSpan.StartingIndexInclusive <= positionIndex &&
                        syntaxToken.TextSpan.EndingIndexExclusive >= positionIndex)
                    {
                        return targetNode;
                    }
                }
            }

            return null;
        }
    }
}