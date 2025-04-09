namespace Luthetus.Ide.RazorLib.FindAllReferences.Models;

public interface IFindAllReferencesService
{
	public event Action? FindAllReferencesStateChanged;
	
	public FindAllReferencesState GetFindAllReferencesState();
	public void SetReferenceList(IReadOnlyList<string> referenceList);
	public void SetFullyQualifiedName(string fullyQualifiedName);
}
