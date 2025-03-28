using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.TextEditor.RazorLib.Installations.Displays;

/// <remarks>
/// This class is an exception to the naming convention, "don't use the word 'Luthetus' in class names".
/// 
/// Reason for this exception: when one first starts interacting with this project,
/// 	this type might be one of the first types they interact with. So, the redundancy of namespace
/// 	and type containing 'Luthetus' feels reasonable here.
/// </remarks>
public partial class LuthetusTextEditorInitializer : ComponentBase, IDisposable
{
    [Inject]
    public ITextEditorRegistryWrap TextEditorRegistryWrap { get; set; } = null!;
    [Inject]
    public ICompilerServiceRegistry CompilerServiceRegistry { get; set; } = null!;
    [Inject]
    public IDecorationMapperRegistry DecorationMapperRegistry { get; set; } = null!;
    [Inject]
    private TextEditorInitializationBackgroundTaskGroup TextEditorInitializationBackgroundTaskGroup { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    public static Key<ContextSwitchGroup> ContextSwitchGroupKey { get; } = Key<ContextSwitchGroup>.NewKey();
    
    private const string TEST_STRING_FOR_MEASUREMENT = "abcdefghijklmnopqrstuvwxyz0123456789";
    private const int TEST_STRING_REPEAT_COUNT = 6;
    
    private int _countOfTestCharacters;
    private string _measureCharacterWidthAndRowHeightElementId = "luth_te_measure-character-width-and-row-height";
    
    protected override void OnInitialized()
    {
    	_countOfTestCharacters = TEST_STRING_REPEAT_COUNT * TEST_STRING_FOR_MEASUREMENT.Length;
    	
    	TextEditorRegistryWrap.CompilerServiceRegistry = CompilerServiceRegistry;
    	TextEditorRegistryWrap.DecorationMapperRegistry = DecorationMapperRegistry;
    	
    	TextEditorService.OptionsApi.TextEditorOptionsStateChanged += HandleTextEditorOptionsStateChanged;

        TextEditorInitializationBackgroundTaskGroup.Enqueue_LuthetusTextEditorInitializerOnInit();
            
        base.OnInitialized();
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
    	if (firstRender)
    	{
    		QueueRemeasureBackgroundTask();
    	}
    	
    	base.OnAfterRender(firstRender);
    }
    
	private async void HandleTextEditorOptionsStateChanged()
	{
		QueueRemeasureBackgroundTask();
	}
	
    private void QueueRemeasureBackgroundTask()
    {
        TextEditorService.TextEditorWorker.PostRedundant(
            nameof(QueueRemeasureBackgroundTask),
			ResourceUri.Empty,
			Key<TextEditorViewModel>.Empty,
            async editContext =>
            {
            	var charAndLineMeasurements = await TextEditorService.JsRuntimeTextEditorApi
		            .GetCharAndLineMeasurementsInPixelsById(
		                _measureCharacterWidthAndRowHeightElementId,
		                _countOfTestCharacters)
		            .ConfigureAwait(false);
		            
		        TextEditorService.OptionsApi.SetCharAndLineMeasurements(editContext, charAndLineMeasurements);
		        Console.WriteLine(charAndLineMeasurements);
	        });
    }
    
    public void Dispose()
    {
    	TextEditorService.OptionsApi.TextEditorOptionsStateChanged -= HandleTextEditorOptionsStateChanged;
    }
}