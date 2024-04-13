using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Reflectives.PersonCase;

/// <summary>
/// <see cref="PersonDisplay"/> is used from within the unit tests,
/// in order to keep around an un-changing component for the Reflectives.
/// </summary>
public partial class PersonDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public PersonModel PersonModel { get; set; } = null!;
}