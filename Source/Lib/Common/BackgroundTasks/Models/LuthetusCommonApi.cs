using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Outlines.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Widgets.Models;
using Luthetus.Common.RazorLib.Reflectives.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class LuthetusCommonApi
{
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IStorageService _storageService;

    public LuthetusCommonApi(
        IBackgroundTaskService backgroundTaskService,
        IStorageService storageService)
    {
        _backgroundTaskService = backgroundTaskService;
        _storageService = storageService;

        Storage = new StorageCommonApi(
            _backgroundTaskService,
            _storageService);
        
        Context = new ContextService();
        Outline = new OutlineService();
        Panel = new PanelService();
        AppDimension = new AppDimensionService();
        Keymap = new KeymapService();
        Widget = new WidgetService();
        Reflective = new ReflectiveService();
    }

    public StorageCommonApi Storage { get; }
    public IAppOptionsService AppOptionsService { get; }
    public IContextService Context { get; }
    public IOutlineService Outline { get; }
    public IPanelService Panel { get; }
    public IAppDimensionService AppDimension { get; }
    public IKeymapService Keymap { get; }
    public IWidgetService Widget { get; }
    public IReflectiveService Reflective { get; }
    public LuthetusCommonConfig CommonConfig { get; }
    public IJSRuntime JsRuntime { get; }
    public LuthetusHostingInformation HostingInformation { get; }
	public ICommonComponentRenderers ComponentRenderers { get; }
	public IBackgroundTaskService BackgroundTaskService { get; }
	public BrowserResizeInterop BrowserResizeInterop { get; set; }
}
