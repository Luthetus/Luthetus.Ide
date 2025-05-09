using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;

namespace Luthetus.Ide.RazorLib.Shareds.Models;

public record struct IdeHeaderState(
	MenuRecord MenuFile,
	MenuRecord MenuTools,
	MenuRecord MenuView,
	MenuRecord MenuRun)
{
	public IdeHeaderState() : this(
		new MenuRecord(Array.Empty<MenuOptionRecord>()),
		new MenuRecord(Array.Empty<MenuOptionRecord>()),
		new MenuRecord(Array.Empty<MenuOptionRecord>()),
		new MenuRecord(Array.Empty<MenuOptionRecord>()))
	{
	}

	public static readonly Key<DropdownRecord> DropdownKeyFile = Key<DropdownRecord>.NewKey();
    public const string ButtonFileId = "luth_ide_header-button-file";

	public static readonly Key<DropdownRecord> DropdownKeyTools = Key<DropdownRecord>.NewKey();
    public const string ButtonToolsId = "luth_ide_header-button-tools";

	public static readonly Key<DropdownRecord> DropdownKeyView = Key<DropdownRecord>.NewKey();
    public const string ButtonViewId = "luth_ide_header-button-view";

	public static readonly Key<DropdownRecord> DropdownKeyRun = Key<DropdownRecord>.NewKey();
    public const string ButtonRunId = "luth_ide_header-button-run";
}
