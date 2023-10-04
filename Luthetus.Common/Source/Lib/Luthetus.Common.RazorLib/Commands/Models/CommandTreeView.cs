namespace Luthetus.Common.RazorLib.Commands.Models;

public class CommandTreeView : CommandWithType<TreeViewCommandParameter>
{
    protected CommandTreeView(
            Func<ICommandParameter, Task> doAsyncFunc,
            string displayName,
            string internalIdentifier,
            bool shouldBubble)
        : base(doAsyncFunc, displayName, internalIdentifier, shouldBubble)
    {
    }
}