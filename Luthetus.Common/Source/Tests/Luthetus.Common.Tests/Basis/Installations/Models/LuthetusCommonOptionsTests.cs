using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Drags.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Misc;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.Storages.States;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Luthetus.Common.Tests.Basis.Installations.Models;

/// <summary>
/// <see cref="LuthetusCommonOptions"/>
/// </summary>
public record LuthetusCommonOptionsTests
{
    /// <summary>
    /// <see cref="LuthetusCommonOptions.InitialThemeKey"/>
    /// </summary>
    [Fact]
    public void InitialThemeKey()
    {
        // Use default value
        {
            var initialThemeKeyDefault = ThemeFacts.VisualStudioDarkThemeClone.Key;
            var luthetusCommonOptions = new LuthetusCommonOptions();

            Assert.Equal(initialThemeKeyDefault, luthetusCommonOptions.InitialThemeKey);
        }
        
        // Init value
        {
            var initialThemeKey = ThemeFacts.VisualStudioLightThemeClone.Key;
            
            var luthetusCommonOptions = new LuthetusCommonOptions
            {
                InitialThemeKey = initialThemeKey
            };

            Assert.Equal(initialThemeKey, luthetusCommonOptions.InitialThemeKey);
        }
        
        // With value
        {
            var initialThemeKeyDefault = ThemeFacts.VisualStudioDarkThemeClone.Key;
            var luthetusCommonOptions = new LuthetusCommonOptions();

            Assert.Equal(initialThemeKeyDefault, luthetusCommonOptions.InitialThemeKey);
            
            var initialThemeKey = ThemeFacts.VisualStudioLightThemeClone.Key;

            luthetusCommonOptions = luthetusCommonOptions with
            {
                InitialThemeKey = initialThemeKey
            };

            Assert.Equal(initialThemeKey, luthetusCommonOptions.InitialThemeKey);
        }
    }

    /// <summary>
    /// <see cref="LuthetusCommonOptions.CommonFactories"/>
    /// </summary>
    [Fact]
    public void CommonFactories()
    {
        // Assert that 'LocalStorageService' is used for the default CommonFactories
        {
            var hostingInformation = new LuthetusHostingInformation(
            LuthetusHostingKind.UnitTesting,
            new BackgroundTaskServiceSynchronous());

            var services = new ServiceCollection()
                .AddLuthetusCommonServices(hostingInformation)
                .AddScoped<IJSRuntime>(_ => new DoNothingJsRuntime())
                .AddFluxor(options => options.ScanAssemblies(typeof(LuthetusCommonOptions).Assembly))
                .AddScoped<StorageSync>()
                .AddScoped<IBackgroundTaskService>(sp => new BackgroundTaskServiceSynchronous());

            var serviceProvider = services.BuildServiceProvider();

            var store = serviceProvider.GetRequiredService<IStore>();
            store.InitializeAsync().Wait();

            Assert.IsType<LocalStorageService>(serviceProvider.GetRequiredService<IStorageService>());
        }

        // Then assert that modifying the CommonFactories properties to use
        // 'DoNothingStorageService' in place of 'LocalStorageService' works.
        {
            var hostingInformation = new LuthetusHostingInformation(
                LuthetusHostingKind.UnitTesting,
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
                .AddFluxor(options => options.ScanAssemblies(typeof(LuthetusCommonOptions).Assembly))
                .AddScoped<StorageSync>()
                .AddScoped<IBackgroundTaskService>(sp => new BackgroundTaskServiceSynchronous());

            var serviceProvider = services.BuildServiceProvider();

            var store = serviceProvider.GetRequiredService<IStore>();
            store.InitializeAsync().Wait();

            Assert.IsType<DoNothingStorageService>(serviceProvider.GetRequiredService<IStorageService>());
        }
    }

    /// <summary>
    /// <see cref="LuthetusCommonOptions.DialogServiceOptions"/>
    /// </summary>
    [Fact]
    public void DialogServiceOptions()
    {
        // Use default value
        {
            var dialogServiceOptionsDefault = new DialogServiceOptions();
            var luthetusCommonOptions = new LuthetusCommonOptions();

            Assert.Equal(dialogServiceOptionsDefault, luthetusCommonOptions.DialogServiceOptions);
        }

        // Init value
        {
            var isMaximizedStyleCssString = "abc123";

            var dialogServiceOptions = new DialogServiceOptions
            {
                IsMaximizedStyleCssString = isMaximizedStyleCssString
            };

            var luthetusCommonOptions = new LuthetusCommonOptions
            {
                DialogServiceOptions = dialogServiceOptions
            };

            Assert.Equal(dialogServiceOptions, luthetusCommonOptions.DialogServiceOptions);
        }

        // With value
        {
            var dialogServiceOptions = new DialogServiceOptions();
            var luthetusCommonOptions = new LuthetusCommonOptions();
            Assert.Equal(dialogServiceOptions, luthetusCommonOptions.DialogServiceOptions);

            var isMaximizedStyleCssString = "abc123";
            
            dialogServiceOptions = dialogServiceOptions with
            {
                IsMaximizedStyleCssString = isMaximizedStyleCssString
            };

            luthetusCommonOptions = luthetusCommonOptions with
            {
                DialogServiceOptions = dialogServiceOptions
            };

            Assert.Equal(dialogServiceOptions, luthetusCommonOptions.DialogServiceOptions);
        }
    }
}
