Goal: Add 'Find all references' { 'Shift' + 'F12' }
===================================================

It seems reasonable that the implementation details for a 'type member'
and a 'local variable' would not be 1 for 1.

Because, local variables would only be referenceable from the same scope or a child scope.
And, since the scope (or child scopes) exist only within the same file,
finding the references to these locals would not require as much
pre-calculation (partial classes are not relevant here because it would be a type-member-variable in that case).

A type-member-variable can be referenced in other files than just the the same file that
the variable is declared in.

So, more pre-calculation could be useful in the 'type-member-variable' case.


Type-member-variable:
---------------------
Aaa


Local variable:
---------------
Aaa


