using Luthetus.TextEditor.RazorLib.Edits.Models;

namespace Luthetus.TextEditor.RazorLib.Commands.Models.Vims;

public static partial class TextEditorCommandVimFacts
{
    public static partial class Motions
    {
        public static readonly TextEditorCommand WordCommand = new(
            "Vim::Word()", "Vim::Word()", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                Word(commandArgs.EditContext);
            });

        public static readonly TextEditorCommand EndCommand = new(
            "Vim::End()", "Vim::End()", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                End(commandArgs.EditContext);
            });

        public static readonly TextEditorCommand BackCommand = new(
            "Vim::Back()", "Vim::Back()", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
				Back(commandArgs.EditContext)
            });

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
                    Visual(commandArgs.EditContext);
                });
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
                    VisualLineFactory(commandArgs.EditContext)
                });
        }
    }
}