namespace Luthetus.Ide.RazorLib.FindAllReferences.Models;

public record struct FindAllReferencesState
{
	public FindAllReferencesState()
	{
	}

	public IReadOnlyList<string> ReferenceList { get; init; } = Array.Empty<string>();
	public string FullyQualifiedName { get; init; }
}
