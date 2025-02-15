namespace Luthetus.Ide.RazorLib.Shareds.Models;

public interface IIdeMainLayoutService
{
	public event Action? IdeMainLayoutStateChanged;
	
	public IdeMainLayoutState GetIdeMainLayoutState();

	public void RegisterFooterJustifyEndComponent(FooterJustifyEndComponent footerJustifyEndComponent);
}
