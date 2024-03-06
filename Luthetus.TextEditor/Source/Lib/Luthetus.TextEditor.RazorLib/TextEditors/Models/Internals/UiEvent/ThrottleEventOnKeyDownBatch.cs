using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;

public class ThrottleEventOnKeyDownBatch : IThrottleEvent
{
    private readonly CursorDisplay? _cursorDisplay;
    private readonly Func<ResourceUri, Key<TextEditorViewModel>, List<KeyboardEventArgs>, Func<TextEditorMenuKind, bool, Task>, TextEditorEdit> _handleAfterOnKeyDownRangeAsyncFactoryFunc;
    private readonly Action<TooltipViewModel?> _setTooltipViewModel;
    private readonly ITextEditorService _textEditorService;

    public ThrottleEventOnKeyDownBatch(
        ThrottleController throttleControllerUiEvents,
        TimeSpan uiEventsDelay,
        List<KeyboardEventArgs> keyboardEventArgsList,
        KeyboardEventArgsKind keyboardEventArgsKind,
        TextEditorViewModelDisplayOptions viewModelDisplayOptions,
        CursorDisplay? cursorDisplay,
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey,
        Func<ResourceUri, Key<TextEditorViewModel>, List<KeyboardEventArgs>, Func<TextEditorMenuKind, bool, Task>, TextEditorEdit> handleAfterOnKeyDownRangeAsyncFactoryFunc,
        Action<TooltipViewModel?> setTooltipViewModel,
        ITextEditorService textEditorService)
    {
        KeyboardEventArgsList = keyboardEventArgsList;
        KeyboardEventArgsKind = keyboardEventArgsKind;
        ViewModelDisplayOptions = viewModelDisplayOptions;
        ResourceUri = resourceUri;
        ViewModelKey = viewModelKey;
        
        _handleAfterOnKeyDownRangeAsyncFactoryFunc = handleAfterOnKeyDownRangeAsyncFactoryFunc;
        _setTooltipViewModel = setTooltipViewModel;
        _cursorDisplay = cursorDisplay;
        _textEditorService = textEditorService;
    }

    public TimeSpan ThrottleTimeSpan => TimeSpan.Zero;
    public List<KeyboardEventArgs> KeyboardEventArgsList { get; }
    public KeyboardEventArgsKind KeyboardEventArgsKind { get; }
    public TextEditorViewModelDisplayOptions ViewModelDisplayOptions { get; }
    public ResourceUri ResourceUri { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }

    public IThrottleEvent? BatchOrDefault(IThrottleEvent moreRecentEvent)
    {
        // TODO: Should this type implement BatchOrDefault, beyond just returning null?
        return null;
    }

    public Task HandleEvent(CancellationToken cancellationToken)
    {
        _textEditorService.Post(
            $"OnKeyDown_{KeyboardEventArgsKind}_Batch:{KeyboardEventArgsList.Count}",
            async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(ResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(ViewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return;

                var hasSelection = TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier);

                var shouldInvokeAfterOnKeyDownAsync = false;
                
                if (KeyboardEventArgsKind == KeyboardEventArgsKind.Command)
                {
                    shouldInvokeAfterOnKeyDownAsync = true;
                    // TODO: Batch command
                }
                else if (KeyboardEventArgsKind == KeyboardEventArgsKind.Movement)
                {
                    if (_cursorDisplay is null || _cursorDisplay.MenuKind != TextEditorMenuKind.AutoCompleteMenu)
                    {
                        // Don't do this foreach loop if the autocomplete menu is showing.
                        foreach (var keyboardEventArgs in KeyboardEventArgsList)
                        {
                            await _textEditorService.ViewModelApi.MoveCursorFactory(
                                    keyboardEventArgs,
                                    modelModifier.ResourceUri,
                                    viewModelModifier.ViewModel.ViewModelKey)
                                .Invoke(editContext)
                                .ConfigureAwait(false);
                        }
                    }

                    await (_cursorDisplay?.SetShouldDisplayMenuAsync(TextEditorMenuKind.None) ?? Task.CompletedTask);
                }
                else if (KeyboardEventArgsKind == KeyboardEventArgsKind.ContextMenu)
                {
                    // TODO: Batch context menu
                }
                else if (KeyboardEventArgsKind == KeyboardEventArgsKind.Text)
                {
                    shouldInvokeAfterOnKeyDownAsync = true;

                    _setTooltipViewModel.Invoke(null);

                    modelModifier.EditByInsertion(
                        string.Join(string.Empty, KeyboardEventArgsList.Select(x => x.Key)),
                        cursorModifierBag,
                        CancellationToken.None);
                }
                else if (KeyboardEventArgsKind == KeyboardEventArgsKind.Other)
                {
                    shouldInvokeAfterOnKeyDownAsync = true;
                    // TODO: Batch KeyboardEventArgsKind.Other
                }

                if (shouldInvokeAfterOnKeyDownAsync)
                {
                    primaryCursorModifier.ShouldRevealCursor = true;

                    var cursorDisplay = _cursorDisplay;

                    if (cursorDisplay is not null)
                    {
                        var afterOnKeyDownRangeAsyncFactory = ViewModelDisplayOptions.AfterOnKeyDownRangeAsyncFactory
                            ?? _handleAfterOnKeyDownRangeAsyncFactoryFunc;

                        await afterOnKeyDownRangeAsyncFactory.Invoke(
                                modelModifier.ResourceUri,
                                viewModelModifier.ViewModel.ViewModelKey,
                                KeyboardEventArgsList,
                                cursorDisplay.SetShouldDisplayMenuAsync)
                            .Invoke(editContext)
                            .ConfigureAwait(false);
                    }
                }
            });

        return Task.CompletedTask;
    }
}
