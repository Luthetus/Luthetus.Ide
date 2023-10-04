using Fluxor;

namespace Luthetus.Common.RazorLib.Themes.States;

public partial record ThemeState
{
    private class Reducer
    {
        [ReducerMethod]
        public ThemeState ReduceRegisterAction(
            ThemeState inState,
            RegisterAction registerAction)
        {
            var inTheme = inState.ThemeBag.FirstOrDefault(
                x => x.Key == registerAction.Theme.Key);

            if (inTheme is not null)
                return inState;

            var outThemeBag = inState.ThemeBag.Add(registerAction.Theme);

            return new ThemeState { ThemeBag = outThemeBag };
        }

        [ReducerMethod]
        public ThemeState ReduceDisposeAction(
            ThemeState inState,
            DisposeAction disposeAction)
        {
            var inTheme = inState.ThemeBag.FirstOrDefault(
                x => x.Key == disposeAction.ThemeKey);

            if (inTheme is null)
                return inState;

            var outThemeBag = inState.ThemeBag.Remove(inTheme);

            return new ThemeState { ThemeBag = outThemeBag };
        }
    }
}