using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;

namespace Luthetus.Ide.RazorLib.CodeSearches.Models;

public class CodeSearchService : ICodeSearchService
{
	private readonly Throttle _throttle = new(TimeSpan.FromMilliseconds(300));
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly ITreeViewService _treeViewService;

    public CodeSearchService(
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        ITreeViewService treeViewService)
    {
        _fileSystemProvider = fileSystemProvider;
        _environmentProvider = environmentProvider;
        _treeViewService = treeViewService;
    }
    
    private CodeSearchState _codeSearchState = new();
    
    public event Action? CodeSearchStateChanged;
    
    public CodeSearchState GetCodeSearchState() => _codeSearchState;
    
    public void ReduceWithAction(Func<CodeSearchState, CodeSearchState> withFunc)
    {
    	var inState = GetCodeSearchState();
    
        var outState = withFunc.Invoke(inState);

        if (outState.Query.StartsWith("f:"))
        {
            outState = outState with
            {
                CodeSearchFilterKind = CodeSearchFilterKind.Files
            };
        }
        else if (outState.Query.StartsWith("t:"))
        {
            outState = outState with
            {
                CodeSearchFilterKind = CodeSearchFilterKind.Types
            };
        }
        else if (outState.Query.StartsWith("m:"))
        {
            outState = outState with
            {
                CodeSearchFilterKind = CodeSearchFilterKind.Members
            };
        }
        else
        {
            outState = outState with
            {
                CodeSearchFilterKind = CodeSearchFilterKind.None
            };
        }

        _codeSearchState = outState;
        CodeSearchStateChanged?.Invoke();
        return;
    }

    public void ReduceAddResultAction(string result)
    {
    	var inState = GetCodeSearchState();
    
        _codeSearchState = inState with
        {
            ResultList = inState.ResultList.Add(result)
        };
        CodeSearchStateChanged?.Invoke();
        return;
    }

    public void ReduceClearResultListAction()
    {
    	var inState = GetCodeSearchState();
    
        _codeSearchState = inState with
        {
            ResultList = ImmutableList<string>.Empty
        };
        CodeSearchStateChanged?.Invoke();
        return;
    }
    
    public void ReduceInitializeResizeHandleDimensionUnitAction(DimensionUnit dimensionUnit)
    {
    	var inState = GetCodeSearchState();
    
        if (dimensionUnit.Purpose != DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_ROW)
        {
        	CodeSearchStateChanged?.Invoke();
        	return;
        }
        
        // TopContentElementDimensions
        {
        	if (inState.TopContentElementDimensions.HeightDimensionAttribute.DimensionUnitList is null)
        	{
        		CodeSearchStateChanged?.Invoke();
        		return;
        	}
        		
        	var existingDimensionUnit = inState.TopContentElementDimensions.HeightDimensionAttribute.DimensionUnitList
        		.FirstOrDefault(x => x.Purpose == dimensionUnit.Purpose);
        		
            if (existingDimensionUnit.Purpose is not null)
            {
            	CodeSearchStateChanged?.Invoke();
        		return;
            }
        		
        	inState.TopContentElementDimensions.HeightDimensionAttribute.DimensionUnitList.Add(dimensionUnit);
        }
        
        // BottomContentElementDimensions
        {
        	if (inState.BottomContentElementDimensions.HeightDimensionAttribute.DimensionUnitList is null)
        	{
        		CodeSearchStateChanged?.Invoke();
        		return;
        	}
        		
        	var existingDimensionUnit = inState.BottomContentElementDimensions.HeightDimensionAttribute.DimensionUnitList.FirstOrDefault(x => x.Purpose == dimensionUnit.Purpose);
            if (existingDimensionUnit.Purpose is not null)
            {
            	CodeSearchStateChanged?.Invoke();
        		return;
            }
        		
        	inState.BottomContentElementDimensions.HeightDimensionAttribute.DimensionUnitList.Add(dimensionUnit);
        }
        
        CodeSearchStateChanged?.Invoke();
        return;
    }

