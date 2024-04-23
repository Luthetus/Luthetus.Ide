using Luthetus.TextEditor.RazorLib.Edits.Models;

namespace Luthetus.TextEditor.RazorLib.Commands.Models.Vims;

public static partial class TextEditorCommandVimFacts
{
    public static partial class Motions
    {
        public static readonly TextEditorCommand Word = new(
            "Vim::Word()", "Vim::Word()", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.TextEditorService.PostReadOnly(
                    nameof(Word),
                    commandArgs.Events,
                    commandArgs.ViewModelKey,
                    WordFactory(commandArgs));

                return Task.CompletedTask;
            })
        {
            TextEditorEditFactory = interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                return WordFactory(commandArgs);
            }
        };

        public static readonly TextEditorCommand End = new(
            "Vim::End()", "Vim::End()", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.TextEditorService.PostReadOnly(
                    nameof(End),
                    commandArgs.Events,
                    commandArgs.ViewModelKey,
                    EndFactory(commandArgs));

                return Task.CompletedTask;
            })
        {
            TextEditorEditFactory = interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                return EndFactory(commandArgs);
            }
        };

        public static readonly TextEditorCommand Back = new(
            "Vim::Back()", "Vim::Back()", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.TextEditorService.PostReadOnly(
                    nameof(Back),
                    commandArgs.Events,
                    commandArgs.ViewModelKey,
                    BackFactory(commandArgs));

                return Task.CompletedTask;
            })
        {
            TextEditorEditFactory = interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                return BackFactory(commandArgs);
            }
        };

        public static TextEditorCommand GetVisualFactory(
            TextEditorCommand innerCommand,
            string displayName)
        {
            return new TextEditorCommand(
                $"Vim::GetVisual({displayName})", $"Vim::GetVisual({displayName})", false, true, TextEditKind.None, null,
                interfaceCommandArgs =>
                {
                    var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                    commandArgs.InnerCommand = innerCommand;

                    commandArgs.TextEditorService.PostReadOnly(
                        nameof(GetVisualFactory),
                        commandArgs.Events,
                        commandArgs.ViewModelKey,
                        VisualFactory(commandArgs));

                    return Task.CompletedTask;
                })
            {
                TextEditorEditFactory = interfaceCommandArgs =>
                {
                    var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                    commandArgs.InnerCommand = innerCommand;

                    return VisualFactory(commandArgs);
                }
            };
        }
        
        public static TextEditorCommand GetVisualLineFactory(
            TextEditorCommand innerCommand,
            string displayName)
        {
            return new TextEditorCommand(
                $"Vim::GetVisual({displayName})", $"Vim::GetVisual({displayName})", false, true, TextEditKind.None, null,
                interfaceCommandArgs =>
                {
                    var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                    commandArgs.InnerCommand = innerCommand;

                    commandArgs.TextEditorService.PostReadOnly(
                        nameof(GetVisualLineFactory),
                        commandArgs.Events,
                        commandArgs.ViewModelKey,
                        VisualLineFactory(commandArgs));

                    return Task.CompletedTask;
                })
            {
                TextEditorEditFactory = interfaceCommandArgs =>
                {
                    var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                    commandArgs.InnerCommand = innerCommand;

                    return VisualLineFactory(commandArgs);
                }
            };
        }
    }
}