using Luthetus.Common.RazorLib.Clipboards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Common.Tests.Core.Clipboards;

public class TEST_CLIPBOARDS
{
    [Fact]
    public async Task InMemoryClipboardService_Test()
    {
        const string Value = "Hello World!";
        var inMemoryClipboardService = new InMemoryClipboardService(true);

        await inMemoryClipboardService.SetClipboardAsync(Value);
        var readResult = await inMemoryClipboardService.ReadClipboardAsync();

        Assert.Equal(Value, readResult);
    }
}
