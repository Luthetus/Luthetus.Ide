using Luthetus.TextEditor.RazorLib.Edits.Models;

namespace Luthetus.TextEditor.RazorLib.Commands.Models.Vims;

public static partial class TextEditorCommandVimFacts
{
    public static partial class Verbs
    {
        public static TextEditorCommand DeleteLineCommand = new(
            "Vim::Delete(Line)", "Vim::Delete(Line)", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                DeleteLine(commandArgs.EditContext);
            });

        public static readonly TextEditorCommand ChangeLineCommand = new(
            "Vim::Change(Line)", "Vim::Change(Line)", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                ChangeLine(commandArgs.EditContext);
            });

        public static TextEditorCommand DeleteMotionCommandConstructor(TextEditorCommand innerTextEditorCommand) => new(
            $"Vim::Delete({innerTextEditorCommand.DisplayName})", $"Vim::Delete({innerTextEditorCommand.DisplayName})", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                commandArgs.InnerCommand = innerTextEditorCommand;
                DeleteMotion(commandArgs.EditContext);
            });

        public static TextEditorCommand ChangeMotionCommandConstructor(TextEditorCommand innerTextEditorCommand) => new(
            $"Vim::Change({innerTextEditorCommand.DisplayName})", $"Vim::Change({innerTextEditorCommand.DisplayName})", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                commandArgs.InnerCommand = innerTextEditorCommand;
				GetChangeMotion(commandArgs.EditContext);
            });

        public static readonly TextEditorCommand ChangeSelectionCommand = new(
            "Vim::Change(Selection)", "Vim::Change(Selection)", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
				ChangeSelection(commandArgs.EditContext);
            });

        public static readonly TextEditorCommand YankCommand = new(
            "Vim::Yank(Selection)", "Vim::Yank(Selection)", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
				Yank(commandArgs.EditContext);
            });

        public static readonly TextEditorCommand NewLineBelowCommand = new(
            "Vim::NewLineBelow()", "Vim::NewLineBelow()", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                NewLineBelow(commandArgs.EditContext);
            });

        public static readonly TextEditorCommand NewLineAboveCommand = new(
            "Vim::NewLineAbove()", "Vim::NewLineAbove()", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
				NewLineAbove(commandArgs.EditContext);
            });
    }
}