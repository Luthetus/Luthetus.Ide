Goal: readonly text editor model
(2024-04-28)
--------------------------------

Question:
- Is readonly attached to a model
- Or is readonly attached to a viewmodel?

Goal: test explorer
(2024-04-28)
--------------------------------

- Test explorer needs to filter the terminal to just
	the output of a single command.

Solution:
- Add readonly viewmodel
- readonly keymap
- ???

Terminal.cs:
- Needs to track the position index when a command begins and ends
- Then a dictionary maps terminal command to position start and end indices
- Pull this text out into its own model?