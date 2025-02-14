using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Reflectives.Models;

public class ReflectiveService : IReflectiveService
{
	private ReflectiveState _reflectiveState = new();
	
	public event Action? ReflectiveStateChanged;
	
	public ReflectiveState GetReflectiveState() => _reflectiveState;
	
	public ReflectiveModel GetReflectiveModel(Key<ReflectiveModel> reflectiveModelKey) =>
		_reflectiveState.ReflectiveModelList.FirstOrDefault(x => x.Key == reflectiveModelKey);
    
    public void ReduceRegisterAction(
        ReflectiveModel entry,
        int providedInsertionIndex)
    {
    	var inState = GetReflectiveState();
    
        if (inState.ReflectiveModelList.Any(x => x.Key == entry.Key))
        {
            ReflectiveStateChanged?.Invoke();
            return;
        }

        var actualInsertionIndex = 0;

        if (providedInsertionIndex >= 0 && providedInsertionIndex < 1 + inState.ReflectiveModelList.Count)
            actualInsertionIndex = providedInsertionIndex;

        var outDisplayStateList = inState.ReflectiveModelList.Insert(
            actualInsertionIndex,
            entry);

        _reflectiveState = new ReflectiveState { ReflectiveModelList = outDisplayStateList };
        
        ReflectiveStateChanged?.Invoke();
        return;
    }

    public void ReduceWithAction(
        Key<ReflectiveModel> key,
        Func<ReflectiveModel, ReflectiveModel> withFunc)
    {
    	var inState = GetReflectiveState();
    
        var inDisplayState = inState.ReflectiveModelList.FirstOrDefault(
            x => x.Key == key);

        if (inDisplayState is null)
        {
            ReflectiveStateChanged?.Invoke();
        	return;
        }

        var outDisplayStateList = inState.ReflectiveModelList.Replace(
            inDisplayState,
            withFunc.Invoke(inDisplayState));

        _reflectiveState = new ReflectiveState { ReflectiveModelList = outDisplayStateList };
        
        ReflectiveStateChanged?.Invoke();
        return;
    }

    public void ReduceDisposeAction(Key<ReflectiveModel> key)
    {
    	var inState = GetReflectiveState();
    
        var inDisplayState = inState.ReflectiveModelList.FirstOrDefault(
            x => x.Key == key);

        if (inDisplayState is null)
        {
            ReflectiveStateChanged?.Invoke();
        	return;
        }

        var outDisplayStateList = inState.ReflectiveModelList.Remove(inDisplayState);

        _reflectiveState = new ReflectiveState
        {
            ReflectiveModelList = outDisplayStateList
        };
        
        ReflectiveStateChanged?.Invoke();
        return;
    }
}
