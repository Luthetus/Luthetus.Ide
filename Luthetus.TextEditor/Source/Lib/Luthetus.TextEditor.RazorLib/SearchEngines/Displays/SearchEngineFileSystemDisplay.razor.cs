using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib.SearchEngines.Models;

namespace Luthetus.TextEditor.RazorLib.SearchEngines.Displays;

public partial class SearchEngineFileSystemDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public SearchEngineFileSystem SearchEngineFileSystem { get; set; } = null!;
}