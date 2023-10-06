using Luthetus.Common.RazorLib.Commands.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Common.Tests.Core.Commands;

public class TEST_COMMANDS
{
    [Fact]
    public void CommandCommon_Test()
    {
        var command = new CommandCommon(
            commandParameter => Task.CompletedTask,
            nameof(CommandCommon_Test),
            nameof(CommandCommon_Test),
            false);

        command.DoAsyncFunc.Invoke(new CommonCommandParameter());
    }
}
