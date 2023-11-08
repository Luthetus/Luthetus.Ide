using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.Themes.States;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Immutable;

namespace Luthetus.Common.Tests.Basis.Themes.States;

/// <summary>
/// <see cref="ThemeState"/>
/// </summary>
public class ThemeStateReducerTests
{
    [Fact]
    public void ReduceRegisterAction()
    {
        /*
        [ReducerMethod]
        public ThemeState ReduceRegisterAction(
            ThemeState inState, RegisterAction registerAction)
         */

        InitializeThemeStateReducerTests(
            out var _, out var themeStateWrap, out var dispatcher, out var themeRecord);

        Assert.DoesNotContain(themeStateWrap.Value.ThemeBag, x => x == themeRecord);

        dispatcher.Dispatch(new ThemeState.RegisterAction(themeRecord));

        Assert.Contains(themeStateWrap.Value.ThemeBag, x => x == themeRecord);
    }

    [Fact]
    public void ReduceDisposeAction()
    {
        /*
        [ReducerMethod]
        public ThemeState ReduceDisposeAction(
            ThemeState inState, DisposeAction disposeAction)
         */

        InitializeThemeStateReducerTests(
            out var _, out var themeStateWrap, out var dispatcher, out var themeRecord);

        Assert.DoesNotContain(themeStateWrap.Value.ThemeBag, x => x == themeRecord);
        
        dispatcher.Dispatch(new ThemeState.RegisterAction(themeRecord));
        Assert.Contains(themeStateWrap.Value.ThemeBag, x => x == themeRecord);

        dispatcher.Dispatch(new ThemeState.DisposeAction(themeRecord.Key));
        Assert.DoesNotContain(themeStateWrap.Value.ThemeBag, x => x == themeRecord);
    }

    private void InitializeThemeStateReducerTests(
        out ServiceProvider serviceProvider,
        out IState<ThemeState> themeStateWrap,
        out IDispatcher dispatcher,
        out ThemeRecord sampleThemeRecord)
    {
        var services = new ServiceCollection()
            .AddScoped<IThemeService, ThemeService>()
            .AddFluxor(options => options.ScanAssemblies(typeof(IThemeService).Assembly));

        serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        themeStateWrap = serviceProvider.GetRequiredService<IState<ThemeState>>();

        dispatcher = serviceProvider.GetRequiredService<IDispatcher>();

        sampleThemeRecord = new ThemeRecord(
            Key<ThemeRecord>.NewKey(),
            "Test Dark Theme",
            "test_dark-theme",
            ThemeContrastKind.Default,
            ThemeColorKind.Dark,
            new ThemeScope[] { ThemeScope.App, ThemeScope.TextEditor }.ToImmutableList());
    }
}