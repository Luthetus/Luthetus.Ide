using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Outlines.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Widgets.Models;
using Luthetus.Common.RazorLib.Reflectives.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Drags.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class LuthetusCommonApi
{
    public LuthetusCommonApi(
        IJSRuntime jsRuntime,
        LuthetusHostingInformation hostingInformation,
        CommonComponentRenderers commonComponentRenderers,
        IBackgroundTaskService backgroundTaskService)
    {
        HostingInformationApi = hostingInformation;


        OptionApi = new AppOptionsService(this);

        ContextApi = new ContextService();
        OutlineApi = new OutlineService(jsRuntime);
        AppDimensionApi = new AppDimensionService();
        PanelApi = new PanelService(AppDimensionApi);
        KeymapApi = new KeymapService();
        WidgetApi = new WidgetService(jsRuntime);
        ReflectiveApi = new ReflectiveService();
        CommonConfigApi = new LuthetusCommonConfig();

        LuthetusCommonJavaScriptInteropApi = jsRuntime.GetLuthetusCommonApi();


        ComponentRendererApi = commonComponentRenderers;
        BackgroundTaskApi = backgroundTaskService;
        BrowserResizeInteropApi = new BrowserResizeInterop(AppDimensionApi);

        DragApi = new DragService();

        ClipboardApi = new JavaScriptInteropClipboardService(jsRuntime);

        DialogApi = new DialogService(jsRuntime);

        NotificationApi = new NotificationService();

        DropdownApi = new DropdownService();

        StorageApi = new LocalStorageService(jsRuntime);

        AppOptionApi = new AppOptionsService(this);

        ThemeApi = new ThemeService();

        TreeViewApi = new TreeViewService(this);
	
		switch (hostingInformation.LuthetusHostingKind)
        {
            case LuthetusHostingKind.Photino:
                EnvironmentProviderApi = new LocalEnvironmentProvider();
                FileSystemProviderApi = new LocalFileSystemProvider(this);
                break;
            default:
                EnvironmentProviderApi = new InMemoryEnvironmentProvider();
                FileSystemProviderApi = new InMemoryFileSystemProvider(this);
                break;
        }
    }

    public IAppOptionsService OptionApi { get; }
    public IContextService ContextApi { get; }
    public IOutlineService OutlineApi { get; }
    public IPanelService PanelApi { get; }
    public IAppDimensionService AppDimensionApi { get; }
    public IKeymapService KeymapApi { get; }
    public IWidgetService WidgetApi { get; }
    public IReflectiveService ReflectiveApi { get; }
    public LuthetusCommonConfig CommonConfigApi { get; }
    public LuthetusCommonJavaScriptInteropApi LuthetusCommonJavaScriptInteropApi { get; }
    public LuthetusHostingInformation HostingInformationApi { get; }
	public ICommonComponentRenderers ComponentRendererApi { get; }
	public IBackgroundTaskService BackgroundTaskApi { get; }
	public BrowserResizeInterop BrowserResizeInteropApi { get; }
	public IDragService DragApi { get; }
    public IClipboardService ClipboardApi { get; }
    public IDialogService DialogApi { get; }
    public INotificationService NotificationApi { get; }
    public IDropdownService DropdownApi { get; }
    public IStorageService StorageApi { get; }
    public IAppOptionsService AppOptionApi { get; }
    public IThemeService ThemeApi { get; }
    public ITreeViewService TreeViewApi { get; }
    public IEnvironmentProvider EnvironmentProviderApi { get; }
    public IFileSystemProvider FileSystemProviderApi { get; }
}
