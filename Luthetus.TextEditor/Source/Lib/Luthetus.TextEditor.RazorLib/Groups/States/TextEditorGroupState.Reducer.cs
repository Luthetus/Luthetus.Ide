using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.Groups.States;

public partial class TextEditorGroupState
{
    public class Reducer
    {
        [ReducerMethod]
        public static TextEditorGroupState ReduceRegisterAction(
            TextEditorGroupState inState,
            RegisterAction registerAction)
        {
            var inGroup = inState.GroupBag.FirstOrDefault(
                x => x.GroupKey == registerAction.Group.GroupKey);

            if (inGroup is not null)
                return inState;

            var outGroupBag = inState.GroupBag.Add(registerAction.Group);

            return new TextEditorGroupState
            {
                GroupBag = outGroupBag
            };
        }

        [ReducerMethod]
        public static TextEditorGroupState ReduceAddViewModelToGroupAction(
            TextEditorGroupState inState,
            AddViewModelToGroupAction addViewModelToGroupAction)
        {
            var inGroup = inState.GroupBag.FirstOrDefault(
                x => x.GroupKey == addViewModelToGroupAction.GroupKey);

            if (inGroup is null)
                return inState;

            if (inGroup.ViewModelKeyBag.Contains(addViewModelToGroupAction.ViewModelKey))
                return inState;

            var outViewModelKeyBag = inGroup.ViewModelKeyBag.Add(
                addViewModelToGroupAction.ViewModelKey);

            var outGroup = inGroup with
            {
                ViewModelKeyBag = outViewModelKeyBag
            };

            if (outGroup.ViewModelKeyBag.Count == 1)
            {
                outGroup = outGroup with
                {
                    ActiveViewModelKey = addViewModelToGroupAction.ViewModelKey
                };
            }

            var outGroupBag = inState.GroupBag.Replace(inGroup, outGroup);

            return new TextEditorGroupState
            {
                GroupBag = outGroupBag
            };
        }

        [ReducerMethod]
        public static TextEditorGroupState ReduceRemoveViewModelFromGroupAction(
            TextEditorGroupState inState,
            RemoveViewModelFromGroupAction removeViewModelFromGroupAction)
        {
            var inGroup = inState.GroupBag.FirstOrDefault(
                x => x.GroupKey == removeViewModelFromGroupAction.GroupKey);

            if (inGroup is null)
                return inState;

            var indexOfViewModelKeyToRemove = inGroup.ViewModelKeyBag.FindIndex(
                x => x == removeViewModelFromGroupAction.ViewModelKey);

            if (indexOfViewModelKeyToRemove == -1)
                return inState;

            var nextViewModelKeyBag = inGroup.ViewModelKeyBag.Remove(
                removeViewModelFromGroupAction.ViewModelKey);

            // This variable is done for renaming
            var activeViewModelKeyIndex = indexOfViewModelKeyToRemove;

            // If last item in list
            if (activeViewModelKeyIndex >= inGroup.ViewModelKeyBag.Count - 1)
            {
                activeViewModelKeyIndex--;
            }
            else
            {
                // ++ operation because nothing yet has been removed.
                // The new active TextEditor is set prior to actually removing the current active TextEditor.
                activeViewModelKeyIndex++;
            }

            Key<TextEditorViewModel> nextActiveTextEditorModelKey;

            // If removing the active will result in empty list set the active as an Empty TextEditorViewModelKey
            if (inGroup.ViewModelKeyBag.Count - 1 == 0)
                nextActiveTextEditorModelKey = Key<TextEditorViewModel>.Empty;
            else
                nextActiveTextEditorModelKey = inGroup.ViewModelKeyBag[activeViewModelKeyIndex];
            
            var outGroupBag = inState.GroupBag.Replace(inGroup, inGroup with
            {
                ViewModelKeyBag = nextViewModelKeyBag,
                ActiveViewModelKey = nextActiveTextEditorModelKey
            });

            return new TextEditorGroupState
            {
                GroupBag = outGroupBag
            };
        }

        [ReducerMethod]
        public static TextEditorGroupState ReduceSetActiveViewModelOfGroupAction(
            TextEditorGroupState inState,
            SetActiveViewModelOfGroupAction setActiveViewModelOfGroupAction)
        {
            var inGroup = inState.GroupBag.FirstOrDefault(
                x => x.GroupKey == setActiveViewModelOfGroupAction.GroupKey);

            if (inGroup is null)
                return inState;

            var outGroupBag = inState.GroupBag.Replace(inGroup, inGroup with
            {
                ActiveViewModelKey = setActiveViewModelOfGroupAction.ViewModelKey
            });

            return new TextEditorGroupState
            {
                GroupBag = outGroupBag
            };
        }

        [ReducerMethod]
        public static TextEditorGroupState ReduceDisposeAction(
            TextEditorGroupState inState,
            DisposeAction disposeAction)
        {
            var inGroup = inState.GroupBag.FirstOrDefault(
                x => x.GroupKey == disposeAction.GroupKey);

            if (inGroup is null)
                return inState;

            var outGroupBag = inState.GroupBag.Remove(inGroup);

            return new TextEditorGroupState
            {
                GroupBag = outGroupBag
            };
        }
    }
}