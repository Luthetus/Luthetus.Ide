using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Extensions.DotNet.ComponentRenderers.Models;

public interface IRemoveCSharpProjectFromSolutionRendererType
{
	public IAbsolutePath AbsolutePath { get; set; }
	public Func<IAbsolutePath, Task> OnAfterSubmitFunc { get; set; }
}