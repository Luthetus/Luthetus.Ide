using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Themes.Models;

public class ThemeService : IThemeService
{
	private ThemeState _themeState = new();
	
	public event Action? ThemeStateChanged;
	
	public ThemeState GetThemeState() => _themeState;

    public void ReduceRegisterAction(ThemeRecord theme)
    {
    	var inState = GetThemeState();
    
        var inTheme = inState.ThemeList.FirstOrDefault(
            x => x.Key == theme.Key);

        if (inTheme is not null)
        {
            ThemeStateChanged?.Invoke();
            return;
        }

        var outThemeList = new List<ThemeRecord>(inState.ThemeList);
        outThemeList.Add(theme);

        _themeState = new ThemeState { ThemeList = outThemeList };
        ThemeStateChanged?.Invoke();
        return;
    }

    public void ReduceDisposeAction(Key<ThemeRecord> themeKey)
    {
    	var inState = GetThemeState();
    
        var inTheme = inState.ThemeList.FirstOrDefault(
            x => x.Key == themeKey);

        if (inTheme is null)
        {
            ThemeStateChanged?.Invoke();
            return;
        }

        var outThemeList = new List<ThemeRecord>(inState.ThemeList);
        outThemeList.Remove(inTheme);

        _themeState = new ThemeState { ThemeList = outThemeList };
        ThemeStateChanged?.Invoke();
        return;
    }
}
