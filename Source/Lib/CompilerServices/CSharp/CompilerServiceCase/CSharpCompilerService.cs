using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.CompilerServices.Lang.CSharp.BinderCase;
using Luthetus.CompilerServices.Lang.CSharp.LexerCase;
using Luthetus.CompilerServices.Lang.CSharp.ParserCase;
using Luthetus.CompilerServices.Lang.CSharp.RuntimeAssemblies;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;

public sealed class CSharpCompilerService : LuthCompilerService
{
    /// <summary>
    /// TODO: The CSharpBinder should be private, but for now I'm making it public to be usable in the CompilerServiceExplorer Blazor component.
    /// </summary>
    public readonly CSharpBinder CSharpBinder = new();

    public CSharpCompilerService(ITextEditorService textEditorService)
        : base(textEditorService)
    {
        Binder = CSharpBinder;

        _compilerServiceOptions = new()
        {
            RegisterResourceFunc = resourceUri => new CSharpResource(resourceUri, this),
            GetLexerFunc = (resource, sourceText) => new CSharpLexer(resource.ResourceUri, sourceText),
            GetParserFunc = (resource, lexer) => new CSharpParser((CSharpLexer)lexer),
            GetBinderFunc = (resource, parser) => Binder
        };

        RuntimeAssembliesLoaderFactory.LoadDotNet6(CSharpBinder);
    }

    public event Action? CursorMovedInSyntaxTree;

    public override ImmutableArray<AutocompleteEntry> GetAutocompleteEntries(string word, TextEditorTextSpan textSpan)
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

                lock (_resourceMapLock)
                {
                    if (_resourceMap.ContainsKey(textSpan.ResourceUri))
                    {
                        cSharpResource = (CSharpResource)_resourceMap[textSpan.ResourceUri];
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
                                var cursorModifierBag = new CursorModifierBagTextEditor(
                                    Key<TextEditorViewModel>.Empty,
                                    new List<TextEditorCursorModifier> { new(cursor) });

                                var textToInsert = $"using {x.Key.NamespaceIdentifier};\n";

                                modelModifier.Insert(
                                    textToInsert,
                                    cursorModifierBag,
                                    CancellationToken.None);

                                foreach (var unsafeViewModel in viewModelList)
                                {
                                    var viewModelModifier = editContext.GetViewModelModifier(unsafeViewModel.ViewModelKey);
                                    var viewModelCursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);

                                    if (viewModelModifier is null || viewModelCursorModifierBag is null)
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
                                            .Invoke(editContext)
											.ConfigureAwait(false);
                                        }
                                    }

                                    await editContext.TextEditorService.ModelApi.ApplySyntaxHighlightingFactory(
                                            modelModifier.ResourceUri)
                                        .Invoke(editContext)
                                        .ConfigureAwait(false);
                                }
                            });
                    });
            }));

        return autocompleteEntryList.DistinctBy(x => x.DisplayName).ToImmutableArray();
    }
}