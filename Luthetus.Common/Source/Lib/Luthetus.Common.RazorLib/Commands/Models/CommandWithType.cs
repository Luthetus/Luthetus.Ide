namespace Luthetus.Common.RazorLib.Commands.Models;

public abstract class CommandWithType<T> : CommandNoType where T : notnull
{
    protected CommandWithType(
            Func<ICommandArgs, Task> doAsyncFunc,
            string displayName,
            string internalIdentifier,
            bool shouldBubble) 
        : base(doAsyncFunc, displayName, internalIdentifier, shouldBubble)
    {
    }
}
