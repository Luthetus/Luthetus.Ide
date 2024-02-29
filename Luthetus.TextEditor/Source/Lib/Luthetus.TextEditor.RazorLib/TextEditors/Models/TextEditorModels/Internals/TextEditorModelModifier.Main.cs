using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels.Internals;

/// <inheritdoc cref="ITextEditorModel"/>
public partial class TextEditorModelModifier : ITextEditorModel
{
    public const int EXPANSION_FACTOR = 3; // When a partition runs out space its content is divided amongst some amount of partitions. If expansion factor is 3, then when a partition expands, it will insert 2 addition partitions after itself. Then the original partition splits its content into thirds. And distributes it across itself, and the other 2 newly inserted partitions.

    public TextEditorModelModifier(TextEditorModel model)
    {
        Content = model.Content;
        EditBlockList = model.EditBlockList.ToList();
        PresentationModelList = model.PresentationModelList.ToList();
        UsingRowEndingKind = model.UsingRowEndingKind;
        ResourceUri = model.ResourceUri;
        ResourceLastWriteTime = model.ResourceLastWriteTime;
        FileExtension = model.FileExtension;
        DecorationMapper = model.DecorationMapper;
        CompilerService = model.CompilerService;
        TextEditorSaveFileHelper = model.TextEditorSaveFileHelper;
        EditBlockIndex = model.EditBlockIndex;
        MostCharactersOnASingleRowTuple = model.MostCharactersOnASingleRowTuple;
    }

    public ITextEditorContent Content { get; set; }
    public List<EditBlock> EditBlockList { get; set; }
    public List<TextEditorPresentationModel> PresentationModelList { get; set; }
    public RowEndingKind UsingRowEndingKind { get; set; }
    public ResourceUri ResourceUri { get; set; }
    public DateTime ResourceLastWriteTime { get; set; }
    public string FileExtension { get; set; }
    public IDecorationMapper DecorationMapper { get; set; }
    public ILuthCompilerService CompilerService { get; set; }
    public TextEditorSaveFileHelper TextEditorSaveFileHelper { get; set; }
    public int EditBlockIndex { get; set; }
    public (int rowIndex, int rowLength) MostCharactersOnASingleRowTuple { get; set; }
    public Key<RenderState> RenderStateKey { get; set; } = Key<RenderState>.NewKey();

    public bool WasModified { get; internal set; }

    public TextEditorModel ToModel()
    {
        return new TextEditorModel(
            Content,
            EditBlockList,
            PresentationModelList,
            UsingRowEndingKind,
            ResourceUri,
            ResourceLastWriteTime,
            FileExtension,
            DecorationMapper,
            CompilerService,
            TextEditorSaveFileHelper,
            EditBlockIndex,
            MostCharactersOnASingleRowTuple,
            RenderStateKey);
    }

    internal void DeleteByRange(int removeCharacterCount, TextEditorCursorModifierBag textEditorCursorModifierBag, CancellationToken none)
    {
        throw new NotImplementedException();
    }

    internal void EditByInsertion(string clipboard, TextEditorCursorModifierBag cursorModifierBag, CancellationToken none)
    {
        throw new NotImplementedException();
    }

    internal void HandleKeyboardEvent(KeyboardEventArgs keyboardEventArgs, TextEditorCursorModifierBag cursorModifierBag, CancellationToken none)
    {
        throw new NotImplementedException();
    }
}