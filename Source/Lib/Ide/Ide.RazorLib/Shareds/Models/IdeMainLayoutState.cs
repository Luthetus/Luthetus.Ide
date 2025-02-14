using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Shareds.Models;

public record struct IdeMainLayoutState(ImmutableList<FooterJustifyEndComponent> FooterJustifyEndComponentList)
{
	public IdeMainLayoutState() : this(ImmutableList<FooterJustifyEndComponent>.Empty)
	{
	}
}
