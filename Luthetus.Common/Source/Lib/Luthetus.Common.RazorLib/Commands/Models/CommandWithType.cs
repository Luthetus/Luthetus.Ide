namespace Luthetus.Common.RazorLib.Commands.Models;

public abstract class CommandWithType<T> : CommandNoType where T : notnull
{
    protected CommandWithType(
            string displayName,
            string internalIdentifier,
            bool shouldBubble,
            Func<ICommandArgs, Task> doAsyncFunc) 
        : base(displayName, internalIdentifier, shouldBubble, doAsyncFunc)
    {
    }
}
