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
                
                var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
                var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
                var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return ValueTask.CompletedTask;
                
                DeleteLine(
	            	commandArgs.EditContext,
			        modelModifier,
			        viewModelModifier,
			        cursorModifierBag,
			        commandArgs);
			    return ValueTask.CompletedTask;
            });

        public static readonly TextEditorCommand ChangeLineCommand = new(
            "Vim::Change(Line)", "Vim::Change(Line)", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                
                var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
                var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
                var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return ValueTask.CompletedTask;
                    
                return ChangeLine(
	            	commandArgs.EditContext,
			        modelModifier,
			        viewModelModifier,
			        cursorModifierBag,
			        commandArgs);
            });

        public static TextEditorCommand DeleteMotionCommandConstructor(TextEditorCommand innerTextEditorCommand) => new(
            $"Vim::Delete({innerTextEditorCommand.DisplayName})", $"Vim::Delete({innerTextEditorCommand.DisplayName})", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                commandArgs.InnerCommand = innerTextEditorCommand;
                
                var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
                var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
                var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return ValueTask.CompletedTask;
                    
                return DeleteMotion(
	            	commandArgs.EditContext,
			        modelModifier,
			        viewModelModifier,
			        cursorModifierBag,
			        commandArgs);
            });

        public static TextEditorCommand ChangeMotionCommandConstructor(TextEditorCommand innerTextEditorCommand) => new(
            $"Vim::Change({innerTextEditorCommand.DisplayName})", $"Vim::Change({innerTextEditorCommand.DisplayName})", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                commandArgs.InnerCommand = innerTextEditorCommand;
				
                var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
                var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
                var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return ValueTask.CompletedTask;

                return ChangeMotion(
	            	commandArgs.EditContext,
			        modelModifier,
			        viewModelModifier,
			        cursorModifierBag,
			        commandArgs);
            });

        public static readonly TextEditorCommand ChangeSelectionCommand = new(
            "Vim::Change(Selection)", "Vim::Change(Selection)", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
				
                var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
                var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
                var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return ValueTask.CompletedTask;
                    
                return ChangeSelection(
	            	commandArgs.EditContext,
			        modelModifier,
			        viewModelModifier,
			        cursorModifierBag,
			        commandArgs);
            });

        public static readonly TextEditorCommand YankCommand = new(
            "Vim::Yank(Selection)", "Vim::Yank(Selection)", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
				
                var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
                var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
                var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return ValueTask.CompletedTask;

                return YankAsync(
	            	commandArgs.EditContext,
			        modelModifier,
			        viewModelModifier,
			        cursorModifierBag,
			        commandArgs);
            });

        public static readonly TextEditorCommand NewLineBelowCommand = new(
            "Vim::NewLineBelow()", "Vim::NewLineBelow()", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                
                var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
                var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
                var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return ValueTask.CompletedTask;
                    
                NewLineBelow(
	            	commandArgs.EditContext,
			        modelModifier,
			        viewModelModifier,
			        cursorModifierBag,
			        commandArgs);
			    return ValueTask.CompletedTask;
            });

        public static readonly TextEditorCommand NewLineAboveCommand = new(
            "Vim::NewLineAbove()", "Vim::NewLineAbove()", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
				
                var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
                var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
                var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return ValueTask.CompletedTask;
                    
                NewLineAbove(
	            	commandArgs.EditContext,
			        modelModifier,
			        viewModelModifier,
			        cursorModifierBag,
			        commandArgs);
			    return ValueTask.CompletedTask;
            });
    }
}