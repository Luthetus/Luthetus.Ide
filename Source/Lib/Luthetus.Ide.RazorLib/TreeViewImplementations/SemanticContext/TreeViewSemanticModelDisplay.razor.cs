using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.SemanticContextCase.Implementations;
using Luthetus.Ide.ClassLib.ComponentRenderers.Types;
using Luthetus.Ide.ClassLib.Namespaces;
using Luthetus.TextEditor.RazorLib.Semantics;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.SemanticContext;

public partial class TreeViewSemanticModelDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public ISemanticModel SemanticModel { get; set; } = null!;
}