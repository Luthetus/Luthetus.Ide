using Luthetus.Ide.ClassLib.FileTemplates;

namespace Luthetus.Ide.ClassLib.ComponentRenderers.Types;

public interface IFileFormRendererType
{
    public string FileName { get; set; }
    public bool IsDirectory { get; set; }
    public bool CheckForTemplates { get; set; }
    public Action<string, IFileTemplate?, ImmutableArray<IFileTemplate>> OnAfterSubmitAction { get; set; }
}