using Fluxor;
using Luthetus.Common.RazorLib.Themes.States;

namespace Luthetus.Common.RazorLib.Themes.Models;

public class ThemeService : IThemeService
{
    private readonly IDispatcher _dispatcher;

    public ThemeService(
        IState<ThemeState> themeStateWrap,
        IDispatcher dispatcher)
    {
        ThemeStateWrap = themeStateWrap;
        _dispatcher = dispatcher;
    }

    public bool IsEnabled { get; }
    public IState<ThemeState> ThemeStateWrap { get; }
}