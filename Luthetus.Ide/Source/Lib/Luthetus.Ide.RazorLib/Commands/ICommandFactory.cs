using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;

namespace Luthetus.Ide.RazorLib.Commands;

public interface ICommandFactory
{
	public DialogRecord? CodeSearchDialog { get; set; }

    public void Initialize();

    public CommandNoType ConstructFocusContextElementCommand(
        ContextRecord contextRecord,
        string displayName,
        string internalIdentifier);
}
