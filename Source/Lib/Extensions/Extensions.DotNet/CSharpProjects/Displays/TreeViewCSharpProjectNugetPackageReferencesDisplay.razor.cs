using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Options.Models;

namespace Luthetus.Extensions.DotNet.CSharpProjects.Displays;

public partial class TreeViewCSharpProjectNugetPackageReferencesDisplay : ComponentBase
{
    [Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;
}