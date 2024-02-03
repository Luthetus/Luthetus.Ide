using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;

namespace Luthetus.Ide.RazorLib.CodeSearches.States;

public partial record CodeSearchState
{
    public class Effector : IDisposable
    {
        private readonly IThrottle _throttle = new Throttle(TimeSpan.FromMilliseconds(300));
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

        [EffectMethod]
        public async Task HandleSearchEffect(
            SearchEffect searchEffect,
            IDispatcher dispatcher)
        {
            _throttle.FireAndForget(async _ =>
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

                var startingAbsolutePathForSearch = parentDirectory.Path;

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
