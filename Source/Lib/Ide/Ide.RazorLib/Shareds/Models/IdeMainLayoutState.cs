using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Shareds.Models;

public record struct IdeMainLayoutState(List<FooterJustifyEndComponent> FooterJustifyEndComponentList)
{
	public IdeMainLayoutState() : this(new List<FooterJustifyEndComponent>())
	{
	}
}
