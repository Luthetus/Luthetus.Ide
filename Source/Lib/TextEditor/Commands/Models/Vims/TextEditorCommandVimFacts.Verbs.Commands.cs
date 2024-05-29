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

                return commandArgs.TextEditorService.PostSimpleBatch(
                    nameof(DeleteLineCommand),
                    DeleteLineFactory(commandArgs));
            })
        {
            TextEditorEditFactory = interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                return DeleteLineFactory(commandArgs);
            }
        };

        public static readonly TextEditorCommand ChangeLineCommand = new(
            "Vim::Change(Line)", "Vim::Change(Line)", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                return commandArgs.TextEditorService.PostSimpleBatch(
                    nameof(ChangeLineCommand),
                    ChangeLineFactory(commandArgs));
            })
        {
            TextEditorEditFactory = interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                return ChangeLineFactory(commandArgs);
            }
        };

        public static TextEditorCommand DeleteMotionCommandConstructor(TextEditorCommand innerTextEditorCommand) => new(
            $"Vim::Delete({innerTextEditorCommand.DisplayName})", $"Vim::Delete({innerTextEditorCommand.DisplayName})", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.InnerCommand = innerTextEditorCommand;

                return commandArgs.TextEditorService.PostSimpleBatch(
                    nameof(DeleteMotionCommandConstructor),
                    DeleteMotionFactory(commandArgs));
            })
        {
            TextEditorEditFactory = interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.InnerCommand = innerTextEditorCommand;

                return DeleteMotionFactory(commandArgs);
            }
        };

        public static TextEditorCommand ChangeMotionCommandConstructor(TextEditorCommand innerTextEditorCommand) => new(
            $"Vim::Change({innerTextEditorCommand.DisplayName})", $"Vim::Change({innerTextEditorCommand.DisplayName})", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.InnerCommand = innerTextEditorCommand;

                return commandArgs.TextEditorService.PostSimpleBatch(
                    nameof(ChangeMotionCommandConstructor),
                    GetChangeMotionFactory(commandArgs));
            })
        {
            TextEditorEditFactory = interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                
                commandArgs.InnerCommand = innerTextEditorCommand;

                return GetChangeMotionFactory(commandArgs);
            }
        };

        public static readonly TextEditorCommand ChangeSelectionCommand = new(
            "Vim::Change(Selection)", "Vim::Change(Selection)", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                return commandArgs.TextEditorService.PostSimpleBatch(
                    nameof(ChangeSelectionCommand),
                    ChangeSelectionFactory(commandArgs));
            })
        {
            TextEditorEditFactory = interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                return ChangeSelectionFactory(commandArgs);
            }
        };

        public static readonly TextEditorCommand YankCommand = new(
            "Vim::Change(Selection)", "Vim::Change(Selection)", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                return commandArgs.TextEditorService.PostSimpleBatch(
                    nameof(YankCommand),
                    YankFactory(commandArgs));
            })
        {
            TextEditorEditFactory = interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                return YankFactory(commandArgs);
            }
        };

        public static readonly TextEditorCommand NewLineBelowCommand = new(
            "Vim::NewLineBelow()", "Vim::NewLineBelow()", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                return commandArgs.TextEditorService.PostSimpleBatch(
                    nameof(NewLineBelowCommand),
                    NewLineBelowFactory(commandArgs));
            })
        {
            TextEditorEditFactory = interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                return NewLineBelowFactory(commandArgs);
            }
        };

        public static readonly TextEditorCommand NewLineAboveCommand = new(
            "Vim::NewLineAbove()", "Vim::NewLineAbove()", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                return commandArgs.TextEditorService.PostSimpleBatch(
                    nameof(NewLineAboveCommand),
                    NewLineAboveFactory(commandArgs));
            })
        {
            TextEditorEditFactory = interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                return NewLineAboveFactory(commandArgs);
            }
        };
    }
}