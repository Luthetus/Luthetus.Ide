# Luthetus.Ide (In Development)
![Example GIF](./Images/Ide/Gifs/ide0.3.0.gif)

## Demo:
https://luthetus.github.io/Luthetus.Ide/

## Introduction:

- A free and open source IDE
- Runs on Linux, Windows, and Mac
- Written with the .NET environment: C#, Blazor UI, and a lightweight Photino webview host.

***Luthetus.Ide*** is an IDE, that works with any programming language, and can be as simple as a plain text editor, or as complex as an IDE.

These days, its common to have many different IDE(s) downloaded. Perhaps you have a separate IDE downloaded for: C#, JavaScript, Rust.

***Luthetus.Ide*** works by way of the interface named "[ILuthCompilerService](/Source/Lib/TextEditor/CompilerServices/Interfaces/ILuthCompilerService.cs)".

Therefore, any programming language can be supported. It is only a question of which ILuthCompilerService(s) one chooses to use.

As of this point in development, the [C# compiler service](/Source/Lib/CompilerServices/CSharp/CompilerServiceCase/CSharpCompilerService.cs) is the most developed implementation. I ["dogfood"](https://en.wikipedia.org/wiki/Eating_your_own_dog_food) the IDE, and since the source code is C#, I spend most time on the C# compiler service.

The IDE needs to be language agnostic, if one never will write C# code, then they shouldn't be forced to have that compiler service added.

Furthermore, many IDE(s) that exist run on a single operating system. ***Luthetus.Ide*** does not tie you to any particular operating system, it is fully cross platform with Linux, Mac, and Windows.

### "Why is ***Luthetus.Ide*** free and open source?".

This is not a matter of undercutting the market, nor is it a matter of me attempting to justify a badly coded product.

An IDE is a ubiquitous tool for programming. Ubiquitous software tooling should not be proprietary software that traps the individual into an ecosystem.

## Installation:
[INSTALLATION.md](./INSTALLATION.md)

## Donations:

I am currently working full time on this project, and live with my parents.

It has been quite stressful for me and my family that I spend so much time on this project, as I am turning 26 years old 2024-09-16.

Once I turn 26, I will not be eligible for my parents healthcare.

As such my goal with this donation button is to try and reach an income of $800 a month (equal to a job that pays $5 an hour), so I can pay for healthcare while continuing to do this full time.

But, I can get a paying job and do this part time if financially necessary.

[![Donate with PayPal](https://raw.githubusercontent.com/Luthetus/paypal-donate-button_Fork/master/paypal-donate-button.png)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=RCG8QN3KL623Y)

## Run the repo:
Clone, then run `Luthetus.Ide.Photino.csproj` (or any of the other Hosts such as WASM or ServerSide).

## NuGet Packages:
The individual libraries used in Luthetus.Ide are available as NuGet Packages.

There is a README.md for each of the libraries to aid in installation:

- [Luthetus.Common](./Docs/Common/README.md)
- [Luthetus.TextEditor](./Docs/TextEditor/README.md)
- [Luthetus.CompilerServices](./Docs/CompilerServices/README.md)

## My Youtube Videos
You may be interested in visiting my [youtube channel](https://www.youtube.com/channel/UCzhWhqYVP40as1MFUesQM9w). I make videos about this repository there.

## Why Am I Making this IDE?
- A year ago I made the github repo "Luthetus.About". And everytime I open the Luthetus.Ide repo I am reminded of its existence and think to myself "what is blud waffling about".
- https://github.com/Luthetus/Luthetus.About