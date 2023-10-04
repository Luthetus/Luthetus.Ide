namespace Luthetus.Common.RazorLib.Commands.Models;

public class CommandCommon : CommandWithType<CommonCommandParameter>
{
    public CommandCommon(
            Func<ICommandParameter, Task> doAsyncFunc,
            string displayName,
            string internalIdentifier,
            bool shouldBubble)
        : base(doAsyncFunc, displayName, internalIdentifier, shouldBubble)
    {
    }
}