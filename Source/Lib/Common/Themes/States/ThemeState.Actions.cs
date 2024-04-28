using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Themes.Models;

namespace Luthetus.Common.RazorLib.Themes.States;

public partial record ThemeState
{
    public record RegisterAction(ThemeRecord Theme);
    public record DisposeAction(Key<ThemeRecord> ThemeKey);
}