    /// <summary>
    /// TODO: This method makes use of <see cref="IThrottle"/> and yet is accessing...
    ///       ...searchEffect.CancellationToken.
    ///       The issue here is that the search effect parameter to this method
    ///       could be out of date by the time that the throttle delay is completed.
    ///       This should be fixed. (2024-05-02)
    /// </summary>
    /// <param name="searchEffect"></param>
    /// <param name="dispatcher"></param>
    /// <returns></returns>
    public Task HandleSearchEffect(CancellationToken cancellationToken = default)
    {
        _throttle.Run(async _ =>
        {
            ReduceClearResultListAction();

            var codeSearchState = GetCodeSearchState();
            ConstructTreeView(codeSearchState);

            var startingAbsolutePathForSearch = codeSearchState.StartingAbsolutePathForSearch;

            if (string.IsNullOrWhiteSpace(startingAbsolutePathForSearch) ||
            	string.IsNullOrWhiteSpace(codeSearchState.Query))
            {
                return;
            }

            await RecursiveHandleSearchEffect(startingAbsolutePathForSearch).ConfigureAwait(false);
            
            // The use of '_codeSearchStateWrap.Value' is purposeful here
            ConstructTreeView(GetCodeSearchState());

            async Task RecursiveHandleSearchEffect(string directoryPathParent)
            {
                var directoryPathChildList = await _fileSystemProvider.Directory.GetDirectoriesAsync(
                        directoryPathParent,
                        cancellationToken)
                    .ConfigureAwait(false);

                var filePathChildList = await _fileSystemProvider.Directory.GetFilesAsync(
                        directoryPathParent,
                        cancellationToken)
                    .ConfigureAwait(false);

                foreach (var filePathChild in filePathChildList)
                {
                	var absolutePath = _environmentProvider.AbsolutePathFactory(filePathChild, false);
                
                    if (absolutePath.NameWithExtension.Contains(codeSearchState.Query))
                        ReduceAddResultAction(filePathChild);
                }

                foreach (var directoryPathChild in directoryPathChildList)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    if (directoryPathChild.Contains(".git") ||
						directoryPathChild.Contains(".vs") ||
						directoryPathChild.Contains(".vscode") ||
						directoryPathChild.Contains(".idea") ||
						directoryPathChild.Contains("bin") ||
						directoryPathChild.Contains("obj"))
					{
						continue;
					}

                    await RecursiveHandleSearchEffect(directoryPathChild).ConfigureAwait(false);
                }
            }
        });
        
        return Task.CompletedTask;
    }
    
    private void ConstructTreeView(CodeSearchState codeSearchState)
	{
	    var treeViewList = codeSearchState.ResultList.Select(
	    	x => (TreeViewNoType)new TreeViewCodeSearchTextSpan(
		        new TextEditorTextSpan(
		        	0,
			        0,
			        (byte)GenericDecorationKind.None,
			        new ResourceUri(x),
			        string.Empty),
				_environmentProvider,
				_fileSystemProvider,
				false,
				false))
			.ToArray();
	
	    var adhocRoot = TreeViewAdhoc.ConstructTreeViewAdhoc(treeViewList);
	    var firstNode = treeViewList.FirstOrDefault();
	
	    var activeNodes = firstNode is null
	        ? TreeViewNoType.GetEmptyTreeViewNoTypeList()
	        : new() { firstNode };
	
	    if (!_treeViewService.TryGetTreeViewContainer(CodeSearchState.TreeViewCodeSearchContainerKey, out _))
	    {
	        _treeViewService.ReduceRegisterContainerAction(new TreeViewContainer(
	            CodeSearchState.TreeViewCodeSearchContainerKey,
	            adhocRoot,
	            activeNodes));
	    }
	    else
	    {
	        _treeViewService.ReduceWithRootNodeAction(CodeSearchState.TreeViewCodeSearchContainerKey, adhocRoot);
	
	        _treeViewService.ReduceSetActiveNodeAction(
	            CodeSearchState.TreeViewCodeSearchContainerKey,
	            firstNode,
	            true,
	            false);
	    }
	}
}
