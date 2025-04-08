namespace Luthetus.Ide.RazorLib.FindAllReferences.Models;

public class FindAllReferencesService : IFindAllReferencesService
{
	private readonly object _stateModificationLock = new();

	private FindAllReferencesState _findAllReferencesState = new(Array.Empty<string>());

	public event Action? FindAllReferencesStateChanged;
	
	public FindAllReferencesState GetFindAllReferencesState() => _findAllReferencesState;
	
	public void SetReferenceList(IReadOnlyList<string> referenceList)
	{
		lock (_stateModificationLock)
        {
    	    var inState = GetFindAllReferencesState();
            _findAllReferencesState = new FindAllReferencesState(referenceList);
        }

        FindAllReferencesStateChanged?.Invoke();
	}
}
