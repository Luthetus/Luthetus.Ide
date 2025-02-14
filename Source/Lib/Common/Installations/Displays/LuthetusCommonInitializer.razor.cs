using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.Common.RazorLib.Installations.Displays;

/// <remarks>
/// This class is an exception to the naming convention, "don't use the word 'Luthetus' in class names".
/// 
/// Reason for this exception: when one first starts interacting with this project,
/// 	this type might be one of the first types they interact with. So, the redundancy of namespace
/// 	and type containing 'Luthetus' feels reasonable here.
/// </remarks>
public partial class LuthetusCommonInitializer : ComponentBase, IDisposable
{
    [Inject]
    private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
	[Inject]
    private CommonBackgroundTaskApi CommonBackgroundTaskApi { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject]
    private BrowserResizeInterop BrowserResizeInterop { get; set; } = null!;
    [Inject]
    private LuthetusHostingInformation LuthetusHostingInformation { get; set; } = null!;
    
    public static Key<ContextSwitchGroup> ContextSwitchGroupKey { get; } = Key<ContextSwitchGroup>.NewKey();
    
    private LuthetusCommonJavaScriptInteropApi? _jsRuntimeCommonApi;
    
    private LuthetusCommonJavaScriptInteropApi JsRuntimeCommonApi =>
    	_jsRuntimeCommonApi ??= JsRuntime.GetLuthetusCommonApi();
    
    /// <summary>
    /// This is to say that the order of the <Luthetus...Initializer/> components
    /// in the markup matters?
    /// </summary>
    private CancellationTokenSource _cancellationTokenSource = new();
    
    private bool _hasStartedContinuousWorker = false;
    private bool _hasStartedIndefiniteWorker = false;

	protected override void OnInitialized()
	{
        CommonBackgroundTaskApi.Enqueue_LuthetusCommonInitializer();

        base.OnInitialized();
	}

	protected override void OnAfterRender(bool firstRender)
	{
		if (firstRender)
		{
			var token = _cancellationTokenSource.Token;

			if (BackgroundTaskService.ContinuousTaskWorker.StartAsyncTask is null)
			{
				_hasStartedContinuousWorker = true;

				BackgroundTaskService.ContinuousTaskWorker.StartAsyncTask = Task.Run(
					() => BackgroundTaskService.ContinuousTaskWorker.StartAsync(token),
					token);
			}

			if (LuthetusHostingInformation.LuthetusPurposeKind == LuthetusPurposeKind.Ide)
			{
				if (BackgroundTaskService.IndefiniteTaskWorker.StartAsyncTask is null)
				{
					_hasStartedIndefiniteWorker = true;

					BackgroundTaskService.IndefiniteTaskWorker.StartAsyncTask = Task.Run(
						() => BackgroundTaskService.IndefiniteTaskWorker.StartAsync(token),
						token);
				}
			}

			BrowserResizeInterop.SubscribeWindowSizeChanged(JsRuntimeCommonApi);
		}

		base.OnAfterRender(firstRender);
	}
    
    public void Dispose()
    {
    	BrowserResizeInterop.DisposeWindowSizeChanged(JsRuntimeCommonApi);
    	_cancellationTokenSource.Cancel();
    	_cancellationTokenSource.Dispose();
    	
    	if (_hasStartedContinuousWorker)
    		BackgroundTaskService.ContinuousTaskWorker.StartAsyncTask = null;
    		
    	if (_hasStartedIndefiniteWorker)
    		BackgroundTaskService.IndefiniteTaskWorker.StartAsyncTask = null;
    }
}