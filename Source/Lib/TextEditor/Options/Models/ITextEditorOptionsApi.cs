using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.Common.RazorLib.Themes.Models;

namespace Luthetus.TextEditor.RazorLib.Options.Models;

public interface ITextEditorOptionsApi
{
    public void SetCursorWidth(double cursorWidthInPixels);
    public void SetFontFamily(string? fontFamily);
    public void SetFontSize(int fontSizeInPixels);
    public Task SetFromLocalStorageAsync();
    public void SetHeight(int? heightInPixels);
    public void SetKeymap(Keymap keymap);
    public void SetShowNewlines(bool showNewlines);
    public void SetUseMonospaceOptimizations(bool useMonospaceOptimizations);
    public void SetShowWhitespace(bool showWhitespace);
    /// <summary>This is setting the TextEditor's theme specifically. This is not to be confused with the AppOptions Themes which get applied at an application level. <br /><br /> This allows for a "DarkTheme-Application" that has a "LightTheme-TextEditor"</summary>
    public void SetTheme(ThemeRecord theme);
    public void ShowSettingsDialog(bool? isResizableOverride = null, string? cssClassString = null);
    public void ShowFindAllDialog(bool? isResizableOverride = null, string? cssClassString = null);
    public void WriteToStorage();
    public void SetRenderStateKey(Key<RenderState> renderStateKey);

    /// <summary>
    /// One should store the result of invoking this method in a variable, then reference that variable.
    /// If one continually invokes this, there is no guarantee that the data had not changed
    /// since the previous invocation.
    /// </summary>
    public TextEditorOptions GetOptions();
}
