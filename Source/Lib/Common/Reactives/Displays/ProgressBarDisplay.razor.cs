using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Reactives.Models;

namespace Luthetus.Common.RazorLib.Reactives.Displays;

public partial class ProgressBarDisplay : ComponentBase, IDisposable
{	
	[Parameter, EditorRequired]
	public ProgressBarModel ProgressBarModel { get; set; } = null!;

	protected override void OnInitialized()
	{
		if (!ProgressBarModel.IsDisposed)
			ProgressBarModel.ProgressChanged += OnProgressChanged;
	}

	public async void OnProgressChanged(bool isDisposing)
	{
		if (isDisposing)
			ProgressBarModel.ProgressChanged -= OnProgressChanged;

		await InvokeAsync(StateHasChanged);
	}

	public void Dispose()
	{
		ProgressBarModel.ProgressChanged -= OnProgressChanged;
	}
}