namespace Luthetus.Extensions.DotNet.CompilerServices.Models;

public interface ICompilerServiceExplorerService
{
	public event Action? CompilerServiceExplorerStateChanged;
	
	public CompilerServiceExplorerState GetCompilerServiceExplorerState();
	
    public void ReduceNewAction(Func<CompilerServiceExplorerState, CompilerServiceExplorerState> newFunc);
}
