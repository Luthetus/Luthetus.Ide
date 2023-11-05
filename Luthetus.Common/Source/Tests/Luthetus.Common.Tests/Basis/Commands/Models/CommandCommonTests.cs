using Luthetus.Common.RazorLib.Commands.Models;

namespace Luthetus.Common.Tests.Basis.Commands.Models;

/// <summary>
/// <see cref="CommandCommon"/>
/// </summary>
public class CommandCommonTests
{
    /// <summary>
    /// <see cref="CommandCommon(Func{ICommandParameter, Task}, string, string, bool)"/>
    /// </summary>
    [Fact]
    public async Task Constructor()
    {
        var number = 0;
        
        var displayName = "Increment Number";
        var internalIdentifier = "increment-number";
        var shouldBubble = false;

        var commandCommon = new CommandCommon(
            commandParameter => 
            {
                number++;
                return Task.CompletedTask;
            },
            displayName,
            internalIdentifier,
            shouldBubble);

        await commandCommon.DoAsyncFunc.Invoke(new CommonCommandParameter());

        Assert.Equal(1, number);
        Assert.Equal(displayName, commandCommon.DisplayName);
        Assert.Equal(internalIdentifier, commandCommon.InternalIdentifier);
        Assert.Equal(shouldBubble, commandCommon.ShouldBubble);
    }
}