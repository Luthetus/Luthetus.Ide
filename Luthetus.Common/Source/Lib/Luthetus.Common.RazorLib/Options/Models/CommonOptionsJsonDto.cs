using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Themes.Models;

namespace Luthetus.Common.RazorLib.Options.Models;

/// <summary>
/// This type needs to exist so the <see cref="CommonOptions"/> properties can be nullable, as in they were not
/// already in local storage. Whereas throughout the app they should never be null.
/// </summary>
public record CommonOptionsJsonDto(
    int? FontSizeInPixels,
    int? IconSizeInPixels,
    Key<ThemeRecord>? ThemeKey,
    string? FontFamily)
{
    public CommonOptionsJsonDto()
        : this(null, null, null, null)
    {
    }

    public CommonOptionsJsonDto(CommonOptions options)
        : this(
              options.FontSizeInPixels,
              options.IconSizeInPixels,
              options.ThemeKey,
              options.FontFamily)
    {
        
    }
}