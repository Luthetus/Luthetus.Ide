using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;

namespace Luthetus.TextEditor.RazorLib.FindAlls.States;

public partial record TextEditorFindAllState
{
	public class Effector : IDisposable
	{
		private readonly IFileSystemProvider _fileSystemProvider;
		private readonly IEnvironmentProvider _environmentProvider;
		private readonly ITreeViewService _treeViewService;
		private readonly IState<TextEditorFindAllState> _textEditorFindAllStateWrap;
		private readonly IDispatcher _dispatcher;
		private readonly Throttle _throttleSetSearchQuery = new Throttle(TimeSpan.FromMilliseconds(500));
		private readonly Throttle _throttleUiUpdate = new Throttle(Throttle.Thirty_Frames_Per_Second);
		
		public Effector(
			IFileSystemProvider fileSystemProvider,
			IEnvironmentProvider environmentProvider,
			ITreeViewService treeViewService,
			IState<TextEditorFindAllState> textEditorFindAllStateWrap,
			IDispatcher dispatcher)
		{
			_fileSystemProvider = fileSystemProvider;
			_environmentProvider = environmentProvider;
			_treeViewService = treeViewService;
			_textEditorFindAllStateWrap = textEditorFindAllStateWrap;
			_dispatcher = dispatcher;
		}
		
