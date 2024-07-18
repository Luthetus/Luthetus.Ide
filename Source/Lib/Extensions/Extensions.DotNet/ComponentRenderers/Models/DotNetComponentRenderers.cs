namespace Luthetus.Extensions.DotNet.ComponentRenderers.Models;

public class DotNetComponentRenderers : IDotNetComponentRenderers
{
	public DotNetComponentRenderers(
		Type nuGetPackageManagerRendererType,
		Type removeCSharpProjectFromSolutionRendererType,
		CompilerServicesTreeViews compilerServicesTreeViews)
	{
		NuGetPackageManagerRendererType = nuGetPackageManagerRendererType;
		RemoveCSharpProjectFromSolutionRendererType = removeCSharpProjectFromSolutionRendererType;
		CompilerServicesTreeViews = compilerServicesTreeViews;
	}

	public Type NuGetPackageManagerRendererType { get; }
	public Type RemoveCSharpProjectFromSolutionRendererType { get; }
	public CompilerServicesTreeViews CompilerServicesTreeViews { get; }
}
