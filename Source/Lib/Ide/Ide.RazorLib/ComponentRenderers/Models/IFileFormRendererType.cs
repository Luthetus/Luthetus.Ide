using Luthetus.Ide.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.ComponentRenderers.Models;

public interface IFileFormRendererType
{
    public string FileName { get; set; }
    public bool IsDirectory { get; set; }
    public bool CheckForTemplates { get; set; }
    public Func<string, IFileTemplate?, List<IFileTemplate>, Task> OnAfterSubmitFunc { get; set; }
}