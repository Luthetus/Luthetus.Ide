using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.TextEditor.RazorLib.Edits.States;

public partial record DirtyResourceUriState
{
    public static class Reducer
    {
        public static DirtyResourceUriState ReduceAddDirtyResourceUriAction(
            DirtyResourceUriState inState,
            AddDirtyResourceUriAction addDirtyResourceUriAction)
        {
            return inState with
            {
                DirtyResourceUriList = inState.DirtyResourceUriList.Add(addDirtyResourceUriAction.ResourceUri)
            };
        }
        
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
