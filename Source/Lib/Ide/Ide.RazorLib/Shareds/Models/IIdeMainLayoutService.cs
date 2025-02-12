using System.Collections.Immutable;
using Luthetus.Ide.RazorLib.Shareds.Models;

namespace Luthetus.Ide.RazorLib.Shareds.Models;

public interface IIdeMainLayoutService
{
	public event Action? IdeMainLayoutStateChanged;
	
	public IdeMainLayoutState GetIdeMainLayoutState();

	public void ReduceRegisterFooterJustifyEndComponentAction(FooterJustifyEndComponent footerJustifyEndComponent);
}
