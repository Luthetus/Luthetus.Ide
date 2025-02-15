using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Website.RazorLib;

public partial class LuthetusWebsiteInitializer : ComponentBase
{
    [Inject]
    private ITextEditorRegistryWrap TextEditorRegistryWrap { get; set; } = null!;
    [Inject]
    private IDecorationMapperRegistry DecorationMapperRegistry { get; set; } = null!;
    [Inject]
    private ICompilerServiceRegistry CompilerServiceRegistry { get; set; } = null!;
    [Inject]
    private WebsiteInitializationBackgroundTaskGroup WebsiteInitializationBackgroundTaskGroup { get; set; } = null!;

    protected override void OnInitialized()
    {
        TextEditorRegistryWrap.DecorationMapperRegistry = DecorationMapperRegistry;
        TextEditorRegistryWrap.CompilerServiceRegistry = CompilerServiceRegistry;

        base.OnInitialized();
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            WebsiteInitializationBackgroundTaskGroup.Enqueue_LuthetusWebsiteInitializerOnAfterRenderAsync();
        }

        return base.OnAfterRenderAsync(firstRender);
    }
}