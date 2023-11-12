namespace Luthetus.Common.RazorLib.Commands.Models;

public class CommonCommand : CommandWithType<CommonCommandArgs>
{
    public CommonCommand(
            string displayName,
            string internalIdentifier,
            bool shouldBubble,
            Func<ICommandArgs, Task> doAsyncFunc)
        : base(displayName, internalIdentifier, shouldBubble, doAsyncFunc)
    {
    }
}