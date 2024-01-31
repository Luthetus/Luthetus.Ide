using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.CompilerServices.Lang.CSharp.BinderCase;
using Luthetus.CompilerServices.Lang.CSharp.LexerCase;
using Luthetus.CompilerServices.Lang.CSharp.ParserCase;
using Luthetus.CompilerServices.Lang.CSharp.RuntimeAssemblies;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;

public class CSharpCompilerService : ICompilerService
{
    private readonly Dictionary<ResourceUri, CSharpResource> _cSharpResourceMap = new();
    private readonly object _cSharpResourceMapLock = new();
    private readonly ITextEditorService _textEditorService;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IDispatcher _dispatcher;
    
    /// <summary>
    /// TODO: The CSharpBinder should be private, but for now I'm making it public to be usable in the CompilerServiceExplorer Blazor component.
    /// </summary>
    public readonly CSharpBinder CSharpBinder = new();

    public CSharpCompilerService(
        ITextEditorService textEditorService,
        IBackgroundTaskService backgroundTaskService,
        IDispatcher dispatcher)
    {
        _textEditorService = textEditorService;
        _backgroundTaskService = backgroundTaskService;
        _dispatcher = dispatcher;

        RuntimeAssembliesLoaderFactory.LoadDotNet6(CSharpBinder);
    }

    public event Action? ResourceRegistered;
    public event Action? ResourceParsed;
    public event Action? ResourceDisposed;
    public event Action? CursorMovedInSyntaxTree;

    public IBinder? Binder => CSharpBinder;

    public ImmutableArray<ICompilerServiceResource> CompilerServiceResources =>
        _cSharpResourceMap.Values
            .Select(csr => (ICompilerServiceResource)csr)
            .ToImmutableArray();

    public void RegisterResource(ResourceUri resourceUri)
    {
        lock (_cSharpResourceMapLock)
        {
            if (_cSharpResourceMap.ContainsKey(resourceUri))
                return;

            _cSharpResourceMap.Add(
                resourceUri,
                new(resourceUri, this));
        }

        QueueParseRequest(resourceUri);
        ResourceRegistered?.Invoke();
    }

    public ICompilerServiceResource? GetCompilerServiceResourceFor(ResourceUri resourceUri)
    {
        var model = _textEditorService.ModelApi.GetOrDefault(resourceUri);

        if (model is null)
            return null;

        lock (_cSharpResourceMapLock)
        {
            if (!_cSharpResourceMap.ContainsKey(resourceUri))
                return null;

            return _cSharpResourceMap[resourceUri];
        }
    }

    public ImmutableArray<TextEditorTextSpan> GetSyntacticTextSpansFor(ResourceUri resourceUri)
    {
        lock (_cSharpResourceMapLock)
        {
            if (!_cSharpResourceMap.ContainsKey(resourceUri))
                return ImmutableArray<TextEditorTextSpan>.Empty;

            return _cSharpResourceMap[resourceUri].SyntacticTextSpans;
        }
    }

    public ImmutableArray<ITextEditorSymbol> GetSymbolsFor(ResourceUri resourceUri)
    {
        lock (_cSharpResourceMapLock)
        {
            if (!_cSharpResourceMap.ContainsKey(resourceUri))
                return ImmutableArray<ITextEditorSymbol>.Empty;

            return _cSharpResourceMap[resourceUri].Symbols;
        }
    }

    public ImmutableArray<TextEditorDiagnostic> GetDiagnosticsFor(ResourceUri resourceUri)
    {
        lock (_cSharpResourceMapLock)
        {
            if (!_cSharpResourceMap.ContainsKey(resourceUri))
                return ImmutableArray<TextEditorDiagnostic>.Empty;

            return _cSharpResourceMap[resourceUri].Diagnostics;
        }
    }

    public void ResourceWasModified(ResourceUri resourceUri, ImmutableArray<TextEditorTextSpan> editTextSpans)
    {
        QueueParseRequest(resourceUri);
    }

