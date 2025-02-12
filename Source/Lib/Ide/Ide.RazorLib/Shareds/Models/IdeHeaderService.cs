using Luthetus.Common.RazorLib.Menus.Models;

namespace Luthetus.Ide.RazorLib.Shareds.Models;

public class IdeHeaderService : IIdeHeaderService
{
	private IdeHeaderState _ideHeaderState = new();
	
	public event Action? IdeHeaderStateChanged;
	
	public IdeHeaderState GetIdeHeaderState() => _ideHeaderState;

	public void ReduceSetMenuFileAction(MenuRecord menu)
	{
		var inState = GetIdeHeaderState();
	
		_ideHeaderState = inState with
		{
			MenuFile = menu
		};
		
		IdeHeaderStateChanged?.Invoke();
		return;
	}
	
	public void ReduceSetMenuToolsAction(MenuRecord menu)
	{
		var inState = GetIdeHeaderState();
	
		_ideHeaderState = inState with
		{
			MenuTools = menu
		};
		
		IdeHeaderStateChanged?.Invoke();
		return;
	}
	
	public void ReduceSetMenuViewAction(MenuRecord menu)
	{
		var inState = GetIdeHeaderState();
	
		_ideHeaderState = inState with
		{
			MenuView = menu
		};
		
		IdeHeaderStateChanged?.Invoke();
		return;
	}
	
	public void ReduceSetMenuRunAction(MenuRecord menu)
	{
		var inState = GetIdeHeaderState();
	
		_ideHeaderState = inState with
		{
			MenuRun = menu
		};
		
		IdeHeaderStateChanged?.Invoke();
		return;
	}
	
	public void ReduceModifyMenuFileAction(Func<MenuRecord, MenuRecord> menuFunc)
	{
		var inState = GetIdeHeaderState();
	
		_ideHeaderState = inState with
		{
			MenuFile = menuFunc.Invoke(inState.MenuFile)
		};
		
		IdeHeaderStateChanged?.Invoke();
		return;
	}
	
	public void ReduceModifyMenuToolsAction(Func<MenuRecord, MenuRecord> menuFunc)
	{
		var inState = GetIdeHeaderState();
	
		_ideHeaderState = inState with
		{
			MenuTools = menuFunc.Invoke(inState.MenuTools)
		};
		
		IdeHeaderStateChanged?.Invoke();
		return;
	}

	public void ReduceModifyMenuViewAction(Func<MenuRecord, MenuRecord> menuFunc)
	{
		var inState = GetIdeHeaderState();
	
		_ideHeaderState = inState with
		{
			MenuView = menuFunc.Invoke(inState.MenuView)
		};
		
		IdeHeaderStateChanged?.Invoke();
		return;
	}
	
	public void ReduceModifyMenuRunAction(Func<MenuRecord, MenuRecord> menuFunc)
	{
		var inState = GetIdeHeaderState();
	
		_ideHeaderState = inState with
		{
			MenuRun = menuFunc.Invoke(inState.MenuRun)
		};
		
		IdeHeaderStateChanged?.Invoke();
		return;
	}
}
