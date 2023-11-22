using Luthetus.Common.RazorLib.ComponentRunners.PersonCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.ComponentRunners.PersonCase;

/// <summary>
/// <see cref="PersonDisplay"/> is used from within the unit tests,
/// in order to keep around an un-changing component for the ComponentRunners.
/// </summary>
public partial class PersonDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public PersonModel PersonModel { get; set; } = null!;
}