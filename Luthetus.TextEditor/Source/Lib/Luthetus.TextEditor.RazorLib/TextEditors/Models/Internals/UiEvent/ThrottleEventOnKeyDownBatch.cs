using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;

public class ThrottleEventOnKeyDownBatch : IThrottleEvent
{
    private readonly TextEditorViewModelDisplay.TextEditorEvents _events;

    public ThrottleEventOnKeyDownBatch(
        TextEditorViewModelDisplay.TextEditorEvents events,
        List<KeyboardEventArgs> keyboardEventArgsList,
        KeyboardEventArgsKind keyboardEventArgsKind,
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey)
    {
        _events = events;

        KeyboardEventArgsList = keyboardEventArgsList;
        KeyboardEventArgsKind = keyboardEventArgsKind;
        ResourceUri = resourceUri;
        ViewModelKey = viewModelKey;
    }

    public TimeSpan ThrottleTimeSpan => TimeSpan.Zero;
    public List<KeyboardEventArgs> KeyboardEventArgsList { get; }
    public KeyboardEventArgsKind KeyboardEventArgsKind { get; }
    public ResourceUri ResourceUri { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }

    public IThrottleEvent? BatchOrDefault(IThrottleEvent moreRecentEvent)
    {
        // TODO: Should this type implement BatchOrDefault, beyond just returning null?
        return null;
    }

    public Task HandleEvent(CancellationToken cancellationToken)
    {
        _events.TextEditorService.Post(
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
                    if (_events.CursorDisplay is null || _events.CursorDisplay.MenuKind != TextEditorMenuKind.AutoCompleteMenu)
                    {
                        // Don't do this foreach loop if the autocomplete menu is showing.
                        foreach (var keyboardEventArgs in KeyboardEventArgsList)
                        {
                            await _events.TextEditorService.ViewModelApi.MoveCursorFactory(
                                    keyboardEventArgs,
                                    modelModifier.ResourceUri,
                                    viewModelModifier.ViewModel.ViewModelKey)
                                .Invoke(editContext)
                                .ConfigureAwait(false);
                        }
                    }

                    await (_events.CursorDisplay?.SetShouldDisplayMenuAsync(TextEditorMenuKind.None) ?? Task.CompletedTask);
                }
                else if (KeyboardEventArgsKind == KeyboardEventArgsKind.ContextMenu)
                {
                    // TODO: Batch context menu
                }
                else if (KeyboardEventArgsKind == KeyboardEventArgsKind.Text)
                {
                    shouldInvokeAfterOnKeyDownAsync = true;

                    _events.TooltipViewModel = null;

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

                    var cursorDisplay = _events.CursorDisplay;

                    if (cursorDisplay is not null)
                    {
                        await _events.HandleAfterOnKeyDownRangeAsyncFactory(
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
