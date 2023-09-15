using Luthetus.Ide.RazorLib.ViewsCase.Models;

namespace Luthetus.Ide.RazorLib.FooterCase.States;

public partial record FooterState
{
    public record SetFooterStateViewAction(View View);
}