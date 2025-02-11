using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Reflectives.Models;

public interface IReflectiveService
{
	public event Action? ReflectiveStateChanged;
	
	public ReflectiveState GetReflectiveState();
	
	public ReflectiveModel GetReflectiveModel(Key<ReflectiveModel> reflectiveModelKey);
    
    public void ReduceRegisterAction(
        ReflectiveModel entry,
        int providedInsertionIndex);

    public void ReduceWithAction(
        Key<ReflectiveModel> key,
        Func<ReflectiveModel, ReflectiveModel> withFunc);

    public void ReduceDisposeAction(Key<ReflectiveModel> key);
}
