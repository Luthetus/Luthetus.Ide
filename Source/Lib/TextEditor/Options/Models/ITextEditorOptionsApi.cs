using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.Common.RazorLib.Themes.Models;

namespace Luthetus.TextEditor.RazorLib.Options.Models;

public interface ITextEditorOptionsApi
{
    public void ShowSettingsDialog(bool? isResizableOverride = null, string? cssClassString = null);
    public void ShowFindAllDialog(bool? isResizableOverride = null, string? cssClassString = null);
    public void SetCursorWidth(double cursorWidthInPixels, bool updateStorage = true);
    public void SetFontFamily(string? fontFamily, bool updateStorage = true);
    public void SetFontSize(int fontSizeInPixels, bool updateStorage = true);
    public void SetHeight(int? heightInPixels, bool updateStorage = true);
    public void SetKeymap(Keymap keymap, bool updateStorage = true);
    public void SetShowNewlines(bool showNewlines, bool updateStorage = true);
    public void SetUseMonospaceOptimizations(bool useMonospaceOptimizations, bool updateStorage = true);
    public void SetShowWhitespace(bool showWhitespace, bool updateStorage = true);
    /// <summary>This is setting the TextEditor's theme specifically. This is not to be confused with the AppOptions Themes which get applied at an application level. <br /><br /> This allows for a "DarkTheme-Application" that has a "LightTheme-TextEditor"</summary>
    public void SetTheme(ThemeRecord theme, bool updateStorage = true);
    public void SetRenderStateKey(Key<RenderState> renderStateKey);
    public Task SetFromLocalStorageAsync();
    public void WriteToStorage();

    /// <summary>
    /// One should store the result of invoking this method in a variable, then reference that variable.
    /// If one continually invokes this, there is no guarantee that the data had not changed
    /// since the previous invocation.
    /// </summary>
    public TextEditorOptions GetOptions();
}
