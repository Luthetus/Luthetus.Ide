namespace Luthetus.Extensions.DotNet.ComponentRenderers.Models;

public interface ICompilerServicesComponentRenderers
{
	public Type NuGetPackageManagerRendererType { get; }
	public Type RemoveCSharpProjectFromSolutionRendererType { get; }
	public CompilerServicesTreeViews CompilerServicesTreeViews { get; }
}
