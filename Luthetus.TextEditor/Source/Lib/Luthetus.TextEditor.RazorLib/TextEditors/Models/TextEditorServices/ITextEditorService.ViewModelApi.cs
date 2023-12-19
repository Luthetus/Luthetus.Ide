using Microsoft.JSInterop;
using Fluxor;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keyboards.Models;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using static Luthetus.TextEditor.RazorLib.Commands.Models.TextEditorCommand;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

public partial interface ITextEditorService
{
    public interface ITextEditorViewModelApi
    {
        #region CREATE_METHODS
        public void Register(Key<TextEditorViewModel> textEditorViewModelKey, ResourceUri resourceUri);
        #endregion

        #region READ_METHODS
        /// <summary>
        /// One should store the result of invoking this method in a variable, then reference that variable.
        /// If one continually invokes this, there is no guarantee that the data had not changed
        /// since the previous invocation.
        /// </summary>
        public ImmutableList<TextEditorViewModel> GetViewModels();
        public TextEditorModel? GetModelOrDefault(Key<TextEditorViewModel> viewModelKey);
        public Task<TextEditorMeasurements> GetTextEditorMeasurementsAsync(string elementId);

        public Task<CharAndRowMeasurements> MeasureCharacterWidthAndRowHeightAsync(
            string measureCharacterWidthAndRowHeightElementId,
            int countOfTestCharacters);

        public TextEditorViewModel? GetOrDefault(Key<TextEditorViewModel> viewModelKey);
        public string? GetAllText(Key<TextEditorViewModel> viewModelKey);

        public void SetCursorShouldBlink(bool cursorShouldBlink);
        #endregion

        #region DELETE_METHODS
        public void Dispose(Key<TextEditorViewModel> viewModelKey);
        #endregion

        public bool CursorShouldBlink { get; }
        public event Action? CursorShouldBlinkChanged;
    }

    public class TextEditorViewModelApi : ITextEditorViewModelApi
    {
        private readonly ITextEditorService _textEditorService;
        private readonly IBackgroundTaskService _backgroundTaskService;
        private readonly IState<TextEditorViewModelState> _viewModelStateWrap;
        private readonly IState<TextEditorModelState> _modelStateWrap;
        private readonly IDispatcher _dispatcher;

        // TODO: Perhaps do not reference IJSRuntime but instead wrap it in a 'IUiProvider' or something like that. The 'IUiProvider' would then expose methods that allow the TextEditorViewModel to adjust the scrollbars. 
        private readonly IJSRuntime _jsRuntime;

        public TextEditorViewModelApi(
            ITextEditorService textEditorService,
            IBackgroundTaskService backgroundTaskService,
            IState<TextEditorViewModelState> viewModelStateWrap,
            IState<TextEditorModelState> modelStateWrap,
            IJSRuntime jsRuntime,
            IDispatcher dispatcher)
        {
            _textEditorService = textEditorService;
            _backgroundTaskService = backgroundTaskService;
            _viewModelStateWrap = viewModelStateWrap;
            _modelStateWrap = modelStateWrap;
            _jsRuntime = jsRuntime;
            _dispatcher = dispatcher;
        }

        private Task _cursorShouldBlinkTask = Task.CompletedTask;
        private CancellationTokenSource _cursorShouldBlinkCancellationTokenSource = new();
        private TimeSpan _blinkingCursorTaskDelay = TimeSpan.FromMilliseconds(1000);

        public bool CursorShouldBlink { get; private set; } = true;
        public event Action? CursorShouldBlinkChanged;

