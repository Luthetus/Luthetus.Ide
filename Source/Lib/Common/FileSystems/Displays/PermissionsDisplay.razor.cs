using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.FileSystems.Displays;

public partial class PermissionsDisplay : ComponentBase
{
    [Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;

    private string _deleteAllowPathTextInput = string.Empty;
    private bool _deleteAllowPathIsDirectoryInput;

    private string _protectPathTextInput = string.Empty;
    private bool _protectPathIsDirectoryInput;

    private void AddModifyDeleteRightsOnClick(
        string localProtectPathTextInput,
        bool localProtectPathIsDirectoryInput)
    {
		CommonApi.EnvironmentProviderApi.DeletionPermittedRegister(new SimplePath(
            localProtectPathTextInput,
            localProtectPathIsDirectoryInput));
    }
    
    private void SubmitProtectOnClick(
        string localProtectPathTextInput,
        bool localProtectPathIsDirectoryInput)
    {
		CommonApi.EnvironmentProviderApi.ProtectedPathsRegister(new SimplePath(
            localProtectPathTextInput,
            localProtectPathIsDirectoryInput));
    }
}