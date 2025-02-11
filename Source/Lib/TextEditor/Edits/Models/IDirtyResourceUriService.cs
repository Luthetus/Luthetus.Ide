using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.Edits.Models;

public interface IDirtyResourceUriService
{
	public event Action DirtyResourceUriStateChanged;
	
	public DirtyResourceUriState GetDirtyResourceUriState();

    public void ReduceAddDirtyResourceUriAction(ResourceUri resourceUri);
    public void ReduceRemoveDirtyResourceUriAction(ResourceUri resourceUri);
}
