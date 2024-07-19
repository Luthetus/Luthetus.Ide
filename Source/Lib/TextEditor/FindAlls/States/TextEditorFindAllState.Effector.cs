using Fluxor;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.TextEditor.RazorLib.FindAlls.States;

public partial record TextEditorFindAllState
{
	public class Effector : IDisposable
	{
		private readonly IFileSystemProvider _fileSystemProvider;
		private readonly IState<TextEditorFindAllState> _textEditorFindAllStateWrap;
		private readonly IDispatcher _dispatcher;
		private readonly Throttle _throttleSetSearchQuery = new Throttle(TimeSpan.FromMilliseconds(500));
		
		public Effector(
			IFileSystemProvider fileSystemProvider,
			IState<TextEditorFindAllState> textEditorFindAllStateWrap,
			IDispatcher dispatcher)
		{
			_fileSystemProvider = fileSystemProvider;
			_textEditorFindAllStateWrap = textEditorFindAllStateWrap;
			_dispatcher = dispatcher;
		}
		
		[EffectMethod(typeof(StartSearchAction))]
		public Task HandleStartSearchAction(IDispatcher dispatcher)
		{
			_throttleSetSearchQuery.Run(_ => 
			{
				dispatcher.Dispatch(new CancelSearchAction());
				
				var progressBarModel = new ProgressBarModel();
				
				var searchTask = (Task?)null;
				
				try
				{
					searchTask = StartSearchTask(
						progressBarModel,
						_fileSystemProvider,
						_textEditorFindAllStateWrap.Value,
						dispatcher);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
				
				dispatcher.Dispatch(new SetSearchTaskAction(searchTask, progressBarModel));
				return searchTask;
			});

			return Task.CompletedTask;
		}
		
		private async Task StartSearchTask(
			ProgressBarModel progressBarModel,
			IFileSystemProvider fileSystemProvider,
			TextEditorFindAllState textEditorFindAllState,
			IDispatcher dispatcher)
		{
			var filePathList = new List<string>();
			
			try
			{
				for (int i = 0; i < 10; i++)
				{
					progressBarModel.SetProgress(i/(10.0));
					
					filePathList.Add(i.ToString());
					
					dispatcher.Dispatch(new FlushSearchResultsAction(filePathList));
					await Task.Delay(500);
				}
			}
			finally
			{
				progressBarModel.SetProgress(1.0);
				progressBarModel.Dispose();
			}
		}
		
		/*
	    public Task SearchAsync(string searchQuery, CancellationToken cancellationToken = default)
	    {
			if (_runCount != 0)
				return Task.CompletedTask;
	
			_runCount++;
	
			_ =  Task.Run(async () => 
			{
				var textEditorFindAllState = _textEditorFindAllStateWrap.Value;
	
				FilePathList.Clear();
				IsSearching = true;
				ProgressOccurred?.Invoke();
				
				await RecursiveSearchAsync(textEditorFindAllState.StartingDirectoryPath, searchQuery, cancellationToken).ConfigureAwait(false);
				IsSearching = false;
				ProgressOccurred?.Invoke();
				_runCount--;
			}).ConfigureAwait(false);
	
			return Task.CompletedTask;
	    }
	
		private async Task RecursiveSearchAsync(string directoryPath, string searchQuery, CancellationToken cancellationToken = default)
		{
			// Considering the use a breadth first algorithm
	
			// Search Files
			{
				var childFileList = await _fileSystemProvider.Directory
					.GetFilesAsync(directoryPath)
					.ConfigureAwait(false);
	
				foreach (var childFile in childFileList)
				{
					await PerformSearchFileAsync(childFile, searchQuery, cancellationToken).ConfigureAwait(false);
				}
			}
	
			// Update UI with progress
			{
				ProgressOccurred?.Invoke();
			}
	
			// Recurse into subdirectories
			{
				var subdirectoryList = await _fileSystemProvider.Directory
					.GetDirectoriesAsync(directoryPath)
					.ConfigureAwait(false);
	
				foreach (var subdirectory in subdirectoryList)
				{
					if (subdirectory.Contains(".git")  || subdirectory.Contains("bin") || subdirectory.Contains("obj"))
						continue;
	
					await RecursiveSearchAsync(subdirectory, searchQuery, cancellationToken).ConfigureAwait(false);
				}
			}
		}
	
		private async Task PerformSearchFileAsync(string filePath, string searchQuery, CancellationToken cancellationToken = default)
		{
			var contents = await _fileSystemProvider.File.ReadAllTextAsync(filePath).ConfigureAwait(false);
	
			if (contents.Contains(searchQuery))
				FilePathList.Add(filePath);
		}
		*/
		
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
