using Fluxor;
using Luthetus.Common.RazorLib.Themes.States;

namespace Luthetus.Common.RazorLib.Themes.Models;

public class ThemeService : IThemeService
{
    private readonly IDispatcher _dispatcher;

    public ThemeService(
        bool isEnabled,
        IState<ThemeState> themeStateWrap,
        IDispatcher dispatcher)
    {
        IsEnabled = isEnabled;
        ThemeStateWrap = themeStateWrap;
        _dispatcher = dispatcher;
    }

    public bool IsEnabled { get; }
    public IState<ThemeState> ThemeStateWrap { get; }
}