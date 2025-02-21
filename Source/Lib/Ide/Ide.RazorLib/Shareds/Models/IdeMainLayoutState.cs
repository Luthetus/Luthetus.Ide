namespace Luthetus.Ide.RazorLib.Shareds.Models;

public record struct IdeMainLayoutState(IReadOnlyList<FooterJustifyEndComponent> FooterJustifyEndComponentList)
{
	public IdeMainLayoutState() : this(Array.Empty<FooterJustifyEndComponent>())
	{
	}
}
