namespace Luthetus.Extensions.DotNet.CompilerServices.Models;

public class CompilerServiceExplorerService : ICompilerServiceExplorerService
{
	private CompilerServiceExplorerState _compilerServiceExplorerState = new();
	
	public event Action? CompilerServiceExplorerStateChanged;
	
	public CompilerServiceExplorerState GetCompilerServiceExplorerState() => _compilerServiceExplorerState;
	
    public void ReduceNewAction(Func<CompilerServiceExplorerState, CompilerServiceExplorerState> newFunc)
    {
    	var inState = GetCompilerServiceExplorerState();
    
        _compilerServiceExplorerState = newFunc.Invoke(inState);
        
        CompilerServiceExplorerStateChanged?.Invoke();
        return;
    }
}
