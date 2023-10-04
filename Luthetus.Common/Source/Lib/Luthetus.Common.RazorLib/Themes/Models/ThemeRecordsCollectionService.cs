using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Themes.States;

namespace Luthetus.Common.RazorLib.Themes.Models;

public class ThemeRecordsCollectionService : IThemeRecordsCollectionService
{
    private readonly IDispatcher _dispatcher;

    public ThemeRecordsCollectionService(
        IState<ThemeState> themeStateWrap,
        IDispatcher dispatcher)
    {
        ThemeStateWrap = themeStateWrap;
        _dispatcher = dispatcher;
    }

    public IState<ThemeState> ThemeStateWrap { get; }

    public void RegisterThemeRecord(ThemeRecord themeRecord)
    {
        _dispatcher.Dispatch(new ThemeState.RegisterAction(themeRecord));
    }

    public void DisposeThemeRecord(Key<ThemeRecord> themeKey)
    {
        _dispatcher.Dispatch(new ThemeState.DisposeAction(themeKey));
    }
}