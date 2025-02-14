using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.AppDatas.Models;
using Luthetus.Ide.RazorLib.CodeSearches.Models;
using Luthetus.Ide.RazorLib.StartupControls.Models;
using Luthetus.Extensions.DotNet.Nugets.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.Models;
using Luthetus.Extensions.DotNet.CommandLines.Models;
using Luthetus.Extensions.DotNet.CompilerServices.Models;
using Luthetus.Extensions.DotNet.TestExplorers.Models;
using Luthetus.Extensions.DotNet.ComponentRenderers.Models;
using Luthetus.Extensions.DotNet.Outputs.Models;

namespace Luthetus.Extensions.DotNet.BackgroundTasks.Models;

public class DotNetBackgroundTaskApi
{
	private readonly LuthetusCommonApi _commonApi;
	private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
	private readonly IAppDataService _appDataService;
	private readonly ICompilerServiceRegistry _compilerServiceRegistry;
	private readonly IDotNetComponentRenderers _dotNetComponentRenderers;
	private readonly IIdeComponentRenderers _ideComponentRenderers;
	private readonly DotNetCliOutputParser _dotNetCliOutputParser;
	private readonly ITextEditorService _textEditorService;
	private readonly IFindAllService _findAllService;
	private readonly ICodeSearchService _codeSearchService;
	private readonly IStartupControlService _startupControlService;
	private readonly ITerminalService _terminalService;

    public DotNetBackgroundTaskApi(
    	LuthetusCommonApi commonApi,
		IdeBackgroundTaskApi ideBackgroundTaskApi,
        IAppDataService appDataService,
		ICompilerServiceRegistry compilerServiceRegistry,
		IDotNetComponentRenderers dotNetComponentRenderers,
		IIdeComponentRenderers ideComponentRenderers,
		DotNetCliOutputParser dotNetCliOutputParser,
		ITextEditorService textEditorService,
		IFindAllService findAllService,
		ICodeSearchService codeSearchService,
		IStartupControlService startupControlService,
		ITerminalService terminalService,
        IServiceProvider serviceProvider)
	{
		_commonApi = commonApi;
		_ideBackgroundTaskApi = ideBackgroundTaskApi;
		_appDataService = appDataService;
        _dotNetComponentRenderers = dotNetComponentRenderers;
		_ideComponentRenderers = ideComponentRenderers;
		_dotNetCliOutputParser = dotNetCliOutputParser;
		_textEditorService = textEditorService;
		_findAllService = findAllService;
		_codeSearchService = codeSearchService;
		_startupControlService = startupControlService;
		_compilerServiceRegistry = compilerServiceRegistry;
		_terminalService = terminalService;

		DotNetSolutionService = new DotNetSolutionService();
		
		CompilerServiceExplorerService = new CompilerServiceExplorerService();

        CompilerService = new CompilerServiceIdeApi(
            commonApi,
            this,
            _ideBackgroundTaskApi,
			CompilerServiceExplorerService,
			_compilerServiceRegistry,
			_ideComponentRenderers);
			
		TestExplorerService = new TestExplorerService(
            this,
			_ideBackgroundTaskApi,
			DotNetSolutionService);

        TestExplorer = new TestExplorerScheduler(
            commonApi,
            this,
            _ideBackgroundTaskApi,
            _textEditorService,
            _dotNetCliOutputParser,
            DotNetSolutionService,
            _terminalService,
            TestExplorerService);
            
        OutputService = new OutputService(this);
            
        Output = new OutputScheduler(
            commonApi,
            this,
			_dotNetCliOutputParser,
			OutputService);

        DotNetSolution = new DotNetSolutionIdeApi(
            commonApi,
            _ideBackgroundTaskApi,
			_appDataService,
			CompilerServiceExplorerService,
            _dotNetComponentRenderers,
            _ideComponentRenderers,
			DotNetSolutionService,
			_textEditorService,
			_findAllService,
			_codeSearchService,
			_startupControlService,
			_compilerServiceRegistry,
			_terminalService,
			_dotNetCliOutputParser,
			serviceProvider);
			
			NuGetPackageManagerService = new NuGetPackageManagerService();
			
			CompilerServiceEditorService = new CompilerServiceEditorService();
	}

	public DotNetSolutionIdeApi DotNetSolution { get; }
    public CompilerServiceIdeApi CompilerService { get; }
    public TestExplorerScheduler TestExplorer { get; }
    public OutputScheduler Output { get; }
    public IOutputService OutputService { get; }
    public ITestExplorerService TestExplorerService { get; }
    public IDotNetSolutionService DotNetSolutionService { get; }
    public INuGetPackageManagerService NuGetPackageManagerService { get; }
    public ICompilerServiceEditorService CompilerServiceEditorService { get; }
    public ICompilerServiceExplorerService CompilerServiceExplorerService { get; }
}
