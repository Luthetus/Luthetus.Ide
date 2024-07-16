The workspaces is needed.

It will be like VsCode, where one can have many folders grouped in a tree view.

I need to move out all the .NET code.

I'm not sure how to go about it.

Does this new project which contains the .NET code reference the IDE?
Or does the IDE reference the new project?

I think the long term answer is that the new project is going to reference the IDE.

But, the first step is probably moving the code 1 to 1 over to the new project,
then having the IDE reference this new project FOR NOW.

I suppose this is an inversion of control scenario.

But taking the step to move it into a separate project should hit me with ideas on
some kind of interface that I can expose from the IDE, such that
the new project can give components to the IDE but the IDE doesn't know who the project is?

Is it perhaps just a List<Panel>?
Ooooo is this a good idea in my head?????

What if all these "new projects" made a menu option in the IDE header's "View" dropdown,
which has a submenu, which contains all the components that were made available
by "installing" that project.

And I say "installing", but I don't actually have a clue what I'm doing right now.
I have seen "MEF" (maybe this means microsoft extension framework), I only saw it
once in passing because of a Visual Studio extension maybe it was a roslyn analyzer????

I should probably look into what "MEF" is.

Juuuuuuuuuussssst mooooooove theeeeeeee coooooodeeeeeeee already

But wait! before I do that
I need to write this down.
The keymap is not good because anyone can choose to use "stopPropagation" and then
the event won't bubble up to the global context record.

HOOoooooOOOOwwwwweeeeevveeeeeerrr
if I use javascript to make my own onkeydown event, I can bypass anyone's
component code.

I do recall though, sometimes I want to allow the component code to have first
go at the keymap.

In Visual Studio Code I wanted to use Neovim in the integrated terminal,
but also be able to use a keymap to set focus to the VSCode workspace panel.
Uhhh I think this is what I was doing.

Something about me not being able to go to VSCode because I'm in the terminal or
maybe it was vice versa idk

I said the order that the events should happen in a .json file and then it worked?

Wow, I somehow perfectly ripped out all the .NET code from the IDE and moved it in one sitting.
The app runs without issue and the onnly difference is there's no solution explorer
or test explorer (probably other UI that I'm not listing here but you get the point).

I can open .cs files and the syntax highlighting and all is there.

just as I wanted. I wanted to separate the compiler services and the UI that can compliment them.

tired and my head hurts :3