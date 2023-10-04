using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.ComponentRunners.Internals.PersonCase;

public partial class PersonSimpleDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public string FirstName { get; set; } = null!;
    [Parameter, EditorRequired]
    public string LastName { get; set; } = null!;

    public Guid Id { get; private set; } = Guid.NewGuid();

    public string DisplayName => $"{FirstName} {LastName}";
}