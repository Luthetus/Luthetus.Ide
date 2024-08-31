namespace Luthetus.Common.RazorLib.Commands.Models;

public class CommonCommand : CommandWithType<CommonCommandArgs>
{
    public static CommonCommand Empty { get; } = new CommonCommand(
        "Do Nothing",
        "do-nothing",
        false,
        _ => Task.CompletedTask);

    public CommonCommand(
            string displayName,
            string internalIdentifier,
            bool shouldBubble,
            Func<ICommandArgs, Task> commandFunc)
        : base(displayName, internalIdentifier, shouldBubble, commandFunc)
    {
    }
}