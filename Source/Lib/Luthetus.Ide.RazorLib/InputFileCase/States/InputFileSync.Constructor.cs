using Luthetus.Ide.RazorLib.ComponentRenderersCase.Models;

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