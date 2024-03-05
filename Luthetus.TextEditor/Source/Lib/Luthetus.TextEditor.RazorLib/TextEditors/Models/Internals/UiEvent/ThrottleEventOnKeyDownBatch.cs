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
        if (moreRecentEvent is ThrottleEventOnKeyDown moreRecentEventOnKeyDown)
        {
            KeyboardEventArgsList.Insert(0, moreRecentEventOnKeyDown.KeyboardEventArgs);
            return this;
        }

        if (moreRecentEvent is ThrottleEventOnKeyDownBatch moreRecentEventOnKeyDownBatch)
        {
            moreRecentEventOnKeyDownBatch.KeyboardEventArgsList.AddRange(KeyboardEventArgsList);
            return moreRecentEventOnKeyDownBatch;
        }

        return null;
    }

    public Task HandleEvent(CancellationToken cancellationToken)
    {
        _textEditorService.Post(
            nameof(ThrottleEventOnKeyDownBatch),
            async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(ResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(ViewModelKey);

                if (modelModifier is null || viewModelModifier is null)
                    return;

                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);

                if (cursorModifierBag is null)
                    return;

                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (primaryCursorModifier is null)
                    return;

                var hasSelection = TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier);

                if (KeyboardEventArgsKind == KeyboardEventArgsKind.Movement)
                {
                    // TODO: Batch movement
                }
                else if (KeyboardEventArgsKind == KeyboardEventArgsKind.ContextMenu)
                {
                    // TODO: Batch context menu
                }
                else if (KeyboardEventArgsKind == KeyboardEventArgsKind.Command || KeyboardEventArgsKind == KeyboardEventArgsKind.Text || KeyboardEventArgsKind == KeyboardEventArgsKind.Other)
                {
                    if (KeyboardEventArgsKind == KeyboardEventArgsKind.Command)
                    {
                        // TODO: Bath command
                    }
                    else if (KeyboardEventArgsKind == KeyboardEventArgsKind.Text)
                    {
                        _setTooltipViewModel.Invoke(null);

                        modelModifier.EditByInsertion(
                            string.Join(string.Empty, KeyboardEventArgsList.Select(x => x.Key)),
                            cursorModifierBag,
                            CancellationToken.None);
                    }

                    primaryCursorModifier.ShouldRevealCursor = true;

                    var cursorDisplay = _cursorDisplay;

                    if (cursorDisplay is not null)
                    {
                        var afterOnKeyDownRangeAsyncFactory = ViewModelDisplayOptions.AfterOnKeyDownRangeAsyncFactory ?? _handleAfterOnKeyDownRangeAsyncFactoryFunc;

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
