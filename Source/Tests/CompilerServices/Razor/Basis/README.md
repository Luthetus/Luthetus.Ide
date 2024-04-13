One might decide to have a unit test for every public API.

That is the idea of this folder. A one to one unit test per public API at minimum.

Whether every public API will be done or not is being decided upon. But, perhaps
that wording illustrates the purpose of this folder.

---

Ideas:

- Automate test heatmap... If a C# compiler service could identify all publically scoped
API, it could verify if after the basis was ran whether
all public API was used.

- Visualize the "basis"... so one could look at a given release of Luthetus.Ide
and then bring up the "UnitTests-Basis-Visualization" for that given release.
	- Draw a graph of all public API references.
	- Color the connection green if no exception occurred.
	- Color the connection red if an exception occurred.
	

- Automate test generation...
	- Try any meaningful arguments, automatically
	as their own test cast. Ex: a method which accepts
	an 'int'. One could decide that meaningful arguments
	are [ Int.Min, -2, -1, 0, 1, 2 Int.Max ]
	- Concerns: could automated test cases result in
	bad outcomes? I.e. somehow the test case
	does bank.Withdraw(Int.Max). As per the concern,
	none of this automation exists at the moment.
