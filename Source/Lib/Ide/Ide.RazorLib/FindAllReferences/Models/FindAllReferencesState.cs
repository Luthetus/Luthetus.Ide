namespace Luthetus.Ide.RazorLib.FindAllReferences.Models;

public struct FindAllReferencesState
{
	public FindAllReferencesState(IReadOnlyList<string> referenceList)
	{
		ReferenceList = referenceList;
	}

	public IReadOnlyList<string> ReferenceList { get; }
}
