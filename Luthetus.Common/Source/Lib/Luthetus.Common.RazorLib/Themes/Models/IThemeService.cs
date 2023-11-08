using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Themes.States;

namespace Luthetus.Common.RazorLib.Themes.Models;

public interface IThemeService
{
    public IState<ThemeState> ThemeStateWrap { get; }

    public void RegisterThemeRecord(ThemeRecord themeRecord);
    public void DisposeThemeRecord(Key<ThemeRecord> themeKey);
}