using Luthetus.Common.RazorLib.Misc;
using Microsoft.JSInterop;

namespace Luthetus.Common.Tests.Basis.Misc;

/// <summary>
/// <see cref="DoNothingJsRuntime"/>
/// </summary>
public class DoNothingJsRuntimeTests
{
    /// <summary>
    /// <see cref="DoNothingJsRuntime.InvokeAsync{TValue}(string, object?[]?)"/>
    /// </summary>
    [Fact]
    public async Task InvokeAsyncA()
    {
        var doNothingJsRuntime = new DoNothingJsRuntime();
        
        var defaultResult = await doNothingJsRuntime.InvokeAsync<string>(string.Empty);

        Assert.Equal(default, defaultResult);
    }

    /// <summary>
    /// <see cref="DoNothingJsRuntime.InvokeAsync{TValue}(string, CancellationToken, object?[]?)"/>
    /// </summary>
    [Fact]
    public async Task InvokeAsyncB()
    {
        var doNothingJsRuntime = new DoNothingJsRuntime();

        var defaultResult = await doNothingJsRuntime.InvokeAsync<string>(string.Empty, CancellationToken.None);

        Assert.Equal(default, defaultResult);
    }
}