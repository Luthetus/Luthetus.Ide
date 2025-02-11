using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.Edits.Models;

public class DirtyResourceUriService : IDirtyResourceUriService
{
	private DirtyResourceUriState _dirtyResourceUriState = new();
	
	public event Action DirtyResourceUriStateChanged;
	
	public DirtyResourceUriState GetDirtyResourceUriState() => _dirtyResourceUriState;

    public void ReduceAddDirtyResourceUriAction(ResourceUri resourceUri)
    {
    	var inState = GetDirtyResourceUriState();
    
        if (resourceUri.Value.StartsWith(ResourceUriFacts.Terminal_ReservedResourceUri_Prefix) ||
			resourceUri.Value.StartsWith(ResourceUriFacts.Git_ReservedResourceUri_Prefix))
        {
            DirtyResourceUriStateChanged?.Invoke();
            return;
        }
       else if (resourceUri == ResourceUriFacts.SettingsPreviewTextEditorResourceUri ||
			    resourceUri == ResourceUriFacts.TestExplorerDetailsTextEditorResourceUri)
        {
            DirtyResourceUriStateChanged?.Invoke();
            return;
        }

		_dirtyResourceUriState = inState with
        {
            DirtyResourceUriList = inState.DirtyResourceUriList.Add(resourceUri)
        };
        
        DirtyResourceUriStateChanged?.Invoke();
        return;
    }

    public void ReduceRemoveDirtyResourceUriAction(ResourceUri resourceUri)
    {
    	var inState = GetDirtyResourceUriState();
    
        _dirtyResourceUriState = inState with
        {
            DirtyResourceUriList = inState.DirtyResourceUriList.Remove(resourceUri)
        };
        
        DirtyResourceUriStateChanged?.Invoke();
        return;
    }
}
