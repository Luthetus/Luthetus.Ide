using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.ComponentRunners.Internals.PersonCase;

public partial class PersonDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public PersonModel PersonModel { get; set; } = null!;
}