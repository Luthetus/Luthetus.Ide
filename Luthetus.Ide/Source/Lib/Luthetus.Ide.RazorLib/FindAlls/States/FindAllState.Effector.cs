using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Reactives.Models;

namespace Luthetus.Ide.RazorLib.FindAlls.States;

public partial record FindAllState
{
    public class Effector : IDisposable
    {
        private readonly IThrottle _throttle = new Throttle(TimeSpan.FromMilliseconds(300));
        private readonly IState<FindAllState> _findAllStateWrap;
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly IEnvironmentProvider _environmentProvider;

        public Effector(
            IState<FindAllState> findAllStateWrap,
            IFileSystemProvider fileSystemProvider,
            IEnvironmentProvider environmentProvider)
        {
            _findAllStateWrap = findAllStateWrap;
            _fileSystemProvider = fileSystemProvider;
            _environmentProvider = environmentProvider;
        }

        [EffectMethod]
        public async Task HandleSearchEffect(
            SearchEffect searchEffect,
            IDispatcher dispatcher)
        {
            await _throttle.FireAsync(async _ =>
            {
                var findAllState = _findAllStateWrap.Value;

                await RecursiveHandleSearchEffect(_environmentProvider.RootDirectoryAbsolutePath.Value);

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
                        if (filePathChild.Contains(findAllState.Query))
                            dispatcher.Dispatch(new AddResultAction(filePathChild));
                    }

                    foreach (var directoryPathChild in directoryPathChildList)
                    {
                        if (searchEffect.CancellationToken.IsCancellationRequested)
                            return;

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
