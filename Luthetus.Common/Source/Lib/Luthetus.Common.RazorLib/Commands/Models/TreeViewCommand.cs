namespace Luthetus.Common.RazorLib.Commands.Models;

public class TreeViewCommand : CommandWithType<TreeViewCommandArgs>
{
    public TreeViewCommand(
            Func<ICommandArgs, Task> doAsyncFunc,
            string displayName,
            string internalIdentifier,
            bool shouldBubble)
        : base(doAsyncFunc, displayName, internalIdentifier, shouldBubble)
    {
    }
}