namespace Luthetus.Ide.RazorLib.Shareds.Models;

public class IdeMainLayoutService : IIdeMainLayoutService
{
	private IdeMainLayoutState _ideMainLayoutState = new();
	
	public event Action? IdeMainLayoutStateChanged;
	
	public IdeMainLayoutState GetIdeMainLayoutState() => _ideMainLayoutState;

	public void ReduceRegisterFooterJustifyEndComponentAction(FooterJustifyEndComponent footerJustifyEndComponent)
	{
		var inState = GetIdeMainLayoutState();
	
		var existingComponent = inState.FooterJustifyEndComponentList.FirstOrDefault(x =>
			x.Key == footerJustifyEndComponent.Key);
			
		if (existingComponent is not null)
		{
			IdeMainLayoutStateChanged?.Invoke();
			return;
		}
	
		_ideMainLayoutState = inState with
		{
			FooterJustifyEndComponentList = inState.FooterJustifyEndComponentList.Add(
				footerJustifyEndComponent)
		};
		
		IdeMainLayoutStateChanged?.Invoke();
		return;
	}
}
