using Luthetus.Ide.ClassLib.CompilerServices.ParserTaskCase;
using Luthetus.Ide.ClassLib.ComponentRenderers.Types;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.ParserTaskCase;

public partial class ParserTaskDisplay : ComponentBase, IParserTaskDisplayRendererType
{
    [Parameter, EditorRequired]
    public IParserTask ParserTask { get; set; } = null!;
}