		[EffectMethod(typeof(StartSearchAction))]
		public Task HandleStartSearchAction(IDispatcher dispatcher)
		{
			_throttleSetSearchQuery.Run(async _ => 
			{
				dispatcher.Dispatch(new CancelSearchAction());
				dispatcher.Dispatch(new ClearSearchAction());
				
				var textEditorFindAllState = _textEditorFindAllStateWrap.Value;

				if (string.IsNullOrWhiteSpace(textEditorFindAllState.StartingDirectoryPath) ||
					string.IsNullOrWhiteSpace(textEditorFindAllState.SearchQuery))
				{
					return;
				}
				
				var cancellationToken = textEditorFindAllState._searchCancellationTokenSource.Token;
				var progressBarModel = new ProgressBarModel();

				ConstructTreeView(textEditorFindAllState);

				dispatcher.Dispatch(new SetProgressBarModelAction(progressBarModel));
				
				try
				{
					await StartSearchTask(
						progressBarModel,
						_fileSystemProvider,
						textEditorFindAllState,
						dispatcher,
						cancellationToken);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
			});

			return Task.CompletedTask;
		}
		
		private async Task StartSearchTask(
			ProgressBarModel progressBarModel,
			IFileSystemProvider fileSystemProvider,
			TextEditorFindAllState textEditorFindAllState,
			IDispatcher dispatcher,
			CancellationToken cancellationToken)
		{
			var filesProcessedCount = 0;
			var textSpanList = new List<TextEditorTextSpan>();
			var searchException = (Exception?)null;
			
			try
			{
				ShowFilesProcessedCountOnUi(0);
				await RecursiveSearch(textEditorFindAllState.StartingDirectoryPath);
			}
			catch (Exception e)
			{
				searchException = e;
			}
			finally
			{
				dispatcher.Dispatch(new FlushSearchResultsAction(textSpanList));
			
				if (searchException is null)
				{
					ShowFilesProcessedCountOnUi(1, true);
				}
				else
				{
					progressBarModel.SetProgress(
						progressBarModel.DecimalPercentProgress,
						searchException.ToString());
						
					progressBarModel.Dispose();
					// The use of '_textEditorFindAllStateWrap.Value' is purposeful.
					ConstructTreeView(_textEditorFindAllStateWrap.Value);
					dispatcher.Dispatch(new SetProgressBarModelAction(progressBarModel));
				}
			}
			
			async Task RecursiveSearch(string directoryPath)
			{
				cancellationToken.ThrowIfCancellationRequested();
				
				// Considering the use a breadth first algorithm
	
				// Search Files
				{
					var childFileList = await fileSystemProvider.Directory
						.GetFilesAsync(directoryPath)
						.ConfigureAwait(false);
		
					foreach (var childFile in childFileList)
					{
						await PerformSearchFile(childFile).ConfigureAwait(false);
						
						filesProcessedCount++;
						ShowFilesProcessedCountOnUi(0);
					}
				}
		
				// Recurse into subdirectories
				{
					var subdirectoryList = await fileSystemProvider.Directory
						.GetDirectoriesAsync(directoryPath)
						.ConfigureAwait(false);
		
					foreach (var subdirectory in subdirectoryList)
					{
						if (subdirectory.Contains(".git")  || subdirectory.Contains("bin") || subdirectory.Contains("obj"))
							continue;
		
						await RecursiveSearch(subdirectory).ConfigureAwait(false);
					}
				}
			}
			
			async Task PerformSearchFile(string filePath)
			{
				var text = await fileSystemProvider.File.ReadAllTextAsync(filePath).ConfigureAwait(false);
				var query = textEditorFindAllState.SearchQuery;
					
		        var matchedTextSpanList = new List<TextEditorTextSpan>();
		
		        for (int outerI = 0; outerI < text.Length; outerI++)
		        {
		            if (outerI + query.Length <= text.Length)
		            {
		                int innerI = 0;
		                for (; innerI < query.Length; innerI++)
		                {
		                    if (text[outerI + innerI] != query[innerI])
		                        break;
		                }
		
		                if (innerI == query.Length)
		                {
		                    // Then the entire query was matched
		                    matchedTextSpanList.Add(new TextEditorTextSpan(
		                        outerI,
		                        outerI + innerI,
		                        (byte)FindOverlayDecorationKind.LongestCommonSubsequence,
		                        new ResourceUri(filePath),
		                        text));
		                }
		            }
		        }
		
		        foreach (var matchedTextSpan in matchedTextSpanList)
		        {
					textSpanList.Add(matchedTextSpan);
		        }
			}
			
			void ShowFilesProcessedCountOnUi(double decimalPercentProgress, bool shouldDisposeProgressBarModel = false)
			{
				_throttleUiUpdate.Run(_ =>
				{
					progressBarModel.SetProgress(
						decimalPercentProgress,
						$"{filesProcessedCount:N0} files processed");
						
					if (shouldDisposeProgressBarModel)
					{
						progressBarModel.Dispose();
						// The use of '_textEditorFindAllStateWrap.Value' is purposeful.
						ConstructTreeView(_textEditorFindAllStateWrap.Value);
						dispatcher.Dispatch(new SetProgressBarModelAction(progressBarModel));
					}
						
					return Task.CompletedTask;
				});
			}
		}
		
		private void ConstructTreeView(TextEditorFindAllState textEditorFindAllState)
		{
		    var treeViewList = textEditorFindAllState.SearchResultList.Select(
		    	x => (TreeViewNoType)new TreeViewFindAllTextSpan(
			        x,
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
		
		    if (!_treeViewService.TryGetTreeViewContainer(TextEditorFindAllState.TreeViewFindAllContainerKey, out _))
		    {
		        _treeViewService.RegisterTreeViewContainer(new TreeViewContainer(
		            TextEditorFindAllState.TreeViewFindAllContainerKey,
		            adhocRoot,
		            activeNodes.ToImmutableList()));
		    }
		    else
		    {
		        _treeViewService.SetRoot(TextEditorFindAllState.TreeViewFindAllContainerKey, adhocRoot);
		
		        _treeViewService.SetActiveNode(
		            TextEditorFindAllState.TreeViewFindAllContainerKey,
		            firstNode,
		            true,
		            false);
		    }
		}
		
		// Count the amount of top level directories
				
		// Start at BlazorCrudApp/
		// Track 2 directories deep for the progress bar.
		// Percentage of the percentage
		//
		// Here BlazorCrudApp/ is made up of 50%
		// 	BlazorCrudApp.sln
		// 	BlazorCrudApp.ServerSide/
		
		// Get DirectoryList of BlazorCrudApp/
		// Get FileList of BlazorCrudApp/
		
		// Draw the tree view as it goes.
		
		// BlazorCrudApp.sln
		// BlazorCrudApp.ServerSide/
		// 	Properties/
		// 		launchSettings.json
		// 	wwwroot/
		// 		css/
		// 			site.css/
		// 		favicon.ico/
		// 	Pages/
		// 		_Host.cshtml
		// 		Error.cshtml.cs
		// 	_Imports.razor
		// 	App.razor
		// 	appsettings.Development.json
		
		/// <summary>
		/// TODO: If the app were to crash while this was running, would the Task be cancelled?...
		/// 	  ...Presumably, if Fluxor invokes the 'Dispose()' method then I could cancel it.
		/// 	  But I'm not sure if it does or not at the moment.
		/// </summary>
		public void Dispose()
		{
			_dispatcher.Dispatch(new CancelSearchAction());
		}
	}
}
