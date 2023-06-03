using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.CustomEvents;
using Luthetus.Common.RazorLib.Icons;
using Luthetus.Common.RazorLib.Icons.Codicon;
using Luthetus.Ide.RazorLib.ContextCase;
using Luthetus.Ide.RazorLib.Button;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;
using Luthetus.Ide.ClassLib.CompilerServices.Common.General;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.SemanticContext.CompilationUnitCase;

public partial class TreeViewCompilationUnitDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public CompilationUnit CompilationUnit { get; set; } = null!;
}