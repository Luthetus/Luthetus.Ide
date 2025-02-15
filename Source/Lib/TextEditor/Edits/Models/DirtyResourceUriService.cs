using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.Edits.Models;

public class DirtyResourceUriService : IDirtyResourceUriService
{
    private readonly object _stateModificationLock = new();

    private DirtyResourceUriState _dirtyResourceUriState = new();
	
	public event Action DirtyResourceUriStateChanged;
	
	public DirtyResourceUriState GetDirtyResourceUriState() => _dirtyResourceUriState;

    public void AddDirtyResourceUri(ResourceUri resourceUri)
    {
        lock (_stateModificationLock)
        {
            var inState = GetDirtyResourceUriState();

            if (resourceUri.Value.StartsWith(ResourceUriFacts.Terminal_ReservedResourceUri_Prefix) ||
                resourceUri.Value.StartsWith(ResourceUriFacts.Git_ReservedResourceUri_Prefix))
            {
                goto finalize;
            }
            else if (resourceUri == ResourceUriFacts.SettingsPreviewTextEditorResourceUri ||
                     resourceUri == ResourceUriFacts.TestExplorerDetailsTextEditorResourceUri)
            {
                goto finalize;
            }

            _dirtyResourceUriState = inState with
            {
                DirtyResourceUriList = inState.DirtyResourceUriList.Add(resourceUri)
            };

            goto finalize;
        }

        finalize:
        DirtyResourceUriStateChanged?.Invoke();
    }

    public void RemoveDirtyResourceUri(ResourceUri resourceUri)
    {
        lock (_stateModificationLock)
        {
            var inState = GetDirtyResourceUriState();

            _dirtyResourceUriState = inState with
            {
                DirtyResourceUriList = inState.DirtyResourceUriList.Remove(resourceUri)
            };

            goto finalize;
        }

        finalize:
        DirtyResourceUriStateChanged?.Invoke();
    }
}
