namespace Luthetus.Common.RazorLib.Commands.Models;

/// <summary>
/// Is boxing a struct by referencing it via an interface it implements more "expensive" than
/// a class instance?
///
/// Because if so, this type is very bad. It is forcing you into a 'class'.
///
/// Meanwhile 3/4 properties are value types and all 4 are immutable.
/// Structs can hold reference types no issue it is just a pointer right?
///
/// I think the most egregious part of this type is that a command
/// is sometimes a high turnover type.
///
/// It is constructed and then "forgotten about" over and over
/// as the user interacts with the user interface and this results in a command firing.
///
/// Some commands are indeed a static readonly reference that uses the 'ICommandArgs'
/// as a way to uniquely handle individual firings.
///
/// But I think a good bit are new'd commands on the spot?
/// </summary>
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
