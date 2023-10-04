using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Contexts.Models;

namespace Luthetus.Ide.RazorLib.Commands;

public interface ICommandFactory
{
    public void Initialize();

    public CommandNoType ConstructFocusContextElementCommand(
        ContextRecord contextRecord,
        string displayName,
        string internalIdentifier);
}
