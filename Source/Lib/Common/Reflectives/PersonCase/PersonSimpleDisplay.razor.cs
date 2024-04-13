using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Reflectives.PersonCase;

/// <summary>
/// <see cref="PersonSimpleDisplay"/> is used from within the unit tests,
/// in order to keep around an un-changing component for the Reflectives.
/// </summary>
public partial class PersonSimpleDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public string FirstName { get; set; } = null!;
    [Parameter, EditorRequired]
    public string LastName { get; set; } = null!;

    public Guid Id { get; private set; } = Guid.NewGuid();

    public string DisplayName => $"{FirstName} {LastName}";
}