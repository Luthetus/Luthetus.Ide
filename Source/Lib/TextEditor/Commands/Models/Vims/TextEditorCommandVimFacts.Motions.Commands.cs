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
                
                var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
                var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
                var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return Task.CompletedTask;
                    
                Word(
	            	commandArgs.EditContext,
			        modelModifier,
			        viewModelModifier,
			        cursorModifierBag,
			        commandArgs);
			    return Task.CompletedTask;
            });

        public static readonly TextEditorCommand EndCommand = new(
            "Vim::End()", "Vim::End()", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                
                var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
                var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
                var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return Task.CompletedTask;
                    
                End(
	            	commandArgs.EditContext,
			        modelModifier,
			        viewModelModifier,
			        cursorModifierBag,
			        commandArgs);
			    return Task.CompletedTask;
            });

        public static readonly TextEditorCommand BackCommand = new(
            "Vim::Back()", "Vim::Back()", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
				
                var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
                var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
                var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return Task.CompletedTask;
                    
                Back(
	            	commandArgs.EditContext,
			        modelModifier,
			        viewModelModifier,
			        cursorModifierBag,
			        commandArgs);
			    return Task.CompletedTask;
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
                    
	                var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
	                var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
	                var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
	                var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);
	
	                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
	                    return Task.CompletedTask;
	                    
	                return Visual(
		            	commandArgs.EditContext,
				        modelModifier,
				        viewModelModifier,
				        cursorModifierBag,
			        	commandArgs);
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
                    
	                var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
	                var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
	                var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
	                var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);
	
	                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
	                    return Task.CompletedTask;
	                    
	                return VisualLine(
		            	commandArgs.EditContext,
				        modelModifier,
				        viewModelModifier,
				        cursorModifierBag,
			        	commandArgs);
                });
        }
    }
}