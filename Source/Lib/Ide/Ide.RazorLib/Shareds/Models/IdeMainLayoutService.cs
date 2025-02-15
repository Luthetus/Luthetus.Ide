namespace Luthetus.Ide.RazorLib.Shareds.Models;

public class IdeMainLayoutService : IIdeMainLayoutService
{
    private readonly object _stateModificationLock = new();

    private IdeMainLayoutState _ideMainLayoutState = new();
	
	public event Action? IdeMainLayoutStateChanged;
	
	public IdeMainLayoutState GetIdeMainLayoutState() => _ideMainLayoutState;

	public void RegisterFooterJustifyEndComponent(FooterJustifyEndComponent footerJustifyEndComponent)
	{
		lock (_stateModificationLock)
		{
			var inState = GetIdeMainLayoutState();

			var existingComponent = inState.FooterJustifyEndComponentList.FirstOrDefault(x =>
				x.Key == footerJustifyEndComponent.Key);

			if (existingComponent is not null)
                goto finalize;

			var outFooterJustifyEndComponentList = new List<FooterJustifyEndComponent>(inState.FooterJustifyEndComponentList);
			outFooterJustifyEndComponentList.Add(footerJustifyEndComponent);

			_ideMainLayoutState = inState with
			{
				FooterJustifyEndComponentList = outFooterJustifyEndComponentList
			};

			goto finalize;
		}

		finalize:
        IdeMainLayoutStateChanged?.Invoke();
    }
}