    public void CursorWasModified(ResourceUri resourceUri, TextEditorCursor cursor)
    {
    }

    public ImmutableArray<AutocompleteEntry> GetAutocompleteEntries(string word, TextEditorTextSpan textSpan)
    {
        var boundScope = CSharpBinder.GetBoundScope(textSpan) as CSharpBoundScope;

        if (boundScope is null)
            return ImmutableArray<AutocompleteEntry>.Empty;

        var autocompleteEntryList = new List<AutocompleteEntry>();

        // (2024-01-27)
        // Goal: when one types 'new Person { ... }',
        //       if their cursor is between the object initialization braces,
        //       then provide autocomplete for the public properties of that type.
        {
            // Idea: Determine where the user's cursor is, in terms of the deepest syntax node
            //       in the CompilationUnit which the cursor is encompassed in.
            //
            // Change: I need to add a parameter that tells me the exact cursor position I believe?
            //         Did the word match to the left or right of the cursor. Or was the cursor
            //         within the word that I recieve?
            //
            // Caching?: Is it possible to keep the current syntax node that the user's cursor
            //           is within, available at all times? As to not be recalculated from the root
            //           compilation unit each time?
            {
                var cSharpResource = (CSharpResource?)null;

                lock (_cSharpResourceMapLock)
                {
                    if (_cSharpResourceMap.ContainsKey(textSpan.ResourceUri))
                    {
                        cSharpResource = _cSharpResourceMap[textSpan.ResourceUri];
                    }
                }

                if (cSharpResource is not null)
                {
                    var aaa = 2;
                }
            }
        }

        var targetScope = boundScope;

        while (targetScope is not null)
        {
            autocompleteEntryList.AddRange(
                targetScope.VariableDeclarationMap.Keys
                .ToArray()
                .Where(x => x.Contains(word, StringComparison.InvariantCulture))
                .Distinct()
                .Take(10)
                .Select(x =>
                {
                    return new AutocompleteEntry(
                        x,
                        AutocompleteEntryKind.Variable,
                        null);
                }));

            autocompleteEntryList.AddRange(
                targetScope.FunctionDefinitionMap.Keys
                .ToArray()
                .Where(x => x.Contains(word, StringComparison.InvariantCulture))
                .Distinct()
                .Take(10)
                .Select(x =>
                {
                    return new AutocompleteEntry(
                        x,
                        AutocompleteEntryKind.Function,
                        null);
                }));

            targetScope = targetScope.Parent;
        }

        var allTypeDefinitions = CSharpBinder.AllTypeDefinitions;

        autocompleteEntryList.AddRange(
            allTypeDefinitions
            .Where(x => x.Key.TypeIdentifier.Contains(word, StringComparison.InvariantCulture))
            .Distinct()
            .Take(10)
            .Select(x =>
            {
                return new AutocompleteEntry(
                    x.Key.TypeIdentifier,
                    AutocompleteEntryKind.Type,
                    () =>
                    {
                        if (boundScope.EncompassingNamespaceStatementNode.IdentifierToken.TextSpan.GetText() == x.Key.NamespaceIdentifier ||
                            boundScope.CurrentUsingStatementNodeList.Any(usn => usn.NamespaceIdentifier.TextSpan.GetText() == x.Key.NamespaceIdentifier))
                        {
                            return;
                        }

                        _textEditorService.Post(
                            "Add using statement",
                            async editContext =>
                            {
                                var modelModifier = editContext.GetModelModifier(textSpan.ResourceUri);

                                if (modelModifier is null)
                                    return;

                                var viewModelList = _textEditorService.ModelApi.GetViewModelsOrEmpty(textSpan.ResourceUri);

                                var cursor = new TextEditorCursor(0, 0, true);
                                var cursorModifierBag = new TextEditorCursorModifierBag(
                                    Key<TextEditorViewModel>.Empty,
                                    new List<TextEditorCursorModifier> { new(cursor) });

                                var textToInsert = $"using {x.Key.NamespaceIdentifier};\n";

                                modelModifier.EditByInsertion(
                                    textToInsert,
                                    cursorModifierBag,
                                    CancellationToken.None);

                                foreach (var unsafeViewModel in viewModelList)
                                {
                                    var viewModelModifier = editContext.GetViewModelModifier(
                                        unsafeViewModel.ViewModelKey);

                                    if (viewModelModifier is null)
                                        continue;

                                    var viewModelCursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);

                                    if (viewModelCursorModifierBag is null)
                                        continue;

                                    foreach (var cursorModifier in viewModelCursorModifierBag.List)
                                    {
                                        for (int i = 0; i < textToInsert.Length; i++)
                                        {
                                            await _textEditorService.ViewModelApi.MoveCursorFactory(
                                                new KeyboardEventArgs
                                                {
                                                    Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                                                },
                                                textSpan.ResourceUri,
                                                viewModelModifier.ViewModel.ViewModelKey)
                                            .Invoke(editContext);
                                        }
                                    }

                                    await modelModifier.ApplySyntaxHighlightingAsync();
                                }
                            });
                    });
            }));

