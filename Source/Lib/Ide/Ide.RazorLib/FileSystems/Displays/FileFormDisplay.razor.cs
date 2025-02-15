using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Ide.RazorLib.FileSystems.Displays;

public partial class FileFormDisplay : ComponentBase, IFileFormRendererType
{
    [CascadingParameter]
    public MenuOptionCallbacks? MenuOptionCallbacks { get; set; }

    [Parameter, EditorRequired]
    public string FileName { get; set; } = string.Empty;
    [Parameter, EditorRequired]
    public Func<string, IFileTemplate?, List<IFileTemplate>, Task> OnAfterSubmitFunc { get; set; } = null!;
    
    [Parameter]
    public bool IsDirectory { get; set; }
    [Parameter]
    public bool CheckForTemplates { get; set; }

    private string? _previousFileNameParameter;
    private string _fileName = string.Empty;
    private FileTemplatesDisplay? _fileTemplatesDisplay;
    private ElementReference? _inputElementReference;

    private string PlaceholderText => IsDirectory ? "Directory name" : "File name";
    public string InputFileName => _fileName;

    protected override Task OnParametersSetAsync()
    {
        if (_previousFileNameParameter is null || _previousFileNameParameter != FileName)
        {
            _previousFileNameParameter = FileName;
            _fileName = FileName;
        }

        return base.OnParametersSetAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (MenuOptionCallbacks is not null &&
                _inputElementReference is not null)
            {
                try
                {
                    await _inputElementReference.Value
                        .FocusAsync()
                        .ConfigureAwait(false);
                }
                catch (Exception)
                {
                    // 2023-04-18: The app has had a bug where it "freezes" and must be restarted.
                    //             This bug is seemingly happening randomly. I have a suspicion
                    //             that there are race-condition exceptions occurring with "FocusAsync"
                    //             on an ElementReference.
                }
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (MenuOptionCallbacks is not null)
        {
            if (keyboardEventArgs.Key == KeyboardKeyFacts.MetaKeys.ESCAPE)
            {
                await MenuOptionCallbacks.HideWidgetAsync
                    .Invoke()
                    .ConfigureAwait(false);
            }
            else if (keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
            {
                await MenuOptionCallbacks.CompleteWidgetAsync
                    .Invoke(async () => await OnAfterSubmitFunc.Invoke(
                        _fileName,
                        _fileTemplatesDisplay?.ExactMatchFileTemplate,
                        _fileTemplatesDisplay?.RelatedMatchFileTemplates ?? new())
                    .ConfigureAwait(false));
            }
        }
    }
}