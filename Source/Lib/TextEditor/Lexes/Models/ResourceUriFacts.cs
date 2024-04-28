namespace Luthetus.TextEditor.RazorLib.Lexes.Models;

public class ResourceUriFacts
{
    public const string Terminal_ReservedResourceUri_Prefix = "__LUTHETUS__/__Terminal__/";
    public static readonly ResourceUri SettingsPreviewTextEditorResourceUri = new("__LUTHETUS__/__TextEditor__/preview-settings");
}