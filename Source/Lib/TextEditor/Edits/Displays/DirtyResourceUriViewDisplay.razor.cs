using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.Edits.Displays;

public partial class DirtyResourceUriViewDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IDirtyResourceUriService DirtyResourceUriService { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private TextEditorService TextEditorService { get; set; } = null!;
    
    protected override void OnInitialized()
	{
		DirtyResourceUriService.DirtyResourceUriStateChanged += OnDirtyResourceUriStateChanged;
		base.OnInitialized();
	}

    private Task OpenInEditorOnClick(string filePath)
    {
    	TextEditorService.WorkerArbitrary.PostUnique(nameof(DirtyResourceUriViewDisplay), async editContext =>
    	{
    		await TextEditorService.OpenInEditorAsync(
    			editContext,
                filePath,
				true,
				null,
				new Category("main"),
				Key<TextEditorViewModel>.NewKey());
    	});
    	return Task.CompletedTask;
    }
    
    public async void OnDirtyResourceUriStateChanged()
    {
    	await InvokeAsync(StateHasChanged);
    }
    
    public void Dispose()
    {
    	DirtyResourceUriService.DirtyResourceUriStateChanged -= OnDirtyResourceUriStateChanged;
    }
}