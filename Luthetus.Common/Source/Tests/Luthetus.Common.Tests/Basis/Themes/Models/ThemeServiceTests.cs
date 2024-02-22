using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.Themes.States;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Immutable;

namespace Luthetus.Common.Tests.Basis.Themes.Models;

/// <summary>
/// <see cref="ThemeService"/>
/// </summary>
public class ThemeServiceTests
{
    /// <summary>
    /// <see cref="ThemeService(IState{ThemeState}, IDispatcher)"/>
    /// <br/>----<br/>
    /// <see cref="ThemeService.ThemeStateWrap"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        InitializeThemeServiceTests(out var themeService, out var themeRecord, out _);

        Assert.NotNull(themeService);
        Assert.NotNull(themeService.ThemeStateWrap);
    }

    /// <summary>
    /// <see cref="ThemeService.RegisterThemeRecord(ThemeRecord)"/>
    /// </summary>
    [Fact]
    public void RegisterThemeRecord()
    {
        InitializeThemeServiceTests(out var themeService, out var themeRecord, out _);

        Assert.DoesNotContain(themeService.ThemeStateWrap.Value.ThemeList, x => x == themeRecord);

        themeService.RegisterThemeRecord(themeRecord);
        Assert.NotEmpty(themeService.ThemeStateWrap.Value.ThemeList);
        Assert.Contains(themeService.ThemeStateWrap.Value.ThemeList, x => x == themeRecord);
    }

    /// <summary>
    /// <see cref="ThemeService.DisposeThemeRecord(Key{ThemeRecord})"/>
    /// </summary>
    [Fact]
    public void DisposeThemeRecord()
    {
        InitializeThemeServiceTests(out var themeService, out var themeRecord, out _);

        Assert.DoesNotContain(themeService.ThemeStateWrap.Value.ThemeList, x => x == themeRecord);

        themeService.RegisterThemeRecord(themeRecord);
        Assert.Contains(themeService.ThemeStateWrap.Value.ThemeList, x => x == themeRecord);

        themeService.DisposeThemeRecord(themeRecord.Key);
        Assert.DoesNotContain(themeService.ThemeStateWrap.Value.ThemeList, x => x == themeRecord);
    }

    private void InitializeThemeServiceTests(
        out IThemeService themeService,
        out ThemeRecord sampleThemeRecord,
        out ServiceProvider serviceProvider)
    {
        var services = new ServiceCollection()
            .AddScoped<IThemeService, ThemeService>()
            .AddFluxor(options => options.ScanAssemblies(typeof(IThemeService).Assembly));

        serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        themeService = serviceProvider.GetRequiredService<IThemeService>();

        sampleThemeRecord = new ThemeRecord(
            Key<ThemeRecord>.NewKey(),
            "Test Dark Theme",
            "test_dark-theme",
            ThemeContrastKind.Default,
            ThemeColorKind.Dark,
            new ThemeScope[] { ThemeScope.App, ThemeScope.TextEditor }.ToImmutableList());
    }
}