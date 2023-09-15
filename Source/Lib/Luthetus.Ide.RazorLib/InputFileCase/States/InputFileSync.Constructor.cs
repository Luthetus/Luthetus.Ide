using Fluxor;
using Luthetus.Common.RazorLib.Dialog;
using Luthetus.Common.RazorLib.Store.DialogCase;
using Luthetus.Ide.RazorLib.ComponentRenderersCase.Models;
using Luthetus.Ide.RazorLib.HtmlCase.Models;
using static Luthetus.Ide.RazorLib.InputFileCase.States.InputFileState;

namespace Luthetus.Ide.RazorLib.InputFileCase.States;

public partial record InputFileSync
{
    private readonly ILuthetusIdeComponentRenderers _luthetusIdeComponentRenderers;

    public InputFileSync(
        ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers)
    {
        _luthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
    }
}