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
            var inGroup = inState.GroupList.FirstOrDefault(
                x => x.GroupKey == registerAction.Group.GroupKey);

            if (inGroup is not null)
                return inState;

            var outGroupList = inState.GroupList.Add(registerAction.Group);

            return new TextEditorGroupState
            {
                GroupList = outGroupList
            };
        }

        [ReducerMethod]
        public static TextEditorGroupState ReduceAddViewModelToGroupAction(
            TextEditorGroupState inState,
            AddViewModelToGroupAction addViewModelToGroupAction)
        {
            var inGroup = inState.GroupList.FirstOrDefault(
                x => x.GroupKey == addViewModelToGroupAction.GroupKey);

            if (inGroup is null)
                return inState;

            if (inGroup.ViewModelKeyList.Contains(addViewModelToGroupAction.ViewModelKey))
                return inState;

            var outViewModelKeyList = inGroup.ViewModelKeyList.Add(
                addViewModelToGroupAction.ViewModelKey);

            var outGroup = inGroup with
            {
                ViewModelKeyList = outViewModelKeyList
            };

            if (outGroup.ViewModelKeyList.Count == 1)
            {
                outGroup = outGroup with
                {
                    ActiveViewModelKey = addViewModelToGroupAction.ViewModelKey
                };
            }

            var outGroupList = inState.GroupList.Replace(inGroup, outGroup);

            return new TextEditorGroupState
            {
                GroupList = outGroupList
            };
        }

        [ReducerMethod]
        public static TextEditorGroupState ReduceRemoveViewModelFromGroupAction(
            TextEditorGroupState inState,
            RemoveViewModelFromGroupAction removeViewModelFromGroupAction)
        {
            var inGroup = inState.GroupList.FirstOrDefault(
                x => x.GroupKey == removeViewModelFromGroupAction.GroupKey);

            if (inGroup is null)
                return inState;

            var indexOfViewModelKeyToRemove = inGroup.ViewModelKeyList.FindIndex(
                x => x == removeViewModelFromGroupAction.ViewModelKey);

            if (indexOfViewModelKeyToRemove == -1)
                return inState;

            var nextViewModelKeyList = inGroup.ViewModelKeyList.Remove(
                removeViewModelFromGroupAction.ViewModelKey);

            // This variable is done for renaming
            var activeViewModelKeyIndex = indexOfViewModelKeyToRemove;

            // If last item in list
            if (activeViewModelKeyIndex >= inGroup.ViewModelKeyList.Count - 1)
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
            if (inGroup.ViewModelKeyList.Count - 1 == 0)
                nextActiveTextEditorModelKey = Key<TextEditorViewModel>.Empty;
            else
                nextActiveTextEditorModelKey = inGroup.ViewModelKeyList[activeViewModelKeyIndex];
            
            var outGroupList = inState.GroupList.Replace(inGroup, inGroup with
            {
                ViewModelKeyList = nextViewModelKeyList,
                ActiveViewModelKey = nextActiveTextEditorModelKey
            });

            return new TextEditorGroupState
            {
                GroupList = outGroupList
            };
        }

        [ReducerMethod]
        public static TextEditorGroupState ReduceSetActiveViewModelOfGroupAction(
            TextEditorGroupState inState,
            SetActiveViewModelOfGroupAction setActiveViewModelOfGroupAction)
        {
            var inGroup = inState.GroupList.FirstOrDefault(
                x => x.GroupKey == setActiveViewModelOfGroupAction.GroupKey);

            if (inGroup is null)
                return inState;

            var outGroupList = inState.GroupList.Replace(inGroup, inGroup with
            {
                ActiveViewModelKey = setActiveViewModelOfGroupAction.ViewModelKey
            });

            return new TextEditorGroupState
            {
                GroupList = outGroupList
            };
        }

        [ReducerMethod]
        public static TextEditorGroupState ReduceDisposeAction(
            TextEditorGroupState inState,
            DisposeAction disposeAction)
        {
            var inGroup = inState.GroupList.FirstOrDefault(
                x => x.GroupKey == disposeAction.GroupKey);

            if (inGroup is null)
                return inState;

            var outGroupList = inState.GroupList.Remove(inGroup);

            return new TextEditorGroupState
            {
                GroupList = outGroupList
            };
        }
    }
}