using Luthetus.Ide.RazorLib.FileSystemCase.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.ComponentRenderersCase.Models;

public interface IFileFormRendererType
{
    public string FileName { get; set; }
    public bool IsDirectory { get; set; }
    public bool CheckForTemplates { get; set; }
    public Action<string, IFileTemplate?, ImmutableArray<IFileTemplate>> OnAfterSubmitAction { get; set; }
}