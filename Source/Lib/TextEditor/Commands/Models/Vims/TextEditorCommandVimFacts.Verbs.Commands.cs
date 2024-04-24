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

                commandArgs.TextEditorService.PostIndependent(
                    nameof(DeleteLineCommand),
                    commandArgs.Events,
                    commandArgs.ViewModelKey,
                    DeleteLineFactory(commandArgs));

                return Task.CompletedTask;
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

                commandArgs.TextEditorService.PostIndependent(
                    nameof(ChangeLineCommand),
                    commandArgs.Events,
                    commandArgs.ViewModelKey,
                    ChangeLineFactory(commandArgs));

                return Task.CompletedTask;
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

                commandArgs.TextEditorService.PostIndependent(
                    nameof(DeleteMotionCommandConstructor),
                    commandArgs.Events,
                    commandArgs.ViewModelKey,
                    DeleteMotionFactory(commandArgs));

                return Task.CompletedTask;
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

                commandArgs.TextEditorService.PostIndependent(
                    nameof(ChangeMotionCommandConstructor),
                    commandArgs.Events,
                    commandArgs.ViewModelKey,
                    GetChangeMotionFactory(commandArgs));

                return Task.CompletedTask;
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

                commandArgs.TextEditorService.PostIndependent(
                    nameof(ChangeSelectionCommand),
                    commandArgs.Events,
                    commandArgs.ViewModelKey,
                    ChangeSelectionFactory(commandArgs));

                return Task.CompletedTask;
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

                commandArgs.TextEditorService.PostIndependent(
                    nameof(YankCommand),
                    commandArgs.Events,
                    commandArgs.ViewModelKey,
                    YankFactory(commandArgs));

                return Task.CompletedTask;
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

                commandArgs.TextEditorService.PostIndependent(
                    nameof(NewLineBelowCommand),
                    commandArgs.Events,
                    commandArgs.ViewModelKey,
                    NewLineBelowFactory(commandArgs));

                return Task.CompletedTask;
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

                commandArgs.TextEditorService.PostIndependent(
                    nameof(NewLineAboveCommand),
                    commandArgs.Events,
                    commandArgs.ViewModelKey,
                    NewLineAboveFactory(commandArgs));

                return Task.CompletedTask;
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