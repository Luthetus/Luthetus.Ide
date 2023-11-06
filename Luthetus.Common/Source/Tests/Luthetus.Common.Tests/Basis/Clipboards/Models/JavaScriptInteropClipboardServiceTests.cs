using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.Misc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;

namespace Luthetus.Common.Tests.Basis.Clipboards.Models;

/// <summary>
/// <see cref="JavaScriptInteropClipboardService"/>
/// </summary>
public class JavaScriptInteropClipboardServiceTests
{
    /// <summary>
    /// <see cref="JavaScriptInteropClipboardService(IJSRuntime)"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var services = new ServiceCollection()
            .AddSingleton<ILoggerFactory, NullLoggerFactory>()
            .AddSingleton<IJSRuntime, DoNothingJsRuntime>();

        var sp = services.BuildServiceProvider();

        var javaScriptInteropClipboardService = new JavaScriptInteropClipboardService(
            sp.GetRequiredService<IJSRuntime>());
    }

    /// <summary>
    /// <see cref="JavaScriptInteropClipboardService.ReadClipboard()"/>
    /// </summary>
    [Fact]
    public void ReadClipboard()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="JavaScriptInteropClipboardService.SetClipboard(string)"/>
    /// </summary>
    [Fact]
    public void SetClipboard()
    {
        throw new NotImplementedException();
    }
}