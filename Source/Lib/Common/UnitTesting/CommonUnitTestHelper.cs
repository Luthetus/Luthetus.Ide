using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Drags.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.Common.RazorLib.UnitTesting;

public class CommonUnitTestHelper
{
    /// <summary>
    /// To create an instance of <see cref="CommonUnitTestHelper"/>,
    /// one should invoke <see cref="AddLuthetusCommonServicesUnitTesting(IServiceCollection, LuthetusHostingInformation)"/>,
    /// then build the <see cref="IServiceProvider"/>, and provide the built serviceProvider to this constructor.
    /// </summary>
    public CommonUnitTestHelper(IServiceProvider serviceProvider)
    {
        EnvironmentProvider = serviceProvider.GetRequiredService<IEnvironmentProvider>();
        FileSystemProvider = serviceProvider.GetRequiredService<IFileSystemProvider>();
        AppOptionsService = serviceProvider.GetRequiredService<IAppOptionsService>();
        DialogService = serviceProvider.GetRequiredService<IDialogService>();
        StorageService = serviceProvider.GetRequiredService<IStorageService>();
        DragService = serviceProvider.GetRequiredService<IDragService>();
        DropdownService = serviceProvider.GetRequiredService<IDropdownService>();
        ClipboardService = serviceProvider.GetRequiredService<IClipboardService>();
        NotificationService = serviceProvider.GetRequiredService<INotificationService>();
        ThemeService = serviceProvider.GetRequiredService<IThemeService>();
        TreeViewService = serviceProvider.GetRequiredService<ITreeViewService>();

        ValidateDependencies();
    }

    /// <summary>
    /// Implementation is intended to be <see cref="InMemoryEnvironmentProvider"/>
    /// </summary>
    public IEnvironmentProvider EnvironmentProvider { get; }
    /// <summary>
    /// Implementation is intended to be <see cref="InMemoryFileSystemProvider"/>
    /// </summary>
    public IFileSystemProvider FileSystemProvider { get; }
    public IAppOptionsService AppOptionsService { get; }
    public IDialogService DialogService { get; }
    public IStorageService StorageService { get; }
    public IDragService DragService { get; }
    public IDropdownService DropdownService { get; }
    /// <summary>
    /// Implementation is intended to be <see cref="InMemoryClipboardService"/>
    /// </summary>
    public IClipboardService ClipboardService { get; }
    public INotificationService NotificationService { get; }
    public IThemeService ThemeService { get; }
    public ITreeViewService TreeViewService { get; }

    /// <summary>This method is not an extension method due to its niche nature.</summary>
    public static IServiceCollection AddLuthetusCommonServicesUnitTesting(
        IServiceCollection services,
        LuthetusHostingInformation hostingInformation)
    {
        return services.AddLuthetusCommonServices(hostingInformation, commonOptions =>
        {
            var outLuthetusCommonFactories = commonOptions.CommonFactories with
            {
                ClipboardServiceFactory = _ => new InMemoryClipboardService(),
                StorageServiceFactory = _ => new DoNothingStorageService(),
            };

            return commonOptions with
            {
                CommonFactories = outLuthetusCommonFactories
            };
        }); ;
    }

    private void ValidateDependencies()
    {
        if (EnvironmentProvider is not InMemoryEnvironmentProvider)
            ThrowInvalidInterfaceException(typeof(IEnvironmentProvider), typeof(InMemoryEnvironmentProvider));

        if (FileSystemProvider is not InMemoryFileSystemProvider)
            ThrowInvalidInterfaceException(typeof(IFileSystemProvider), typeof(InMemoryFileSystemProvider));

        if (ClipboardService is not InMemoryClipboardService)
            ThrowInvalidInterfaceException(typeof(IClipboardService), typeof(InMemoryClipboardService));

        if (StorageService is not DoNothingStorageService)
            ThrowInvalidInterfaceException(typeof(IStorageService), typeof(DoNothingStorageService));
    }

    private void ThrowInvalidInterfaceException(Type interfaceType, Type concreteType)
    {
        throw new ApplicationException(
            $"The current implementation of {nameof(IStorageService)}" +
            $" is NOT {nameof(DoNothingStorageService)}." +
            $" To avoid side effects in unit tests," +
            $" change the implementation of {nameof(IStorageService)}" +
            $" to {nameof(DoNothingStorageService)}");
    }
}
