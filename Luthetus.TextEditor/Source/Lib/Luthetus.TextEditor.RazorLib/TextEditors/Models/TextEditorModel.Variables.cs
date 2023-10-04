using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public partial class TextEditorModel
{
    public const int TAB_WIDTH = 4;
    public const int GUTTER_PADDING_LEFT_IN_PIXELS = 5;
    public const int GUTTER_PADDING_RIGHT_IN_PIXELS = 15;
    public const int MAXIMUM_EDIT_BLOCKS = 10;
    public const int MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN = 5;

    private readonly List<RichCharacter> _contentBag = new();
    private readonly List<EditBlock> _editBlocksPersistedBag = new();
    private readonly List<(RowEndingKind rowEndingKind, int count)> _rowEndingKindCountsBag = new();
    /// <summary>To get the ending position of RowIndex _rowEndingPositions[RowIndex]<br /><br />_rowEndingPositions returns the start of the NEXT row</summary>
    private readonly List<(int positionIndex, RowEndingKind rowEndingKind)> _rowEndingPositionsBag = new();
    /// <summary>Provides exact position index of a tab character</summary>
    private readonly List<int> _tabKeyPositionsBag = new();
    private readonly List<TextEditorPresentationModel> _presentationModelsBag = new();

    public int RowCount => _rowEndingPositionsBag.Count;
    public int DocumentLength => _contentBag.Count;
    public ImmutableArray<EditBlock> EditBlocksBag => _editBlocksPersistedBag.ToImmutableArray();
    public ImmutableArray<(int positionIndex, RowEndingKind rowEndingKind)> RowEndingPositionsBag => _rowEndingPositionsBag.ToImmutableArray();
    public ImmutableArray<(RowEndingKind rowEndingKind, int count)> RowEndingKindCountsBag => _rowEndingKindCountsBag.ToImmutableArray();
    public ImmutableArray<TextEditorPresentationModel> PresentationModelsBag => _presentationModelsBag.ToImmutableArray();

    /// <summary>If there is a mixture of<br />-Carriage Return<br />-Linefeed<br />-CRLF<br />Then <see cref="OnlyRowEndingKind" /> will be null.<br /><br />If there are no line endingsthen <see cref="OnlyRowEndingKind" /> will be null.</summary>
    public RowEndingKind? OnlyRowEndingKind { get; private set; }
    public RowEndingKind UsingRowEndingKind { get; private set; }
    /// <summary>TODO: On (2023-10-02) Key&lt;TextEditorModel&gt; was removed, because it felt redundant given only 1 <see cref="TextEditorModel"/> can exist for a given <see cref="ResourceUri"/>. This change however creates an issue regarding 'fake' resource uri's that are used for in-memory files. For example, <see cref="Luthetus.TextEditor.RazorLib.Options.Displays.TextEditorSettingsPreview"/> now has the resource URI of "__LUTHETUS_SETTINGS_PREVIEW__". This is an issue because could a user have on their filesystem the file "__LUTHETUS_SETTINGS_PREVIEW__"? (at that exact resource uri)</summary>
    public ResourceUri ResourceUri { get; private set; }
    public DateTime ResourceLastWriteTime { get; private set; }
    /// <summary><see cref="FileExtension"/> is displayed as is within the<see cref="TextEditorFooter"/>.<br/><br/>The <see cref="TextEditorFooter"/> is only displayed if<see cref="TextEditorViewModelDisplay.IncludeFooterHelperComponent"/> is set to true.</summary>
    public string FileExtension { get; private set; }
    public IDecorationMapper DecorationMapper { get; private set; }
    public ICompilerService CompilerService { get; private set; }
    public int EditBlockIndex { get; private set; }
    public (int rowIndex, int rowLength) MostCharactersOnASingleRowTuple { get; private set; }

    public Key<RenderState> RenderStateKey { get; } = Key<RenderState>.NewKey();
    public Keymap TextEditorKeymap { get; }
    public TextEditorSaveFileHelper TextEditorSaveFileHelper { get; } = new();
    public TextEditorOptions? TextEditorOptions { get; }
}