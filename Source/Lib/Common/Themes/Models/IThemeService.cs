using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Themes.Models;

public interface IThemeService
{
	public event Action? ThemeStateChanged;
	
	public ThemeState GetThemeState();

    public void ReduceRegisterAction(ThemeRecord theme);
    public void ReduceDisposeAction(Key<ThemeRecord> themeKey);
}
