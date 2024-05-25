using System.Collections.Immutable;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;
using Fluxor;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.TextEditor.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.JsRuntimes.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public class TextEditorEvents
{
    private readonly TextEditorViewModelDisplay _viewModelDisplay;
    private readonly ThrottleAsync _throttleApplySyntaxHighlighting = new ThrottleAsync(TimeSpan.FromMilliseconds(500));

    public TextEditorEvents(TextEditorViewModelDisplay viewModelDisplay, TextEditorOptions? options)
    {
        _viewModelDisplay = viewModelDisplay;
		Options = options ?? TextEditorService.OptionsStateWrap.Value.Options;
	}

    /// <summary>
    /// (2024-04-05) The code I'm writing today feels like spaghetti code / technical debt.
    /// That being said, I'll call today's hacky code a "technical loan".
    /// </summary>
	public TextEditorEvents(TextEditorEvents events, TextEditorKeymapDefault textEditorKeymapDefault)
	{
        _viewModelDisplay = events._viewModelDisplay;

        Options = events.Options with
        {
            Keymap = textEditorKeymapDefault
        };
	}

	public static TimeSpan ThrottleDelayDefault { get; } = TimeSpan.FromMilliseconds(30);
    public static TimeSpan OnMouseOutTooltipDelay { get; } = TimeSpan.FromMilliseconds(1_000);
    public static TimeSpan MouseStoppedMovingDelay { get; } = TimeSpan.FromMilliseconds(400);
    public Task MouseStoppedMovingTask { get; set; } = Task.CompletedTask;
    public Task MouseNoLongerOverTooltipTask { get; set; } = Task.CompletedTask;
    public CancellationTokenSource MouseNoLongerOverTooltipCancellationTokenSource { get; set; } = new();
    public CancellationTokenSource MouseStoppedMovingCancellationTokenSource { get; set; } = new();
    public TooltipViewModel? TooltipViewModel { get; set; }

    /// <summary>This accounts for one who might hold down Left Mouse Button from outside the TextEditorDisplay's content div then move their mouse over the content div while holding the Left Mouse Button down.</summary>
    public bool ThinksLeftMouseButtonIsDown { get; set; }

    public ViewModelDisplayOptions ViewModelDisplayOptions => _viewModelDisplay.ViewModelDisplayOptions;
    public CursorDisplay? CursorDisplay => _viewModelDisplay.CursorDisplay;
    public ITextEditorService TextEditorService => _viewModelDisplay.TextEditorService;
    public IClipboardService ClipboardService => _viewModelDisplay.ClipboardService;
    public IJSRuntime JsRuntime => _viewModelDisplay.JsRuntime;
    public IDispatcher Dispatcher => _viewModelDisplay.Dispatcher;
    public IServiceProvider ServiceProvider => _viewModelDisplay.ServiceProvider;
    public LuthetusTextEditorConfig TextEditorConfig => _viewModelDisplay.TextEditorConfig;

    public Action CursorPauseBlinkAnimationAction 
    { 
        get
        {
            var localCursorDisplay = CursorDisplay;

            if (localCursorDisplay is not null)
                return localCursorDisplay.PauseBlinkAnimation;

            return () => { };
        }
    }

    public Func<MenuKind, bool, Task> CursorSetShouldDisplayMenuAsyncFunc 
    { 
        get
        {
            return async (a, b) =>
            {
                var localCursorDisplay = CursorDisplay;
                if (localCursorDisplay is not null)
                    await localCursorDisplay.SetShouldDisplayMenuAsync(a, b).ConfigureAwait(false);
            };
        }
    }

	public TextEditorOptions Options { get; init; }

	/// <summary>The default <see cref="AfterOnKeyDownAsync"/> will provide syntax highlighting, and autocomplete.<br/><br/>The syntax highlighting occurs on ';', whitespace, paste, undo, redo<br/><br/>The autocomplete occurs on LetterOrDigit typed or { Ctrl + Space }. Furthermore, the autocomplete is done via <see cref="IAutocompleteService"/> and the one can provide their own implementation when registering the Luthetus.TextEditor services using <see cref="LuthetusTextEditorConfig.AutocompleteServiceFactory"/></summary>
	public TextEditorEdit HandleAfterOnKeyDownAsyncFactory(
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey,
        KeyboardEventArgs keyboardEventArgs,
        Func<MenuKind, bool, Task> setTextEditorMenuKind)
    {
        if (_viewModelDisplay.ViewModelDisplayOptions.AfterOnKeyDownAsyncFactory is not null)
        {
            return _viewModelDisplay.ViewModelDisplayOptions.AfterOnKeyDownAsyncFactory.Invoke(
                resourceUri,
                viewModelKey,
                keyboardEventArgs,
                setTextEditorMenuKind);
        }

        return async editContext =>
        {
            var modelModifier = editContext.GetModelModifier(resourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return;

            // Indexing can be invoked and this method still check for syntax highlighting and such
            if (IsAutocompleteIndexerInvoker(keyboardEventArgs))
            {
                _ = Task.Run(async () =>
                {
                    if (primaryCursorModifier.ColumnIndex > 0)
                    {
                        // All keyboardEventArgs that return true from "IsAutocompleteIndexerInvoker"
                        // are to be 1 character long, as well either specific whitespace or punctuation.
                        // Therefore 1 character behind might be a word that can be indexed.
                        var word = modelModifier.ReadPreviousWordOrDefault(
                            primaryCursorModifier.LineIndex,
                            primaryCursorModifier.ColumnIndex);

                        if (word is not null)
                        {
                            await _viewModelDisplay.AutocompleteIndexer
                                .IndexWordAsync(word)
                                .ConfigureAwait(false);
                        }
                    }
                });
            }

            if (IsAutocompleteMenuInvoker(keyboardEventArgs))
            {
                await setTextEditorMenuKind
                    .Invoke(MenuKind.AutoCompleteMenu, true)
                    .ConfigureAwait(false);
            }
            else if (IsSyntaxHighlightingInvoker(keyboardEventArgs))
            {
                await ThrottleApplySyntaxHighlighting(modelModifier).ConfigureAwait(false);
            }
        };
    }

    public TextEditorEdit HandleAfterOnKeyDownRangeAsyncFactory(
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey,
        List<KeyboardEventArgs> keyboardEventArgsList,
        Func<MenuKind, bool, Task> setTextEditorMenuKind)
    {
        if (_viewModelDisplay.ViewModelDisplayOptions.AfterOnKeyDownRangeAsyncFactory is not null)
        {
            return _viewModelDisplay.ViewModelDisplayOptions.AfterOnKeyDownRangeAsyncFactory.Invoke(
                resourceUri,
                viewModelKey,
                keyboardEventArgsList,
                setTextEditorMenuKind);
        }

        return async editContext =>
        {
            var modelModifier = editContext.GetModelModifier(resourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return;

            var seenIsAutocompleteIndexerInvoker = false;
            var seenIsAutocompleteMenuInvoker = false;
            var seenIsSyntaxHighlightingInvoker = false;

            foreach (var keyboardEventArgs in keyboardEventArgsList)
            {
                if (!seenIsAutocompleteIndexerInvoker && IsAutocompleteIndexerInvoker(keyboardEventArgs))
                    seenIsAutocompleteIndexerInvoker = true;

                if (!seenIsAutocompleteMenuInvoker && IsAutocompleteMenuInvoker(keyboardEventArgs))
                    seenIsAutocompleteMenuInvoker = true;
                else if (!seenIsSyntaxHighlightingInvoker && IsSyntaxHighlightingInvoker(keyboardEventArgs))
                    seenIsSyntaxHighlightingInvoker = true;
            }

            if (seenIsAutocompleteIndexerInvoker)
            {
                _ = Task.Run(async () =>
                {
                    if (primaryCursorModifier.ColumnIndex > 0)
                    {
                        // All keyboardEventArgs that return true from "IsAutocompleteIndexerInvoker"
                        // are to be 1 character long, as well either specific whitespace or punctuation.
                        // Therefore 1 character behind might be a word that can be indexed.
                        var word = modelModifier.ReadPreviousWordOrDefault(
                            primaryCursorModifier.LineIndex,
                            primaryCursorModifier.ColumnIndex);

                        if (word is not null)
                        {
                            await _viewModelDisplay.AutocompleteIndexer
                                .IndexWordAsync(word)
                                .ConfigureAwait(false);
                        }
                    }
                });
            }

            if (seenIsAutocompleteMenuInvoker)
            {
                await setTextEditorMenuKind
                    .Invoke(MenuKind.AutoCompleteMenu, true)
                    .ConfigureAwait(false);
            }

            if (seenIsSyntaxHighlightingInvoker)
            {
                await ThrottleApplySyntaxHighlighting(modelModifier).ConfigureAwait(false);
            }
        };
    }

    public bool IsSyntaxHighlightingInvoker(KeyboardEventArgs keyboardEventArgs)
    {
        return keyboardEventArgs.Key == ";" ||
               KeyboardKeyFacts.IsWhitespaceCode(keyboardEventArgs.Code) ||
               keyboardEventArgs.CtrlKey && keyboardEventArgs.Key == "s" ||
               keyboardEventArgs.CtrlKey && keyboardEventArgs.Key == "v" ||
               keyboardEventArgs.CtrlKey && keyboardEventArgs.Key == "z" ||
               keyboardEventArgs.CtrlKey && keyboardEventArgs.Key == "y";
    }

    public bool IsAutocompleteMenuInvoker(KeyboardEventArgs keyboardEventArgs)
    {
        // Is {Ctrl + Space} or LetterOrDigit was hit without Ctrl being held
        return keyboardEventArgs.CtrlKey &&
                   keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE ||
               !keyboardEventArgs.CtrlKey &&
                   !KeyboardKeyFacts.IsWhitespaceCode(keyboardEventArgs.Code) &&
                   !KeyboardKeyFacts.IsMetaKey(keyboardEventArgs);
    }

    /// <summary>
    /// All keyboardEventArgs that return true from "IsAutocompleteIndexerInvoker"
    /// are to be 1 character long, as well either whitespace or punctuation.
    /// Therefore 1 character behind might be a word that can be indexed.
    /// </summary>
    public bool IsAutocompleteIndexerInvoker(KeyboardEventArgs keyboardEventArgs)
    {
        return (KeyboardKeyFacts.IsWhitespaceCode(keyboardEventArgs.Code) ||
                    KeyboardKeyFacts.IsPunctuationCharacter(keyboardEventArgs.Key.First())) &&
                !keyboardEventArgs.CtrlKey;
    }

    public async Task HandleMouseStoppedMovingEventAsync(MouseEventArgs mouseEventArgs)
    {
        var model = _viewModelDisplay.GetModel();
        var viewModel = _viewModelDisplay.GetViewModel();

        if (model is null || viewModel is null)
            return;

        // Lazily calculate row and column index a second time. Otherwise one has to calculate it every mouse moved event.
        var rowAndColumnIndex = await CalculateRowAndColumnIndex(mouseEventArgs).ConfigureAwait(false);

        // TODO: (2023-05-28) This shouldn't be re-calcuated in the best case scenario. That is to say, the previous line invokes 'CalculateRowAndColumnIndex(...)' which also invokes this logic
        var relativeCoordinatesOnClick = await JsRuntime.GetLuthetusTextEditorApi()
            .GetRelativePosition(
                viewModel.BodyElementId,
                mouseEventArgs.ClientX,
                mouseEventArgs.ClientY)
            .ConfigureAwait(false);

        var cursorPositionIndex = model.GetPositionIndex(new TextEditorCursor(
            rowAndColumnIndex.rowIndex,
            rowAndColumnIndex.columnIndex,
            true));

        var foundMatch = false;

        var symbols = model.CompilerService.GetSymbolsFor(model.ResourceUri);
        var diagnostics = model.CompilerService.GetDiagnosticsFor(model.ResourceUri);

        if (diagnostics.Length != 0)
        {
            foreach (var diagnostic in diagnostics)
            {
                if (cursorPositionIndex >= diagnostic.TextSpan.StartingIndexInclusive &&
                    cursorPositionIndex < diagnostic.TextSpan.EndingIndexExclusive)
                {
                    // Prefer showing a diagnostic over a symbol when both exist at the mouse location.
                    foundMatch = true;

                    var parameterMap = new Dictionary<string, object?>
                    {
                        {
                            nameof(ITextEditorDiagnosticRenderer.Diagnostic),
                            diagnostic
                        }
                    };

                    _viewModelDisplay._events.TooltipViewModel = new(
                        _viewModelDisplay.LuthetusTextEditorComponentRenderers.DiagnosticRendererType,
                        parameterMap,
                        relativeCoordinatesOnClick,
                        null,
                        ContinueRenderingTooltipAsync);
                }
            }
        }

        if (!foundMatch && symbols.Length != 0)
        {
            foreach (var symbol in symbols)
            {
                if (cursorPositionIndex >= symbol.TextSpan.StartingIndexInclusive &&
                    cursorPositionIndex < symbol.TextSpan.EndingIndexExclusive)
                {
                    foundMatch = true;

                    var parameters = new Dictionary<string, object?>
                    {
                        {
                            nameof(ITextEditorSymbolRenderer.Symbol),
                            symbol
                        }
                    };

                    _viewModelDisplay._events.TooltipViewModel = new(
                        _viewModelDisplay.LuthetusTextEditorComponentRenderers.SymbolRendererType,
                        parameters,
                        relativeCoordinatesOnClick,
                        null,
                        ContinueRenderingTooltipAsync);
                }
            }
        }

        if (!foundMatch)
        {
            if (_viewModelDisplay._events.TooltipViewModel is null)
                return; // Avoid the re-render if nothing changed

            _viewModelDisplay._events.TooltipViewModel = null;
        }

        // TODO: Measure the tooltip, and reposition if it would go offscreen.

        await _viewModelDisplay.Hack_ReRender();
    }

    public Task ContinueRenderingTooltipAsync()
    {
        MouseNoLongerOverTooltipCancellationTokenSource.Cancel();
        MouseNoLongerOverTooltipCancellationTokenSource = new();

        return Task.CompletedTask;
    }

    public async Task<(int rowIndex, int columnIndex)> CalculateRowAndColumnIndex(MouseEventArgs mouseEventArgs)
    {
        var model = _viewModelDisplay.GetModel();
        var viewModel = _viewModelDisplay.GetViewModel();
        var globalTextEditorOptions = TextEditorService.OptionsStateWrap.Value.Options;

        if (model is null || viewModel is null)
            return (0, 0);

        var charMeasurements = viewModel.VirtualizationResult.CharAndLineMeasurements;

        var relativeCoordinatesOnClick = await JsRuntime.GetLuthetusTextEditorApi()
            .GetRelativePosition(
                viewModel.BodyElementId,
                mouseEventArgs.ClientX,
                mouseEventArgs.ClientY)
            .ConfigureAwait(false);

        var positionX = relativeCoordinatesOnClick.RelativeX;
        var positionY = relativeCoordinatesOnClick.RelativeY;

        // Scroll position offset
        {
            positionX += relativeCoordinatesOnClick.RelativeScrollLeft;
            positionY += relativeCoordinatesOnClick.RelativeScrollTop;
        }

        var rowIndex = (int)(positionY / charMeasurements.LineHeight);

        rowIndex = rowIndex > model.LineCount - 1
            ? model.LineCount - 1
            : rowIndex;

        int columnIndexInt;

        if (!globalTextEditorOptions.UseMonospaceOptimizations)
        {
            var guid = Guid.NewGuid();

            columnIndexInt = await JsRuntime.GetLuthetusTextEditorApi()
                .CalculateProportionalColumnIndex(
                    _viewModelDisplay.ProportionalFontMeasurementsContainerElementId,
                    $"luth_te_proportional-font-measurement-parent_{_viewModelDisplay._textEditorHtmlElementId}_{guid}",
                    $"luth_te_proportional-font-measurement-cursor_{_viewModelDisplay._textEditorHtmlElementId}_{guid}",
                    positionX,
                    charMeasurements.CharacterWidth,
                    model.GetLineText(rowIndex))
                .ConfigureAwait(false);

            if (columnIndexInt == -1)
            {
                var columnIndexDouble = positionX / charMeasurements.CharacterWidth;
                columnIndexInt = (int)Math.Round(columnIndexDouble, MidpointRounding.AwayFromZero);
            }
        }
        else
        {
            var columnIndexDouble = positionX / charMeasurements.CharacterWidth;
            columnIndexInt = (int)Math.Round(columnIndexDouble, MidpointRounding.AwayFromZero);
        }

        var lengthOfRow = model.GetLineLength(rowIndex);

        // Tab key column offset
        {
            var parameterForGetTabsCountOnSameRowBeforeCursor = columnIndexInt > lengthOfRow
                ? lengthOfRow
                : columnIndexInt;

            int tabsOnSameRowBeforeCursor;

            try
            {
                tabsOnSameRowBeforeCursor = model.GetTabCountOnSameLineBeforeCursor(
                    rowIndex,
                    parameterForGetTabsCountOnSameRowBeforeCursor);
            }
            catch (LuthetusTextEditorException)
            {
                tabsOnSameRowBeforeCursor = 0;
            }

            // 1 of the character width is already accounted for
            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            columnIndexInt -= extraWidthPerTabKey * tabsOnSameRowBeforeCursor;
        }

        columnIndexInt = columnIndexInt > lengthOfRow
            ? lengthOfRow
            : columnIndexInt;

        rowIndex = Math.Max(rowIndex, 0);
        columnIndexInt = Math.Max(columnIndexInt, 0);

        return (rowIndex, columnIndexInt);
    }

    private Task ThrottleApplySyntaxHighlighting(TextEditorModelModifier modelModifier)
    {
        return _throttleApplySyntaxHighlighting.PushEvent(_ =>
        {
            return modelModifier.CompilerService.ResourceWasModified(modelModifier.ResourceUri, ImmutableArray<TextEditorTextSpan>.Empty);
        });
    }
}