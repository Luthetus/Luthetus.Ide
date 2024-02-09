using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.ProgramExecutions.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Debugger;

public partial class DebuggerDisplay : ComponentBase
{
    [Inject]
    private IState<ProgramExecutionState> ProgramExecutionStateWrap { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;

    private string InputElementIdForDebuggerAbsolutePath = "luth_debugger__debuggerAbsolutePath-input";
    private string InputElementIdForInterpreterType = "luth_debugger__interpreterType-input";
    private string InputElementIdForDotNetAbsolutePath = "luth_debugger__dotNetAbsolutePath-input";
    private string InputElementIdForProgramDllAbsolutePath = "luth_debugger__programDllAbsolutePath-input";
    private string InputElementIdForProcessId = "luth_debugger__processId-input";

    private string _debuggerAbsolutePathBind = "netcoredbg";
    private string _interpreterTypeBind = "cli";
    private string _dotNetAbsolutePathBind = "dotnet";
    private string _programDllAbsolutePathBind = string.Empty;
    private string _processIdBind = string.Empty;
    
    private string _debuggerAbsolutePathDefault = "/path/to/netcoredbg";
    private string _interpreterTypeDefault = "TYPE{cli, mi, vscode}";
    private string _dotNetAbsolutePathDefault = "/path/to/dotnet";
    private string _programDllAbsolutePathDefault = "/path/to/program.dll";
    private string _processIdDefault = "process id";
    
    private string ShowDebuggerAbsolutePath => string.IsNullOrWhiteSpace(_debuggerAbsolutePathBind) ? _debuggerAbsolutePathDefault : _debuggerAbsolutePathBind;
    private string ShowInterpreterType => string.IsNullOrWhiteSpace(_interpreterTypeBind) ? _interpreterTypeDefault : _interpreterTypeBind;
    private string ShowDotNetAbsolutePath => string.IsNullOrWhiteSpace(_dotNetAbsolutePathBind) ? _dotNetAbsolutePathDefault : _dotNetAbsolutePathBind;
    private string ShowProgramDllAbsolutePath => string.IsNullOrWhiteSpace(_programDllAbsolutePathBind) ? _programDllAbsolutePathDefault : _programDllAbsolutePathBind;

    private string FormattedCommand => 
        $"{ShowDebuggerAbsolutePath}" +
        " " +
        $"--interpreter={ShowInterpreterType}" +
        " -- " +
        $"{ShowDotNetAbsolutePath}" +
        " " +
        $"{ShowProgramDllAbsolutePath}";

    protected override void OnInitialized()
    {
        var startupProjectAbsolutePath = GetStartupProjectAbsolutePath();

        if (!string.IsNullOrWhiteSpace(startupProjectAbsolutePath))
            _programDllAbsolutePathBind = startupProjectAbsolutePath;

        base.OnInitialized();
    }

    private string GetStartupProjectAbsolutePath()
    {
        var programExecutionState = ProgramExecutionStateWrap.Value;

        if (programExecutionState.StartupProjectAbsolutePath is null)
            return string.Empty;

        var ancestorDirectory = programExecutionState.StartupProjectAbsolutePath.ParentDirectory;

        if (ancestorDirectory is null)
            return string.Empty;

        return EnvironmentProvider.JoinPaths(
            ancestorDirectory.Path,
                $"bin{EnvironmentProvider.DirectorySeparatorChar}" +
                $"Debug{EnvironmentProvider.DirectorySeparatorChar}" +
                $"net6.0{EnvironmentProvider.DirectorySeparatorChar}");
    }
}