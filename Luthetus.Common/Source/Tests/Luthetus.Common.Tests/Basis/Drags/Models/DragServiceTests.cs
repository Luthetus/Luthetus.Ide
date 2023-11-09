using Fluxor;
using Luthetus.Common.RazorLib.Drags.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.Common.Tests.Basis.Drags.Models;

/// <summary>
/// <see cref="DragService"/>
/// </summary>
public class DragServiceTests
{
    /// <summary>
    /// <see cref="DragService(IDispatcher, IState{RazorLib.Drags.Displays.DragState})"/>
    /// <br/>----<br/>
    /// <see cref="DragService.DragStateWrap"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        InitializeDragServiceTests(out var dragService, out _);

        Assert.NotNull(dragService);
        Assert.NotNull(dragService.DragStateWrap);
    }

    /// <summary>
    /// <see cref="DragService.WithAction(Func{RazorLib.Drags.Displays.DragState, RazorLib.Drags.Displays.DragState})"/>
    /// </summary>
    [Fact]
    public void WithAction()
    {
        InitializeDragServiceTests(out var dragService, out _);

        Assert.False(dragService.DragStateWrap.Value.ShouldDisplay);
        Assert.Null(dragService.DragStateWrap.Value.MouseEventArgs);

        dragService.WithAction(x => x with
        {
            ShouldDisplay = true,
            MouseEventArgs = new(),
        });

        Assert.True(dragService.DragStateWrap.Value.ShouldDisplay);
        Assert.NotNull(dragService.DragStateWrap.Value.MouseEventArgs);
    }

    private void InitializeDragServiceTests(
        out IDragService dragService,
        out ServiceProvider serviceProvider)
    {
        var services = new ServiceCollection()
            .AddScoped<IDragService, DragService>()
            .AddFluxor(options => options.ScanAssemblies(typeof(IDragService).Assembly));

        serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        dragService = serviceProvider.GetRequiredService<IDragService>();
    }
}