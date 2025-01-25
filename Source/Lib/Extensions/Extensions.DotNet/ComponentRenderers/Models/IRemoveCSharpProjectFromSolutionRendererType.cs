using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Extensions.DotNet.ComponentRenderers.Models;

public interface IRemoveCSharpProjectFromSolutionRendererType
{
	public AbsolutePath AbsolutePath { get; set; }
	public Func<AbsolutePath, Task> OnAfterSubmitFunc { get; set; }
}