namespace Luthetus.TextEditor.RazorLib.Lexers.Models;

public class ResourceUriFacts
{
    public const string Terminal_ReservedResourceUri_Prefix = "__LUTHETUS__/__Terminal__/";
    public const string Git_ReservedResourceUri_Prefix = "__LUTHETUS__/__Git__/";
    public static readonly ResourceUri SettingsPreviewTextEditorResourceUri = new("__LUTHETUS__/__TextEditor__/preview-settings");
    public static readonly ResourceUri TestExplorerDetailsTextEditorResourceUri = new("__LUTHETUS__/__TestExplorer__/details");
}