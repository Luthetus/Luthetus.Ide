using Luthetus.Ide.RazorLib.FileSystems.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.ComponentRenderers.Models;

public interface IFileFormRendererType
{
    public string FileName { get; set; }
    public bool IsDirectory { get; set; }
    public bool CheckForTemplates { get; set; }
    public Func<string, IFileTemplate?, ImmutableArray<IFileTemplate>, Task> OnAfterSubmitFunc { get; set; }
}