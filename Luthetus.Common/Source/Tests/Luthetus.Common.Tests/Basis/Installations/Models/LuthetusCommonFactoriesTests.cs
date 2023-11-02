namespace Luthetus.Common.RazorLib.Installations.Models;

public record LuthetusCommonFactoriesTests
{
    [Fact]
    public void DragServiceFactory()
    {
        /*
        public Func<IServiceProvider, IDragService> DragServiceFactory { get; init; } =
            serviceProvider => new DragService(true);
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void ClipboardServiceFactory()
    {
        /*
        public Func<IServiceProvider, IClipboardService> ClipboardServiceFactory { get; init; } =
            serviceProvider => new JavaScriptInteropClipboardService(
                true,
                serviceProvider.GetRequiredService<IJSRuntime>());
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void DialogServiceFactory()
    {
        /*
        public Func<IServiceProvider, IDialogService> DialogServiceFactory { get; init; } =
            serviceProvider => new DialogService(
                true,
                serviceProvider.GetRequiredService<IDispatcher>(),
                serviceProvider.GetRequiredService<IState<DialogState>>());
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void NotificationServiceFactory()
    {
        /*
        public Func<IServiceProvider, INotificationService> NotificationServiceFactory { get; init; } =
            serviceProvider => new NotificationService(
                true,
                serviceProvider.GetRequiredService<IDispatcher>(),
                serviceProvider.GetRequiredService<IState<NotificationState>>());
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void DropdownServiceFactory()
    {
        /*
        public Func<IServiceProvider, IDropdownService> DropdownServiceFactory { get; init; } =
            serviceProvider => new DropdownService(
                true,
                serviceProvider.GetRequiredService<IDispatcher>(),
                serviceProvider.GetRequiredService<IState<DropdownState>>());
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void StorageServiceFactory()
    {
        /*
        public Func<IServiceProvider, IStorageService> StorageServiceFactory { get; init; } =
            serviceProvider => new LocalStorageService(
                true,
                serviceProvider.GetRequiredService<IJSRuntime>());
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void AppOptionsServiceFactory()
    {
        /*
        public Func<IServiceProvider, IAppOptionsService> AppOptionsServiceFactory { get; init; } =
            serviceProvider => new AppOptionsService(
                true,
                serviceProvider.GetRequiredService<IState<AppOptionsState>>(),
                serviceProvider.GetRequiredService<IState<ThemeState>>(),
                serviceProvider.GetRequiredService<IDispatcher>(),
                serviceProvider.GetRequiredService<IStorageService>(),
                serviceProvider.GetRequiredService<StorageSync>());
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void ThemeServiceFactory()
    {
        /*
        public Func<IServiceProvider, IThemeService> ThemeServiceFactory { get; init; } =
            serviceProvider => new ThemeService(
                true,
                serviceProvider.GetRequiredService<IState<ThemeState>>(),
                serviceProvider.GetRequiredService<IDispatcher>());
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void TreeViewServiceFactory()
    {
        /*
        public Func<IServiceProvider, ITreeViewService> TreeViewServiceFactory { get; init; } =
            serviceProvider => new TreeViewService(
                true,
                serviceProvider.GetRequiredService<IState<TreeViewState>>(),
                serviceProvider.GetRequiredService<IDispatcher>());
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void EnvironmentProviderFactory()
    {
        /*
        public Func<IServiceProvider, IEnvironmentProvider>? EnvironmentProviderFactory { get; init; }
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void FileSystemProviderFactory()
    {
        /*
        public Func<IServiceProvider, IFileSystemProvider>? FileSystemProviderFactory { get; init; }
         */

        throw new NotImplementedException();
    }
}