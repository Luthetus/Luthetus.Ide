using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.Models;

namespace Luthetus.Ide.RazorLib.FindAllReferences;

public partial class FindAllReferencesDisplay : ComponentBase
{
	public static readonly Key<Panel> FindAllReferencesPanelKey = Key<Panel>.NewKey();
    public static readonly Key<IDynamicViewModel> FindAllReferencesDynamicViewModelKey = Key<IDynamicViewModel>.NewKey();
}