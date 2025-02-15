using Luthetus.Common.RazorLib.Menus.Models;

namespace Luthetus.Ide.RazorLib.Shareds.Models;

public interface IIdeHeaderService
{
	public event Action? IdeHeaderStateChanged;
	
	public IdeHeaderState GetIdeHeaderState();

	public void SetMenuFile(MenuRecord menu);
	public void SetMenuTools(MenuRecord menu);
	public void SetMenuView(MenuRecord menu);
	public void SetMenuRun(MenuRecord menu);
	
	public void ModifyMenuFile(Func<MenuRecord, MenuRecord> menuFunc);
	public void ModifyMenuTools(Func<MenuRecord, MenuRecord> menuFunc);
	public void ModifyMenuView(Func<MenuRecord, MenuRecord> menuFunc);
	public void ModifyMenuRun(Func<MenuRecord, MenuRecord> menuFunc);
}
