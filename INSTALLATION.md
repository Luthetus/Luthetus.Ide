# Install Luthetus.Ide

### If using Linux or Mac and the publishing doesn't work, try "dotnet run -c Release" from the "/Source/Lib/Ide/Host.Photino/" directory.

- Clone the source code
- Open the path "/Source/Lib/Ide/Host.Photino/" in the terminal

For example my path is "C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\Ide\Host.Photino\"

- Execute in the terminal: "dotnet publish -c Release"

- Change directory to: "./bin/Release/net6.0/publish/"

For example my path is "C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\Ide\Host.Photino\bin\Release\net6.0\publish\"

- Execute in the terminal: "dotnet ./Luthetus.Ide.Photino.dll"

NOTE: there should also be a "Luthetus.Ide.Photino.exe" file
but it might not work depending on one's setup. The .dll terminal command however should
work regardless of one's setup.

- After running the IDE, the application window should appear in the operating systems
window manager.

- It is recommended to check the "Info" tab after installation, to check
if the libraries were compiled with "Release" mode or "Debug" mode.
After following these steps, one should see "Release" mode.