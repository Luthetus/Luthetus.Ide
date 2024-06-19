using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.Misc;
using Luthetus.Common.RazorLib.Storages.Models;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Luthetus.Common.Tests.Basis.Installations.Models;

/// <summary>
/// <see cref="LuthetusCommonConfig"/>
/// </summary>
public record LuthetusCommonConfigTests
{
    /// <summary>
    /// <see cref="LuthetusCommonConfig.InitialThemeKey"/>
    /// </summary>
    [Fact]
    public void InitialThemeKey()
    {
        // Use default value
        {
            var initialThemeKeyDefault = ThemeFacts.VisualStudioDarkThemeClone.Key;
            var commonConfig = new LuthetusCommonConfig();

            Assert.Equal(initialThemeKeyDefault, commonConfig.InitialThemeKey);
        }
        
        // Init value
        {
            var initialThemeKey = ThemeFacts.VisualStudioLightThemeClone.Key;
            
            var commonConfig = new LuthetusCommonConfig
            {
                InitialThemeKey = initialThemeKey
            };

            Assert.Equal(initialThemeKey, commonConfig.InitialThemeKey);
        }
        
        // With value
        {
            var initialThemeKeyDefault = ThemeFacts.VisualStudioDarkThemeClone.Key;
            var commonConfig = new LuthetusCommonConfig();

            Assert.Equal(initialThemeKeyDefault, commonConfig.InitialThemeKey);
            
            var initialThemeKey = ThemeFacts.VisualStudioLightThemeClone.Key;

            commonConfig = commonConfig with
            {
                InitialThemeKey = initialThemeKey
            };

            Assert.Equal(initialThemeKey, commonConfig.InitialThemeKey);
        }
    }

    /// <summary>
    /// <see cref="LuthetusCommonConfig.CommonFactories"/>
    /// </summary>
    [Fact]
    public void CommonFactories()
    {
        // Assert that 'LocalStorageService' is used for the default CommonFactories
        {
            var hostingInformation = new LuthetusHostingInformation(
                LuthetusHostingKind.UnitTestingSynchronous,
                new BackgroundTaskServiceSynchronous());

            var services = new ServiceCollection()
                .AddLuthetusCommonServices(hostingInformation)
                .AddScoped<IJSRuntime>(_ => new DoNothingJsRuntime())
                .AddFluxor(options => options.ScanAssemblies(typeof(LuthetusCommonConfig).Assembly));

            var serviceProvider = services.BuildServiceProvider();

            var store = serviceProvider.GetRequiredService<IStore>();
            store.InitializeAsync().Wait();

            Assert.IsType<LocalStorageService>(serviceProvider.GetRequiredService<IStorageService>());
        }

        // Then assert that modifying the CommonFactories properties to use
        // 'DoNothingStorageService' in place of 'LocalStorageService' works.
        {
            var hostingInformation = new LuthetusHostingInformation(
                LuthetusHostingKind.UnitTestingSynchronous,
                new BackgroundTaskServiceSynchronous());

            var services = new ServiceCollection()
                .AddLuthetusCommonServices(hostingInformation, options =>
                {
                    var outCommonFactories = options.CommonFactories with
                    {
                        StorageServiceFactory = sp => new DoNothingStorageService()
                    };

                    return options with
                    {
                        CommonFactories = outCommonFactories
                    };
                })
                .AddScoped<IJSRuntime>(_ => new DoNothingJsRuntime())
                .AddFluxor(options => options.ScanAssemblies(typeof(LuthetusCommonConfig).Assembly));

            var serviceProvider = services.BuildServiceProvider();

            var store = serviceProvider.GetRequiredService<IStore>();
            store.InitializeAsync().Wait();

            Assert.IsType<DoNothingStorageService>(serviceProvider.GetRequiredService<IStorageService>());
        }
    }

    /// <summary>
    /// <see cref="LuthetusCommonConfig.DialogServiceOptions"/>
    /// </summary>
    [Fact]
    public void DialogServiceOptions()
    {
        // Use default value
        {
            var dialogServiceOptionsDefault = new DialogServiceOptions();
            var commonConfig = new LuthetusCommonConfig();

            Assert.Equal(dialogServiceOptionsDefault, commonConfig.DialogServiceOptions);
        }

        // Init value
        {
            var isMaximizedStyleCssString = "abc123";

            var dialogServiceOptions = new DialogServiceOptions
            {
                IsMaximizedStyleCssString = isMaximizedStyleCssString
            };

            var commonConfig = new LuthetusCommonConfig
            {
                DialogServiceOptions = dialogServiceOptions
            };

            Assert.Equal(dialogServiceOptions, commonConfig.DialogServiceOptions);
        }

        // With value
        {
            var dialogServiceOptions = new DialogServiceOptions();
            var commonConfig = new LuthetusCommonConfig();
            Assert.Equal(dialogServiceOptions, commonConfig.DialogServiceOptions);

            var isMaximizedStyleCssString = "abc123";
            
            dialogServiceOptions = dialogServiceOptions with
            {
                IsMaximizedStyleCssString = isMaximizedStyleCssString
            };

            commonConfig = commonConfig with
            {
                DialogServiceOptions = dialogServiceOptions
            };

            Assert.Equal(dialogServiceOptions, commonConfig.DialogServiceOptions);
        }
    }
}
