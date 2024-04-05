# GOAL: Integrated Terminal

TerminalDisplay.razor would be a pseudo-terminal
CliWrap would be API for the pseudo-terminal to interact with the underlying tty

Thought:
	-How would one make TextEditorViewModelDisplay.razor readonly? (more accurately, 'partially-readonly')
		-This comes up because, the simplest implementation of a terminal
			would have any history on the screen be readonly,
			then the last line in the terminal, (if user input is being read)
			would be modifiable.
		-Idea: Add a 'terminal-keymap' for the text editor.
			-The 'terminal-keymap' could then check when text insertion is typed,
				whether the cursor is on the final line or not.
		-Issue: Would the 'terminal-keymap' be included in the list of 'all-keymaps'?
			-i.e. 'default-keymap', and 'vim-keymap'?
				-Presumably one does NOT want 'terminal-keymap' to be included,
					and for it to be a special keymap for use only by the terminal.