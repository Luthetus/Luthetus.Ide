namespace Luthetus.Common.RazorLib.Commands.Models;

public abstract class CommandNoType
{
    public CommandNoType(
        string displayName,
        string internalIdentifier,
        bool shouldBubble,
        Func<ICommandArgs, ValueTask> commandFunc)
    {
        DisplayName = displayName;
        InternalIdentifier = internalIdentifier;
        ShouldBubble = shouldBubble;
        CommandFunc = commandFunc;
    }

    public string DisplayName { get; }
    public string InternalIdentifier { get; }
    public bool ShouldBubble { get; }
    public Func<ICommandArgs, ValueTask> CommandFunc { get; }
}