        return autocompleteEntryList.DistinctBy(x => x.DisplayName).ToImmutableArray();
    }

    public void DisposeResource(ResourceUri resourceUri)
    {
        lock (_cSharpResourceMapLock)
        {
            _cSharpResourceMap.Remove(resourceUri);
        }

        ResourceDisposed?.Invoke();
    }

    private void QueueParseRequest(ResourceUri resourceUri)
    {
        _textEditorService.Post(
            nameof(QueueParseRequest),
            async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(resourceUri);

                if (modelModifier is null)
                    return;

                await _textEditorService.ModelApi.CalculatePresentationModelFactory(
                        modelModifier.ResourceUri,
                        CompilerServiceDiagnosticPresentationFacts.PresentationKey)
                    .Invoke(editContext);

                var pendingCalculation = modelModifier.PresentationModelsList.FirstOrDefault(x =>
                    x.TextEditorPresentationKey == CompilerServiceDiagnosticPresentationFacts.PresentationKey)
                    ?.PendingCalculation;

                if (pendingCalculation is null)
                    pendingCalculation = new(modelModifier.GetAllText());

                var lexer = new CSharpLexer(resourceUri, pendingCalculation.ContentAtRequest);
                lexer.Lex();

                CompilationUnit? compilationUnit = null;

                try
                {
                    // Even if the parser throws an exception, be sure to
                    // make use of the Lexer to do whatever syntax highlighting is possible.
                    var parser = new CSharpParser(lexer);
                    compilationUnit = parser.Parse(CSharpBinder, resourceUri);
                }
                finally
                {
                    lock (_cSharpResourceMapLock)
                    {
                        if (_cSharpResourceMap.ContainsKey(resourceUri))
                        {
                            var cSharpResource = _cSharpResourceMap[resourceUri];

                            cSharpResource.SyntaxTokens = lexer.SyntaxTokens;

                            if (compilationUnit is not null)
                                cSharpResource.CompilationUnit = compilationUnit;
                        }
                    }

                    // TODO: Shouldn't one get a reference to the most recent TextEditorModel instance with the given key and invoke .ApplySyntaxHighlightingAsync() on that?
                    await modelModifier.ApplySyntaxHighlightingAsync();

                    var presentationModel = modelModifier.PresentationModelsList.FirstOrDefault(x =>
                        x.TextEditorPresentationKey == CompilerServiceDiagnosticPresentationFacts.PresentationKey);

                    if (presentationModel?.PendingCalculation is not null)
                    {
                        presentationModel.PendingCalculation.TextEditorTextSpanList =
                            GetDiagnosticsFor(modelModifier.ResourceUri)
                                .Select(x => x.TextSpan)
                                .ToImmutableArray();

                        (presentationModel.CompletedCalculation, presentationModel.PendingCalculation) =
                            (presentationModel.PendingCalculation, presentationModel.CompletedCalculation);
                    }

                    ResourceParsed?.Invoke();
                }
            });
    }
}