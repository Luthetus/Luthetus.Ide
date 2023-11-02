using Luthetus.TextEditor.RazorLib.Keymaps.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Common.RazorLib.Keymaps.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>Stores the <see cref="RichCharacter"/> class instances that represent the text.<br/><br/>Each TextEditorModel has a unique underlying resource uri.<br/><br/>Therefore, if one has a text file named "myHomework.txt", then only one TextEditorModel can exist with the resource uri of "myHomework.txt".</summary>
public partial class TextEditorModel
{
    public TextEditorModel(
        ResourceUri resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string content,
        IDecorationMapper? decorationMapper,
        ICompilerService? compilerService,
        Keymap? textEditorKeymap,
        TextEditorSaveFileHelper textEditorSaveFileHelper)
    {
        ResourceUri = resourceUri;
        ResourceLastWriteTime = resourceLastWriteTime;
        FileExtension = fileExtension;
        DecorationMapper = decorationMapper ?? new TextEditorDecorationMapperDefault();
        CompilerService = compilerService ?? new TextEditorDefaultCompilerService();
        TextEditorKeymap = textEditorKeymap ?? new TextEditorKeymapDefault();
        TextEditorSaveFileHelper = textEditorSaveFileHelper;

        SetContent(content);
    }

    /// <summary>Clone the TextEditorModel using shallow copy so that Fluxor will notify all the Blazor Components of the <see cref="TextEditorModel"/> having been replaced<br/><br/>Do not use a record would that do a deep value comparison and be incredibly slow? (i.e.) compare every RichCharacter in the list.</summary>
    public TextEditorModel(TextEditorModel original)
    {
        ResourceUri = original.ResourceUri;
        ResourceLastWriteTime = original.ResourceLastWriteTime;
        FileExtension = original.FileExtension;
        _contentBag = original._contentBag;
        _editBlocksPersistedBag = original._editBlocksPersistedBag;
        _rowEndingKindCountsBag = original._rowEndingKindCountsBag;
        _rowEndingPositionsBag = original._rowEndingPositionsBag;
        _tabKeyPositionsBag = original._tabKeyPositionsBag;
        _presentationModelsBag = original._presentationModelsBag;

        OnlyRowEndingKind = original.OnlyRowEndingKind;
        UsingRowEndingKind = original.UsingRowEndingKind;
        DecorationMapper = original.DecorationMapper;
        CompilerService = original.CompilerService;
        TextEditorKeymap = original.TextEditorKeymap;
        TextEditorSaveFileHelper = original.TextEditorSaveFileHelper;
        EditBlockIndex = original.EditBlockIndex;
        MostCharactersOnASingleRowTuple = original.MostCharactersOnASingleRowTuple;
        TextEditorOptions = original.TextEditorOptions;
    }
}