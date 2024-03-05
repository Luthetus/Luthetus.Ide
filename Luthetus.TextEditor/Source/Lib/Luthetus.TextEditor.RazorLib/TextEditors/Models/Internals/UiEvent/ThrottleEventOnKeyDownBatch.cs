using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;

public class ThrottleEventOnKeyDownBatch : IThrottleEvent
{
    public ThrottleEventOnKeyDownBatch(
        List<KeyboardEventArgs> keyboardEventArgsList, ResourceUri resourceUri, Key<TextEditorViewModel> viewModelKey)
    {
        KeyboardEventArgsList = keyboardEventArgsList;
        ResourceUri = resourceUri;
        ViewModelKey = viewModelKey;
    }

    public TimeSpan ThrottleTimeSpan => TimeSpan.Zero;
    public List<KeyboardEventArgs> KeyboardEventArgsList { get; }
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
        throw new NotImplementedException();
    }
}
