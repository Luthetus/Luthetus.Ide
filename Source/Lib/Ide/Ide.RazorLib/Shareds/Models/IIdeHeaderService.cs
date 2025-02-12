using Luthetus.Common.RazorLib.Menus.Models;

namespace Luthetus.Ide.RazorLib.Shareds.Models;

public interface IIdeHeaderService
{
	public event Action? IdeHeaderStateChanged;
	
	public IdeHeaderState GetIdeHeaderState();

	public void ReduceSetMenuFileAction(MenuRecord menu);
	public void ReduceSetMenuToolsAction(MenuRecord menu);
	public void ReduceSetMenuViewAction(MenuRecord menu);
	public void ReduceSetMenuRunAction(MenuRecord menu);
	
	public void ReduceModifyMenuFileAction(Func<MenuRecord, MenuRecord> menuFunc);
	public void ReduceModifyMenuToolsAction(Func<MenuRecord, MenuRecord> menuFunc);
	public void ReduceModifyMenuViewAction(Func<MenuRecord, MenuRecord> menuFunc);
	public void ReduceModifyMenuRunAction(Func<MenuRecord, MenuRecord> menuFunc);
}
