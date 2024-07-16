namespace Luthetus.CompilerServices.RazorLib.ComponentRenderers.Models;

public class CompilerServicesComponentRenderers : ICompilerServicesComponentRenderers
{
	public CompilerServicesComponentRenderers(
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
