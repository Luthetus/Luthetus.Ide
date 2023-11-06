namespace Luthetus.Common.RazorLib.Commands.Models;

public class CommonCommand : CommandWithType<CommonCommandArgs>
{
    public CommonCommand(
            Func<ICommandArgs, Task> doAsyncFunc,
            string displayName,
            string internalIdentifier,
            bool shouldBubble)
        : base(doAsyncFunc, displayName, internalIdentifier, shouldBubble)
    {
    }
}