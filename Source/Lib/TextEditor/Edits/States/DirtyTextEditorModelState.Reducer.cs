using Fluxor;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.Edits.States;

public partial record DirtyResourceUriState
{
    public static class Reducer
    {
        [ReducerMethod]
        public static DirtyResourceUriState ReduceAddDirtyResourceUriAction(
            DirtyResourceUriState inState,
            AddDirtyResourceUriAction addDirtyResourceUriAction)
        {
            if (addDirtyResourceUriAction.ResourceUri.Value.StartsWith(ResourceUriFacts.Terminal_ReservedResourceUri_Prefix) ||
				addDirtyResourceUriAction.ResourceUri.Value.StartsWith(ResourceUriFacts.Git_ReservedResourceUri_Prefix))
            {
                return inState;
            }
           else if (addDirtyResourceUriAction.ResourceUri == ResourceUriFacts.SettingsPreviewTextEditorResourceUri ||
				    addDirtyResourceUriAction.ResourceUri == ResourceUriFacts.TestExplorerDetailsTextEditorResourceUri)
            {
                return inState;
            }

			return inState with
            {
                DirtyResourceUriList = inState.DirtyResourceUriList.Add(addDirtyResourceUriAction.ResourceUri)
            };
        }

        [ReducerMethod]
        public static DirtyResourceUriState ReduceRemoveDirtyResourceUriAction(
            DirtyResourceUriState inState,
            RemoveDirtyResourceUriAction removeDirtyResourceUriAction)
        {
            return inState with
            {
                DirtyResourceUriList = inState.DirtyResourceUriList.Remove(removeDirtyResourceUriAction.ResourceUri)
            };
        }
    }
}
