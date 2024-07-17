namespace Luthetus.Extensions.DotNet.ComponentRenderers.Models;

public interface IDotNetComponentRenderers
{
	public Type NuGetPackageManagerRendererType { get; }
	public Type RemoveCSharpProjectFromSolutionRendererType { get; }
	public CompilerServicesTreeViews CompilerServicesTreeViews { get; }
}
