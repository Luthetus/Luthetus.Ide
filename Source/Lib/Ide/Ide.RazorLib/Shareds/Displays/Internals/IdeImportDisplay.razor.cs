using System.IO.Compression;
using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Ide.RazorLib.Exceptions;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Ide.RazorLib.Shareds.Displays.Internals;

public partial class IdeImportDisplay : ComponentBase, IDisposable
{
    [Inject]
    private HttpClient HttpClient { get; set; } = null!;
    [Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;

    private readonly object RequestRepoContentLock = new();

    private string _owner = "huntercfreeman";
    private string _repo = "BlazorApp";
    private string _ref = string.Empty;

    private ImportPhase _activePhase = ImportPhase.None;
    private string _activeQuery = string.Empty;
    private int _totalFilesInZipArchive;
    private int _processedFilesInZipArchive;
    private string _nameOfEntryMostRecentlyProcessed = string.Empty;
    private string? _errorMessage;

    private CancellationTokenSource _cancellationTokenSource = new();
    private CancellationToken? _activeCancellationToken = null;

    private string _parametersForFinishedQuery = string.Empty;

    public enum ImportPhase
    {
        None,
        RequestRepoContents,
        ReadZipArchive,
        Error,
        Finished,
    }

    private async Task LoadFromUrlOnClick()
    {
        var localOwner = _owner;
        var localRepo = _repo;
        var localRef = _ref;
        var localActiveQuery = string.Empty;

        if (_activeCancellationToken is not null)
            return;

        lock (RequestRepoContentLock)
        {
            if (_activeCancellationToken is not null)
                return;

            _activeCancellationToken = _cancellationTokenSource.Token;

            localActiveQuery = 
                _activeQuery =
                $"https://api.github.com/repos/{localOwner}/{localRepo}/zipball/{localRef}";
        }

        try
        {
            // UI progress indicator
            {
                _errorMessage = null;
                _activePhase = ImportPhase.RequestRepoContents;
                await InvokeAsync(StateHasChanged);
            }

            _activeCancellationToken.Value.ThrowIfCancellationRequested();

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"https://api.github.com/repos/{localOwner}/{localRepo}/zipball/{localRef}");

            request.Headers.Add("Accept", "application/vnd.github+json");
            request.Headers.Add("User-Agent", "Luthetus");
            // request.Headers.Add("Authorization", 222"Bearer <YOUR-TOKEN>");
            request.Headers.Add("X-GitHub-Api-Version", "2022-11-28");

            var response = await HttpClient.SendAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var zipArchive = new ZipArchive(responseStream);

                // UI progress indicator
                {
                    _processedFilesInZipArchive = 0;
                    _totalFilesInZipArchive = zipArchive.Entries.Count;
                    _activePhase = ImportPhase.ReadZipArchive;
                    await InvokeAsync(StateHasChanged);
                }

                foreach (var entry in zipArchive.Entries)
                {
                    _activeCancellationToken.Value.ThrowIfCancellationRequested();

                    var stream = entry.Open();
                    var streamReader = new StreamReader(stream);
                    var contents = await streamReader.ReadToEndAsync().ConfigureAwait(false);

                    // Add the file to the in-memory filesystem.
                    // (all the code in this file is for the demo website)
                    {
                        if (string.IsNullOrWhiteSpace(entry.Name))
                            continue;

                        var absoluteFilePathString = $"/{localRepo}/{entry.FullName}";

                        await CommonApi.FileSystemProviderApi.File
                            .WriteAllTextAsync(
                                absoluteFilePathString,
                                contents,
                                _activeCancellationToken.Value)
                            .ConfigureAwait(false);

                        if (entry.Name.EndsWith(ExtensionNoPeriodFacts.DOT_NET_SOLUTION))
                            PromptUserOpenSolution(absoluteFilePathString);
                    }

                    // UI progress indicator
                    {
                        _processedFilesInZipArchive++;
                        _nameOfEntryMostRecentlyProcessed = entry.Name;
                        await InvokeAsync(StateHasChanged);
                    }
                }
            }
            else
            {
                throw new LuthetusIdeException(response.ToString());
            }
        }
        catch (Exception exception)
        {
            _errorMessage = exception.ToString();

            // UI progress indicator
            {
                _activePhase = ImportPhase.Error;
                await InvokeAsync(StateHasChanged);
            }
        }
        finally
        {
            if (_activePhase != ImportPhase.Error)
            {
                // UI progress indicator
                {
                    _activePhase = ImportPhase.Finished;
                    _parametersForFinishedQuery = $"({localOwner}/{localRepo})";
                    await InvokeAsync(StateHasChanged);
                }
            }

            _activeCancellationToken = null;
        }
    }

    private void CancelOnClick()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new();
    }

    private void PromptUserOpenSolution(string absolutePathString)
    {
    	/*
		//// Am moving .NET code out so the IDE is language agnostic. (2024-07-15)
		// =======================================================================
        var absolutePath = EnvironmentProvider.AbsolutePathFactory(
            absolutePathString,
            false);

        var notificationRecord = new NotificationViewModel(
            Key<IDynamicViewModel>.NewKey(),
            "A .NET Solution was found",
            typeof(IdePromptOpenSolutionDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(IdePromptOpenSolutionDisplay.AbsolutePath),
                    absolutePath
                }
            },
            TimeSpan.FromSeconds(7),
            false,
            null);

        Dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));
		*/
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }
}