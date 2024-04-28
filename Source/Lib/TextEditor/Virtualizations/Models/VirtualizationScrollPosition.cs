namespace Luthetus.TextEditor.RazorLib.Virtualizations.Models;

/// <summary>
/// <see cref="ScrollWidthInPixels"/> may represent either
/// the JavaScript value of checking scrollWidth on a scrollable element.
/// <br/>
/// OR
/// <br/>
/// <see cref="ScrollWidthInPixels"/> may represent the maximum width
/// of a faked scrollbar (overflow: hidden for example)
/// <br/>
/// ALSO
/// <br/>
/// The comment about <see cref="ScrollWidthInPixels"/> also applies to
/// the height for <see cref="ScrollHeightInPixels"/>
/// </summary>
public record VirtualizationScrollPosition(
    double ScrollLeftInPixels,
    double ScrollTopInPixels,
    double ScrollWidthInPixels,
    double ScrollHeightInPixels);