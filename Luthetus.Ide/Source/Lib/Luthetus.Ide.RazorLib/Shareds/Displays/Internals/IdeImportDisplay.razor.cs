using Microsoft.AspNetCore.Components;
using System.IO.Compression;

namespace Luthetus.Ide.RazorLib.Shareds.Displays.Internals;

public partial class IdeImportDisplay : ComponentBase, IDisposable
{
    [Inject]
    private HttpClient HttpClient { get; set; } = null!;

    private readonly object RequestRepoContentLock = new();

    private string _owner = "Luthetus";
    private string _repo = "Luthetus.Website";
    private string _ref = string.Empty;

    private string _text = string.Empty;
    private ImportPhase _activePhase = ImportPhase.None;
    private string _activeQuery = string.Empty;
    private int _totalFilesInZipArchive;
    private int _processedFilesInZipArchive;
    private string? _errorMessage;

    private CancellationTokenSource _cancellationTokenSource = new();
    private CancellationToken? _activeCancellationToken = null;

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
        if (_activeCancellationToken is not null)
            return;

        lock (RequestRepoContentLock)
        {
            if (_activeCancellationToken is not null)
                return;

            _activeCancellationToken = _cancellationTokenSource.Token;
            _activeQuery = $"https://api.github.com/repos/{_owner}/{_repo}/zipball/{_ref}";
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
                $"https://api.github.com/repos/{_owner}/{_repo}/zipball/{_ref}");

            request.Headers.Add("Accept", "application/vnd.github+json");
            request.Headers.Add("User-Agent", "Luthetus");
            // request.Headers.Add("Authorization", 222"Bearer <YOUR-TOKEN>");
            request.Headers.Add("X-GitHub-Api-Version", "2022-11-28");
            request.Headers.Add("owner", _owner);
            request.Headers.Add("repo", _repo);
            request.Headers.Add("ref", _ref);

            var response = await HttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
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
                    var aaa = await streamReader.ReadToEndAsync();

                    // UI progress indicator
                    {
                        _processedFilesInZipArchive++;
                        await InvokeAsync(StateHasChanged);
                    }
                }
            }
            else
            {
                _errorMessage = response.ToString();
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
            _activeCancellationToken = null;

            if (_activePhase != ImportPhase.Error)
            {
                // UI progress indicator
                {
                    _activePhase = ImportPhase.Finished;
                    await InvokeAsync(StateHasChanged);
                }
            }
        }
    }

    private void CancelOnClick()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new();
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }
}