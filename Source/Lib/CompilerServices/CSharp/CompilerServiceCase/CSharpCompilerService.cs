using System.Collections.Immutable;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.CompilerServices.CSharp.BinderCase;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;
using Luthetus.CompilerServices.CSharp.RuntimeAssemblies;

namespace Luthetus.CompilerServices.CSharp.CompilerServiceCase;

public sealed class CSharpCompilerService : CompilerService
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
            GetBinderFunc = (resource, parser) => Binder,
			OnAfterLexAction = (resource, lexer) =>
            {
                var cSharpResource = (CSharpResource)resource;
                var cSharpLexer = (CSharpLexer)lexer;

                cSharpResource.EscapeCharacterList = cSharpLexer.EscapeCharacterList;
            },
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
                            return Task.CompletedTask;
                        }

                        _textEditorService.PostDistinct(
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
                                    cancellationToken: CancellationToken.None);

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
						return Task.CompletedTask;
                    });
            }));

        return autocompleteEntryList.DistinctBy(x => x.DisplayName).ToImmutableArray();
    }
}