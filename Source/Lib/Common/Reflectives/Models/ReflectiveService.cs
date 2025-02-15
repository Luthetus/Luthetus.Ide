using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Reflectives.Models;

public class ReflectiveService : IReflectiveService
{
    private readonly object _stateModificationLock = new();

    private ReflectiveState _reflectiveState = new();
	
	public event Action? ReflectiveStateChanged;
	
	public ReflectiveState GetReflectiveState() => _reflectiveState;
	
	public ReflectiveModel GetReflectiveModel(Key<ReflectiveModel> reflectiveModelKey) =>
		_reflectiveState.ReflectiveModelList.FirstOrDefault(x => x.Key == reflectiveModelKey);
    
    public void Register(
        ReflectiveModel entry,
        int providedInsertionIndex)
    {
        lock (_stateModificationLock)
        {
            var inState = GetReflectiveState();

            if (inState.ReflectiveModelList.Any(x => x.Key == entry.Key))
                goto finalize;

            var actualInsertionIndex = 0;

            if (providedInsertionIndex >= 0 && providedInsertionIndex < 1 + inState.ReflectiveModelList.Count)
                actualInsertionIndex = providedInsertionIndex;

            var outDisplayStateList = inState.ReflectiveModelList.Insert(
                actualInsertionIndex,
                entry);

            _reflectiveState = new ReflectiveState { ReflectiveModelList = outDisplayStateList };

            goto finalize;
        }

        finalize:
        ReflectiveStateChanged?.Invoke();
    }

    public void With(
        Key<ReflectiveModel> key,
        Func<ReflectiveModel, ReflectiveModel> withFunc)
    {
        lock (_stateModificationLock)
        {
            var inState = GetReflectiveState();

            var inDisplayState = inState.ReflectiveModelList.FirstOrDefault(
                x => x.Key == key);

            if (inDisplayState is null)
                goto finalize;

            var outDisplayStateList = inState.ReflectiveModelList.Replace(
                inDisplayState,
                withFunc.Invoke(inDisplayState));

            _reflectiveState = new ReflectiveState { ReflectiveModelList = outDisplayStateList };

            goto finalize;
        }

        finalize:
        ReflectiveStateChanged?.Invoke();
    }

    public void Dispose(Key<ReflectiveModel> key)
    {
        lock (_stateModificationLock)
        {
            var inState = GetReflectiveState();

            var inDisplayState = inState.ReflectiveModelList.FirstOrDefault(
                x => x.Key == key);

            if (inDisplayState is null)
                goto finalize;

            var outDisplayStateList = inState.ReflectiveModelList.Remove(inDisplayState);

            _reflectiveState = new ReflectiveState
            {
                ReflectiveModelList = outDisplayStateList
            };

            goto finalize;
        }

        finalize:
        ReflectiveStateChanged?.Invoke();
    }
}
