using Luthetus.Common.RazorLib.Reflectives.PersonCase;
using Luthetus.Ide.RazorLib.DotNetSolutions.Displays;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Shareds.Displays.Internals;

public partial class IdeDevelopmentDisplay : ComponentBase
{
    private readonly List<Type> _componentTypeList = new()
    {
        typeof(PersonDisplay),
        typeof(PersonSimpleDisplay),
        typeof(DotNetSolutionFormDisplay),
    };
}