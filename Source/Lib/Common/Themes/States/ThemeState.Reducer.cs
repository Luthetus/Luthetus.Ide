using Fluxor;
using Luthetus.Common.RazorLib.Themes.Models;

namespace Luthetus.Common.RazorLib.Themes.States;

public partial record ThemeState
{
    public class Reducer
    {
        [ReducerMethod]
        public ThemeState ReduceRegisterAction(
            ThemeState inState,
            RegisterAction registerAction)
        {
            var inTheme = inState.ThemeList.FirstOrDefault(
                x => x.Key == registerAction.Theme.Key);

            if (inTheme is not null)
                return inState;

            var outThemeList = new List<ThemeRecord>(inState.ThemeList);
            outThemeList.Add(registerAction.Theme);

            return new ThemeState { ThemeList = outThemeList };
        }

        [ReducerMethod]
        public ThemeState ReduceDisposeAction(
            ThemeState inState,
            DisposeAction disposeAction)
        {
            var inTheme = inState.ThemeList.FirstOrDefault(
                x => x.Key == disposeAction.ThemeKey);

            if (inTheme is null)
                return inState;

            var outThemeList = new List<ThemeRecord>(inState.ThemeList);
            outThemeList.Remove(inTheme);

            return new ThemeState { ThemeList = outThemeList };
        }
    }
}