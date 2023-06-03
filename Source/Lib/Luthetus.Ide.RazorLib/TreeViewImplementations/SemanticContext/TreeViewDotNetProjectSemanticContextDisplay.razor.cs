using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.SemanticContextCase.Implementations;
using Luthetus.Ide.ClassLib.ComponentRenderers.Types;
using Luthetus.Ide.ClassLib.Namespaces;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.SemanticContext;

public partial class TreeViewDotNetProjectSemanticContextDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public DotNetProjectSemanticContext DotNetProjectSemanticContext { get; set; } = null!;
}