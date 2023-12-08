using Luthetus.TextEditor.RazorLib.Keymaps.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>
/// Stores the <see cref="RichCharacter"/> class instances that represent the text.<br/><br/>
/// Each TextEditorModel has a unique underlying resource uri.<br/><br/>
/// Therefore, if one has a text file named "myHomework.txt", then only one TextEditorModel
/// can exist with the resource uri of "myHomework.txt".
/// </summary>
public partial record TextEditorModel
{
    public TextEditorModel(
        ResourceUri resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string content,
        IDecorationMapper? decorationMapper,
        ICompilerService? compilerService,
        Keymap? textEditorKeymap)
    {
        ResourceUri = resourceUri;
        ResourceLastWriteTime = resourceLastWriteTime;
        FileExtension = fileExtension;
        DecorationMapper = decorationMapper ?? new TextEditorDecorationMapperDefault();
        CompilerService = compilerService ?? new TextEditorDefaultCompilerService();
        TextEditorKeymap = textEditorKeymap ?? new TextEditorKeymapDefault();

		var contentBag = new List<RichCharacter>();
		var rowEndingKindCountsBag = new List<(RowEndingKind rowEndingKind, int count)>();
		var rowEndingPositionsBag = new List<(int positionIndex, RowEndingKind rowEndingKind)>();
		var tabKeyPositionsBag = new List<int>();
		var onlyRowEndingKind = (RowEndingKind?)null;
		var usingRowEndingKind = RowEndingKind.Unset;
		(int rowIndex, int rowLength) mostCharactersOnASingleRowTuple = (0, 0);

		/////////////////////////

		var rowIndex = 0;
		var previousCharacter = '\0';

		var charactersOnRow = 0;

		var carriageReturnCount = 0;
		var linefeedCount = 0;
		var carriageReturnLinefeedCount = 0;

		for (var index = 0; index < content.Length; index++)
		{
			var character = content[index];

			charactersOnRow++;

			if (character == KeyboardKeyFacts.WhitespaceCharacters.CARRIAGE_RETURN)
			{
				if (charactersOnRow > mostCharactersOnASingleRowTuple.rowLength - MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN)
					mostCharactersOnASingleRowTuple = (rowIndex, charactersOnRow + MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

				rowEndingPositionsBag.Add((index + 1, RowEndingKind.CarriageReturn));
				rowIndex++;

				charactersOnRow = 0;

				carriageReturnCount++;
			}
			else if (character == KeyboardKeyFacts.WhitespaceCharacters.NEW_LINE)
			{
				if (charactersOnRow > mostCharactersOnASingleRowTuple.rowLength - MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN)
					mostCharactersOnASingleRowTuple = (rowIndex, charactersOnRow + MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

				if (previousCharacter == KeyboardKeyFacts.WhitespaceCharacters.CARRIAGE_RETURN)
				{
					var lineEnding = rowEndingPositionsBag[rowIndex - 1];

					rowEndingPositionsBag[rowIndex - 1] = (lineEnding.positionIndex + 1, RowEndingKind.CarriageReturnLinefeed);

					carriageReturnCount--;
					carriageReturnLinefeedCount++;
				}
				else
				{
					rowEndingPositionsBag.Add((index + 1, RowEndingKind.Linefeed));
					rowIndex++;

					linefeedCount++;
				}

				charactersOnRow = 0;
			}

			if (character == KeyboardKeyFacts.WhitespaceCharacters.TAB)
				tabKeyPositionsBag.Add(index);

			previousCharacter = character;

			contentBag.Add(new RichCharacter
			{
				Value = character,
				DecorationByte = default,
			});
		}

		rowEndingKindCountsBag.AddRange(new List<(RowEndingKind rowEndingKind, int count)>
		{
			(RowEndingKind.CarriageReturn, carriageReturnCount),
			(RowEndingKind.Linefeed, linefeedCount),
			(RowEndingKind.CarriageReturnLinefeed, carriageReturnLinefeedCount),
		});

		var setUsingRowEndingKind = true;

		var existingRowEndingsBag = rowEndingKindCountsBag
			.Where(x => x.count > 0)
			.ToArray();

		if (!existingRowEndingsBag.Any())
		{
			onlyRowEndingKind = RowEndingKind.Unset;
			usingRowEndingKind = RowEndingKind.Linefeed;
		}
		else
		{
			if (existingRowEndingsBag.Length == 1)
			{
				var rowEndingKind = existingRowEndingsBag.Single().rowEndingKind;

				if (setUsingRowEndingKind)
					usingRowEndingKind = rowEndingKind;

				onlyRowEndingKind = rowEndingKind;
			}
			else
			{
				if (setUsingRowEndingKind)
					usingRowEndingKind = existingRowEndingsBag.MaxBy(x => x.count).rowEndingKind;

				onlyRowEndingKind = null;
			}
		}

		rowEndingPositionsBag.Add((content.Length, RowEndingKind.EndOfFile));

		ContentBag = contentBag.ToImmutableList();
		RowEndingKindCountsBag = rowEndingKindCountsBag.ToImmutableList();
		RowEndingPositionsBag = rowEndingPositionsBag.ToImmutableList();
		TabKeyPositionsBag = tabKeyPositionsBag.ToImmutableList();
		OnlyRowEndingKind = onlyRowEndingKind;
		UsingRowEndingKind = usingRowEndingKind;
		MostCharactersOnASingleRowTuple = mostCharactersOnASingleRowTuple;
	}
}