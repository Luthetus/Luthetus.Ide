using Luthetus.Common.RazorLib.FileSystems.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.FileSystems.Displays;

public partial class PermissionsDisplay : ComponentBase
{
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
}