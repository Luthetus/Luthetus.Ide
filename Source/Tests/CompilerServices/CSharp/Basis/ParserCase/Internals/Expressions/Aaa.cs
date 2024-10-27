void Aaa()
{
    if (false)
        return; // Scope is being made here correctly.

    Console.WriteLine(); // But it erroneously continues defining scope at each expression following the if statement.
    
    var variable = 2;

    Console.WriteLine(); // But it erroneously continues defining scope at each expression following the if statement.
}