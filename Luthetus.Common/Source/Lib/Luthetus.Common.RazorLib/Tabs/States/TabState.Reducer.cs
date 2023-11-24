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
            var outTabGroupBag = inState.TabGroupBag.Add(registerTabGroupAction.TabGroup);
            
            return new TabState { TabGroupBag = outTabGroupBag };
        }

        [ReducerMethod]
        public static TabState ReduceDisposeTabGroupAction(
            TabState inState,
            DisposeTabGroupAction disposeTabGroupAction)
        {
            var inTabGroup = inState.TabGroupBag.FirstOrDefault(
                x => x.Key == disposeTabGroupAction.TabGroupKey);

            if (inTabGroup is null)
                return inState;

            var outTabGroupBag = inState.TabGroupBag.Remove(inTabGroup);

            return new TabState { TabGroupBag = outTabGroupBag };
        }
        
        [ReducerMethod]
        public static TabState ReduceSetTabEntryBagAction(
            TabState inState,
            SetTabEntryBagAction setTabEntryBagAction)
        {
            var inTabGroup = inState.TabGroupBag.FirstOrDefault(
                x => x.Key == setTabEntryBagAction.TabGroupKey);

            if (inTabGroup is null)
                return inState;

            var outTabGroup = inTabGroup with { EntryBag = setTabEntryBagAction.TabEntryBag };

            var outTabGroupBag = inState.TabGroupBag.Replace(
                inTabGroup,
                outTabGroup);

            return new TabState { TabGroupBag = outTabGroupBag };
        }
        
        [ReducerMethod]
        public static TabState ReduceSetActiveTabEntryKeyAction(
            TabState inState,
            SetActiveTabEntryKeyAction setActiveTabEntryKeyAction)
        {
            var inTabGroup = inState.TabGroupBag.FirstOrDefault(
                x => x.Key == setActiveTabEntryKeyAction.TabGroupKey);

            if (inTabGroup is null)
                return inState;

            var outTabGroup = inTabGroup with { ActiveEntryKey = setActiveTabEntryKeyAction.TabEntryKey };

            var outTabGroupBag = inState.TabGroupBag.Replace(
                inTabGroup,
                outTabGroup);

            return new TabState { TabGroupBag = outTabGroupBag };
        }
    }
}
