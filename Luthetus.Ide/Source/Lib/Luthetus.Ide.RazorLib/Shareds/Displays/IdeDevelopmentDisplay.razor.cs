using Luthetus.Common.RazorLib.Reflectives.PersonCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Shareds.Displays;

public partial class IdeDevelopmentDisplay : ComponentBase
{
    private readonly List<Type> _componentTypeList = new()
    {
        typeof(PersonDisplay),
        typeof(PersonSimpleDisplay),
    };
}