namespace Luthetus.Common.RazorLib.Commands.Models;

public class CommandCommonTests
{
    [Fact]
    public async Task Constructor()
    {
        /*
        public CommandCommon(
            Func<ICommandParameter, Task> doAsyncFunc, string displayName, string internalIdentifier, bool shouldBubble)
         */

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