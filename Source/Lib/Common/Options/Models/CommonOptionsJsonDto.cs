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
    int? ResizeHandleWidthInPixels,
    int? ResizeHandleHeightInPixels,
    Key<ThemeRecord>? ThemeKey,
    string? FontFamily)
{
    public CommonOptionsJsonDto()
        : this(
        	FontSizeInPixels: null,
        	IconSizeInPixels: null,
        	ResizeHandleWidthInPixels: null,
        	ResizeHandleHeightInPixels: null,
        	ThemeKey: null,
        	FontFamily: null)
    {
    }

    public CommonOptionsJsonDto(CommonOptions options)
        : this(
              options.FontSizeInPixels,
              options.IconSizeInPixels,
              options.ResizeHandleWidthInPixels,
              options.ResizeHandleHeightInPixels,
              options.ThemeKey,
              options.FontFamily)
    {
        
    }
}