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

                return commandArgs.TextEditorService.PostSimpleBatch(
                    nameof(Word),
                    string.Empty,
					null,
                    WordFactory(commandArgs));
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

                return commandArgs.TextEditorService.PostSimpleBatch(
                    nameof(End),
                    string.Empty,
					null,
                    EndFactory(commandArgs));
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

                return commandArgs.TextEditorService.PostSimpleBatch(
                    nameof(Back),
                    string.Empty,
					null,
                    BackFactory(commandArgs));
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

                    return commandArgs.TextEditorService.PostSimpleBatch(
                        nameof(GetVisualFactory),
                        string.Empty,
						null,
                        VisualFactory(commandArgs));
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

                    return commandArgs.TextEditorService.PostSimpleBatch(
                        nameof(GetVisualLineFactory),
                        string.Empty,
						null,
                        VisualLineFactory(commandArgs));
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