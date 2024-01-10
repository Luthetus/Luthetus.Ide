using Fluxor;

namespace Luthetus.Common.RazorLib.Tabs.States;

public partial record TabState
{
    public class Reducer
    {
        [ReducerMethod]
        public static TabState ReduceRegisterTabGroupAction(
            TabState inState,
            RegisterTabGroupAction registerTabGroupAction)
        {
            var outTabGroupList = inState.TabGroupList.Add(registerTabGroupAction.TabGroup);
            
            return new TabState { TabGroupList = outTabGroupList };
        }

        [ReducerMethod]
        public static TabState ReduceDisposeTabGroupAction(
            TabState inState,
            DisposeTabGroupAction disposeTabGroupAction)
        {
            var inTabGroup = inState.TabGroupList.FirstOrDefault(
                x => x.Key == disposeTabGroupAction.TabGroupKey);

            if (inTabGroup is null)
                return inState;

            var outTabGroupList = inState.TabGroupList.Remove(inTabGroup);

            return new TabState { TabGroupList = outTabGroupList };
        }
        
        [ReducerMethod]
        public static TabState ReduceSetTabEntryListAction(
            TabState inState,
            SetTabEntryListAction setTabEntryListAction)
        {
            var inTabGroup = inState.TabGroupList.FirstOrDefault(
                x => x.Key == setTabEntryListAction.TabGroupKey);

            if (inTabGroup is null)
                return inState;

            var outTabGroup = inTabGroup with { EntryList = setTabEntryListAction.TabEntryList };

            var outTabGroupList = inState.TabGroupList.Replace(
                inTabGroup,
                outTabGroup);

            return new TabState { TabGroupList = outTabGroupList };
        }
        
        [ReducerMethod]
        public static TabState ReduceSetActiveTabEntryKeyAction(
            TabState inState,
            SetActiveTabEntryKeyAction setActiveTabEntryKeyAction)
        {
            var inTabGroup = inState.TabGroupList.FirstOrDefault(
                x => x.Key == setActiveTabEntryKeyAction.TabGroupKey);

            if (inTabGroup is null)
                return inState;

            var outTabGroup = inTabGroup with { ActiveEntryKey = setActiveTabEntryKeyAction.TabEntryKey };

            var outTabGroupList = inState.TabGroupList.Replace(
                inTabGroup,
                outTabGroup);

            return new TabState { TabGroupList = outTabGroupList };
        }
    }
}
