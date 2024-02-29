using Luthetus.TextEditor.RazorLib.Rows.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels.Internals;

public partial class TextEditorModelModifier
{
    internal class ShiftRowsOutput
    {
        public ShiftRowsOutput(
            int relativeRowEndingIndex,
            ImmutableList<RowEnding> rowEndingList,
            ImmutableList<(RowEndingKind rowEndingKind, int count)> rowEndingKindCountList,
            RowEndingKind? onlyRowEndingKind)
        {
            RelativeRowIndex = relativeRowEndingIndex;
            RowEndingList = rowEndingList;
            RowEndingKindCountList = rowEndingKindCountList;
            OnlyRowEndingKind = onlyRowEndingKind;
        }

        public int RelativeRowIndex { get; }
        public ImmutableList<RowEnding> RowEndingList { get; }
        public ImmutableList<(RowEndingKind rowEndingKind, int count)> RowEndingKindCountList { get; }
        public RowEndingKind? OnlyRowEndingKind { get; }
    }
}