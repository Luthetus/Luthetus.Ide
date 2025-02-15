using Luthetus.Common.RazorLib.Menus.Models;

namespace Luthetus.Ide.RazorLib.Shareds.Models;

public class IdeHeaderService : IIdeHeaderService
{
    private readonly object _stateModificationLock = new();

    private IdeHeaderState _ideHeaderState = new();
	
	public event Action? IdeHeaderStateChanged;
	
	public IdeHeaderState GetIdeHeaderState() => _ideHeaderState;

	public void SetMenuFile(MenuRecord menu)
	{
		lock (_stateModificationLock)
		{
			var inState = GetIdeHeaderState();

			_ideHeaderState = inState with
			{
				MenuFile = menu
			};

			goto finalize;
		}

		finalize:
        IdeHeaderStateChanged?.Invoke();
    }
	
	public void SetMenuTools(MenuRecord menu)
	{
		lock (_stateModificationLock)
		{
			var inState = GetIdeHeaderState();

			_ideHeaderState = inState with
			{
				MenuTools = menu
			};

            goto finalize;
        }

        finalize:
        IdeHeaderStateChanged?.Invoke();
    }
	
	public void SetMenuView(MenuRecord menu)
	{
		lock (_stateModificationLock)
		{
			var inState = GetIdeHeaderState();

			_ideHeaderState = inState with
			{
				MenuView = menu
			};

            goto finalize;
        }

        finalize:
        IdeHeaderStateChanged?.Invoke();
    }
	
	public void SetMenuRun(MenuRecord menu)
	{
		lock (_stateModificationLock)
		{
			var inState = GetIdeHeaderState();

			_ideHeaderState = inState with
			{
				MenuRun = menu
			};

            goto finalize;
        }

        finalize:
        IdeHeaderStateChanged?.Invoke();
    }
	
	public void ModifyMenuFile(Func<MenuRecord, MenuRecord> menuFunc)
	{
		lock (_stateModificationLock)
		{
			var inState = GetIdeHeaderState();

			_ideHeaderState = inState with
			{
				MenuFile = menuFunc.Invoke(inState.MenuFile)
			};

            goto finalize;
        }

        finalize:
        IdeHeaderStateChanged?.Invoke();
    }
	
	public void ModifyMenuTools(Func<MenuRecord, MenuRecord> menuFunc)
	{
		lock (_stateModificationLock)
		{
			var inState = GetIdeHeaderState();

			_ideHeaderState = inState with
			{
				MenuTools = menuFunc.Invoke(inState.MenuTools)
			};

            goto finalize;
        }

        finalize:
        IdeHeaderStateChanged?.Invoke();
    }

	public void ModifyMenuView(Func<MenuRecord, MenuRecord> menuFunc)
	{
		lock (_stateModificationLock)
		{
			var inState = GetIdeHeaderState();

			_ideHeaderState = inState with
			{
				MenuView = menuFunc.Invoke(inState.MenuView)
			};

            goto finalize;
        }

        finalize:
        IdeHeaderStateChanged?.Invoke();
    }
	
	public void ModifyMenuRun(Func<MenuRecord, MenuRecord> menuFunc)
	{
		lock (_stateModificationLock)
		{
			var inState = GetIdeHeaderState();

			_ideHeaderState = inState with
			{
				MenuRun = menuFunc.Invoke(inState.MenuRun)
			};

            goto finalize;
        }

        finalize:
        IdeHeaderStateChanged?.Invoke();
    }
}