        public void SetCursorShouldBlink(bool cursorShouldBlink)
        {
            if (!cursorShouldBlink)
            {
                if (CursorShouldBlink)
                {
                    // Change true -> false THEREFORE: notify subscribers
                    CursorShouldBlink = cursorShouldBlink;
                    CursorShouldBlinkChanged?.Invoke();
                }

                // Single Threaded Applications flicker every "_blinkingCursorTaskDelay" event while holding a key down if this line is not included
                _cursorShouldBlinkCancellationTokenSource.Cancel();

                if (_cursorShouldBlinkTask.IsCompleted)
                {
                    // Considering that just before entering this if block we cancel the cancellation token source. I want to ensure we get a new one if a new Task session beings.
                    _cursorShouldBlinkCancellationTokenSource = new();

                    _cursorShouldBlinkTask = Task.Run(async () =>
                    {
                        while (true)
                        {
                            try
                            {
                                var cancellationToken = _cursorShouldBlinkCancellationTokenSource.Token;

                                await Task
                                    .Delay(_blinkingCursorTaskDelay, cancellationToken)
                                    .ConfigureAwait(false);

                                // Change false -> true THEREFORE: notify subscribers
                                CursorShouldBlink = true;
                                CursorShouldBlinkChanged?.Invoke();
                                break;
                            }
                            catch (TaskCanceledException)
                            {
                                // Single Threaded Applications cannot exit the while loop unless they cancel the token themselves.
                                _cursorShouldBlinkCancellationTokenSource.Cancel();
                                _cursorShouldBlinkCancellationTokenSource = new();
                            }
                        }
                    });
                }
            }
        }

        #region CREATE_METHODS
        public void Register(Key<TextEditorViewModel> textEditorViewModelKey, ResourceUri resourceUri)
        {
            _dispatcher.Dispatch(new TextEditorViewModelState.RegisterAction(
                textEditorViewModelKey,
                resourceUri,
                _textEditorService));
        }
        #endregion

        #region READ_METHODS
        public TextEditorViewModel? GetOrDefault(Key<TextEditorViewModel> textEditorViewModelKey)
        {
            return _textEditorService.ViewModelStateWrap.Value.ViewModelBag.FirstOrDefault(
                x => x.ViewModelKey == textEditorViewModelKey);
        }

        public ImmutableList<TextEditorViewModel> GetViewModels()
        {
            return _textEditorService.ViewModelStateWrap.Value.ViewModelBag;
        }

        public TextEditorModel? GetModelOrDefault(Key<TextEditorViewModel> textEditorViewModelKey)
        {
            var viewModelState = _textEditorService.ViewModelStateWrap.Value;

            var viewModel = viewModelState.ViewModelBag.FirstOrDefault(
                x => x.ViewModelKey == textEditorViewModelKey);

            if (viewModel is null)
                return null;

            return _textEditorService.ModelApi.GetOrDefault(viewModel.ResourceUri);
        }

        public string? GetAllText(Key<TextEditorViewModel> textEditorViewModelKey)
        {
            var textEditorModel = GetModelOrDefault(textEditorViewModelKey);

            return textEditorModel is null
                ? null
                : _textEditorService.ModelApi.GetAllText(textEditorModel.ResourceUri);
        }

        public async Task<TextEditorMeasurements> GetTextEditorMeasurementsAsync(string elementId)
        {
            return await _jsRuntime.InvokeAsync<TextEditorMeasurements>("luthetusTextEditor.getTextEditorMeasurementsInPixelsById",
                elementId);
        }

        public async Task<CharAndRowMeasurements> MeasureCharacterWidthAndRowHeightAsync(
            string measureCharacterWidthAndRowHeightElementId,
            int countOfTestCharacters)
        {
            return await _jsRuntime.InvokeAsync<CharAndRowMeasurements>("luthetusTextEditor.getCharAndRowMeasurementsInPixelsById",
                measureCharacterWidthAndRowHeightElementId,
                countOfTestCharacters);
        }
        #endregion

        #region DELETE_METHODS
        public void Dispose(Key<TextEditorViewModel> textEditorViewModelKey)
        {
            _dispatcher.Dispatch(new TextEditorViewModelState.DisposeAction(textEditorViewModelKey));
        }
        #endregion
    }
}