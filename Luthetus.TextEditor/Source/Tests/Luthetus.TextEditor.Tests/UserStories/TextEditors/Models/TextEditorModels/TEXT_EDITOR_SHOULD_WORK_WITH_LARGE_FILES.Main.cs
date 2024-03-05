using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.Tests.UserStories.TextEditors.Models.TextEditorModels;

public partial class TEXT_EDITOR_SHOULD_WORK_WITH_LARGE_FILES
{
    [Fact]
    public void Construct_Model_With_Large_File()
    {
        var fileExtension = ExtensionNoPeriodFacts.TXT;
        var resourceUri = new ResourceUri("/unitTesting.txt");
        var resourceLastWriteTime = DateTime.UtcNow;

        var model = new TextEditorModel(
             resourceUri,
             resourceLastWriteTime,
             fileExtension,
             LARGE_FILE,
             null,
             null);

        throw new NotImplementedException();
    }

    //[Fact]
    //public async Task THROTTLE_CONTROLLER()
    //{
    //    var fileExtension = ExtensionNoPeriodFacts.TXT;
    //    var resourceUri = new ResourceUri("/unitTesting.txt");
    //    var resourceLastWriteTime = DateTime.UtcNow;

    //    var model = new TextEditorModel(
    //         resourceUri,
    //         resourceLastWriteTime,
    //         fileExtension,
    //         LARGE_FILE,
    //         null,
    //         null);

    //    var textEditorComponent = new TextEditorComponent();
    //    var throttleController = new ThrottleController();

    //    for (int i = 0; i < 3; i++)
    //    {
    //        throttleController.EnqueueEvent(new ThrottleEvent<List<char>>(
    //            0,
    //            TimeSpan.FromMilliseconds(50),
    //            new() { 'a' },
    //            (throttleEvent, throttleCancellationToken) => TypeCharacterFunc(throttleEvent, textEditorComponent, throttleCancellationToken),
    //            tuple => BatchTypeCharacterFunc(tuple, textEditorComponent)));
    //    }

    //    await throttleController.StopAsync();

    //    // If non-batch insertion of a character, insert 'a'
    //    //
    //    // else if batch insertion, insert the first character as 'c', then the remainder as 'b'
    //    throw new NotImplementedException();
    //}
    
    //[Fact]
    //public async Task THROTTLE_CONTROLLER_FOR_3()
    //{
    //    var textEditorComponent = new TextEditorComponent();
    //    var throttleController = new ThrottleController();

    //    for (int i = 0; i < 3; i++)
    //    {
    //        throttleController.EnqueueEvent(new ThrottleEvent<List<char>>(
    //            0,
    //            TimeSpan.FromMilliseconds(50),
    //            new() { 'a' },
    //            (throttleEvent, throttleCancellationToken) => TypeCharacterFunc(throttleEvent, textEditorComponent, throttleCancellationToken),
    //            tuple => BatchTypeCharacterFunc(tuple, textEditorComponent)));
    //    }

    //    await throttleController.StopAsync();

    //    // If non-batch insertion of a character, insert 'a'
    //    //
    //    // else if batch insertion, insert the first character as 'c', then the remainder as 'b'

    //    if ("cbb" != textEditorComponent.Text)
    //    {
    //        var aaa = 2;
    //    }

    //    Assert.Equal("cbb", textEditorComponent.Text);
    //}

    //private string _typeCharacterId = "Type a character";

    //private Task TypeCharacterFunc(IThrottleEvent throttleEvent, TextEditorComponent textEditorComponent, CancellationToken cancellationToken)
    //{
    //    if (throttleEvent is not ThrottleEvent<List<char>> charEvent)
    //        return Task.CompletedTask;

    //    foreach (var character in charEvent.Item)
    //    {
    //        textEditorComponent.WriteCharacter(character);
    //    }

    //    return Task.CompletedTask;
    //}
    
    //private IThrottleEvent? BatchTypeCharacterFunc((IThrottleEvent oldEvent, IThrottleEvent recentEvent) tuple, TextEditorComponent textEditorComponent)
    //{
    //    if (tuple.oldEvent is ThrottleEvent<List<char>> oldCharEvent &&
    //        tuple.recentEvent is ThrottleEvent<List<char>> recentCharEvent)
    //    {
    //        var changedText = new List<char>();
            
    //        changedText.AddRange(oldCharEvent.Item.Select((x, i) => 
    //        {
    //            return i == 0 ? 'c' : 'b';
    //        }));

    //        changedText.AddRange(recentCharEvent.Item.Select(x => 'b'));

    //        return new ThrottleEvent<List<char>>(
    //            0,
    //            TimeSpan.FromMilliseconds(1_000),
    //            changedText,
    //            (throttleEvent, throttleCancellationToken) => TypeCharacterFunc(throttleEvent, textEditorComponent, throttleCancellationToken),
    //            tuple => BatchTypeCharacterFunc(tuple, textEditorComponent));
    //    }
    //    else
    //    {
    //        return null;
    //    }
    //}

    //private class TextEditorComponent
    //{
    //    private readonly StringBuilder _textBuilder = new();

    //    public string Text => _textBuilder.ToString();

    //    public void WriteCharacter(char character)
    //    {
    //        _textBuilder.Append(character);
    //    }
    //}
}
