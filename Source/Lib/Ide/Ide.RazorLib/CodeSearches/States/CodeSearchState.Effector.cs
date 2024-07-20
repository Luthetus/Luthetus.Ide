using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.Ide.RazorLib.CodeSearches.Models;

namespace Luthetus.Ide.RazorLib.CodeSearches.States;

public partial record CodeSearchState
{
    public class Effector
    {
        private readonly ThrottleAsync _throttle = new ThrottleAsync(TimeSpan.FromMilliseconds(300));
        private readonly IState<CodeSearchState> _codeSearchStateWrap;
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly IEnvironmentProvider _environmentProvider;
        private readonly ITreeViewService _treeViewService;

        public Effector(
            IState<CodeSearchState> codeSearchStateWrap,
            IFileSystemProvider fileSystemProvider,
            IEnvironmentProvider environmentProvider,
            ITreeViewService treeViewService)
        {
            _codeSearchStateWrap = codeSearchStateWrap;
            _fileSystemProvider = fileSystemProvider;
            _environmentProvider = environmentProvider;
            _treeViewService = treeViewService;
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
        [EffectMethod]
        public Task HandleSearchEffect(
            SearchEffect searchEffect,
            IDispatcher dispatcher)
        {
            return _throttle.PushEvent(async _ =>
            {
                dispatcher.Dispatch(new ClearResultListAction());

                var codeSearchState = _codeSearchStateWrap.Value;
                ConstructTreeView(codeSearchState);

                var startingAbsolutePathForSearch = codeSearchState.StartingAbsolutePathForSearch;

                if (string.IsNullOrWhiteSpace(startingAbsolutePathForSearch) ||
                	string.IsNullOrWhiteSpace(codeSearchState.Query))
                {
                    return;
                }

                await RecursiveHandleSearchEffect(startingAbsolutePathForSearch).ConfigureAwait(false);
                
                // The use of '_codeSearchStateWrap.Value' is purposeful here
                ConstructTreeView(_codeSearchStateWrap.Value);

                async Task RecursiveHandleSearchEffect(string directoryPathParent)
                {
                    var directoryPathChildList = await _fileSystemProvider.Directory.GetDirectoriesAsync(
                            directoryPathParent,
                            searchEffect.CancellationToken)
                        .ConfigureAwait(false);

                    var filePathChildList = await _fileSystemProvider.Directory.GetFilesAsync(
                            directoryPathParent,
                            searchEffect.CancellationToken)
                        .ConfigureAwait(false);

                    foreach (var filePathChild in filePathChildList)
                    {
                        if (filePathChild.Contains(codeSearchState.Query))
                            dispatcher.Dispatch(new AddResultAction(filePathChild));
                    }

                    foreach (var directoryPathChild in directoryPathChildList)
                    {
                        if (searchEffect.CancellationToken.IsCancellationRequested)
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
		        ? Array.Empty<TreeViewNoType>()
		        : new[] { firstNode };
		
		    if (!_treeViewService.TryGetTreeViewContainer(CodeSearchState.TreeViewCodeSearchContainerKey, out _))
		    {
		        _treeViewService.RegisterTreeViewContainer(new TreeViewContainer(
		            CodeSearchState.TreeViewCodeSearchContainerKey,
		            adhocRoot,
		            activeNodes.ToImmutableList()));
		    }
		    else
		    {
		        _treeViewService.SetRoot(CodeSearchState.TreeViewCodeSearchContainerKey, adhocRoot);
		
		        _treeViewService.SetActiveNode(
		            CodeSearchState.TreeViewCodeSearchContainerKey,
		            firstNode,
		            true,
		            false);
		    }
		}
    }
}
