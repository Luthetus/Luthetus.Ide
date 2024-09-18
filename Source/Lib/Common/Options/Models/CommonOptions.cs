using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Themes.Models;

namespace Luthetus.Common.RazorLib.Options.Models;

public record CommonOptions(
    int FontSizeInPixels,
    int IconSizeInPixels,
    int ResizeHandleWidthInPixels,
    int ResizeHandleHeightInPixels,
    Key<ThemeRecord> ThemeKey,
    string? FontFamily,
    bool ShowPanelTitles);
