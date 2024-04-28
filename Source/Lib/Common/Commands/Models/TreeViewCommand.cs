namespace Luthetus.Common.RazorLib.Commands.Models;

public class TreeViewCommand : CommandWithType<TreeViewCommandArgs>
{
    public TreeViewCommand(
            string displayName,
            string internalIdentifier,
            bool shouldBubble,
            Func<ICommandArgs, Task> commandFunc)
        : base(displayName, internalIdentifier, shouldBubble, commandFunc)
    {
    }
}