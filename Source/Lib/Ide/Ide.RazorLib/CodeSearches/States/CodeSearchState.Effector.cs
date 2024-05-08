using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;

namespace Luthetus.Ide.RazorLib.CodeSearches.States;

public partial record CodeSearchState
{
    public class Effector : IDisposable
    {
        private readonly ThrottleAsync _throttle = new ThrottleAsync(TimeSpan.FromMilliseconds(300));
        private readonly IState<CodeSearchState> _codeSearchStateWrap;
        private readonly IState<DotNetSolutionState> _dotNetSolutionStateWrap;
        private readonly IFileSystemProvider _fileSystemProvider;

        public Effector(
            IState<CodeSearchState> codeSearchStateWrap,
            IState<DotNetSolutionState> dotNetSolutionStateWrap,
            IFileSystemProvider fileSystemProvider)
        {
            _codeSearchStateWrap = codeSearchStateWrap;
            _dotNetSolutionStateWrap = dotNetSolutionStateWrap;
            _fileSystemProvider = fileSystemProvider;
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
                var dotNetSolutionState = _dotNetSolutionStateWrap.Value;
                var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionModel;

                if (dotNetSolutionModel is null)
                    return;

                var parentDirectory = dotNetSolutionModel.AbsolutePath.ParentDirectory;

                if (parentDirectory is null)
                    return;

                var startingAbsolutePathForSearch = parentDirectory.Value;

                dispatcher.Dispatch(new WithAction(inState => inState with
                {
                    StartingAbsolutePathForSearch = startingAbsolutePathForSearch
                }));

                await RecursiveHandleSearchEffect(startingAbsolutePathForSearch);

                async Task RecursiveHandleSearchEffect(string directoryPathParent)
                {
                    var directoryPathChildList = await _fileSystemProvider.Directory.GetDirectoriesAsync(
                        directoryPathParent,
                        searchEffect.CancellationToken);

                    var filePathChildList = await _fileSystemProvider.Directory.GetFilesAsync(
                        directoryPathParent,
                        searchEffect.CancellationToken);

                    foreach (var filePathChild in filePathChildList)
                    {
                        if (filePathChild.Contains(codeSearchState.Query))
                            dispatcher.Dispatch(new AddResultAction(filePathChild));
                    }

                    foreach (var directoryPathChild in directoryPathChildList)
                    {
                        if (searchEffect.CancellationToken.IsCancellationRequested)
                            return;

                        if (directoryPathChild.Contains(".git") || directoryPathChild.Contains("bin") || directoryPathChild.Contains("obj"))
                            continue;

                        await RecursiveHandleSearchEffect(directoryPathChild);
                    }
                }
            });
        }

        public void Dispose()
        {
            _throttle.Dispose();
        }
    }
}
