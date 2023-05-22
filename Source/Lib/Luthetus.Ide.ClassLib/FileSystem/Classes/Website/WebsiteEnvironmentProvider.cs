using Luthetus.Common.RazorLib.Store.AccountCase;
using Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;
using Fluxor;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.FileSystem.Classes.Website;

public class WebsiteEnvironmentProvider : IEnvironmentProvider
{
    private readonly IState<AccountState> _accountStateWrap;

    public WebsiteEnvironmentProvider(
        IState<AccountState> accountStateWrap)
    {
        _accountStateWrap = accountStateWrap;
    }

    public IAbsoluteFilePath RootDirectoryAbsoluteFilePath
    {
        get
        {
            var accountState = _accountStateWrap.Value;

            return new AbsoluteFilePath(
                string.Empty,
                true,
                this);
        }
    }

    public IAbsoluteFilePath HomeDirectoryAbsoluteFilePath
    {
        get
        {
            var accountState = _accountStateWrap.Value;

            return new AbsoluteFilePath(
                accountState.GroupName + '/',
                true,
                this);
        }
    }

    public char DirectorySeparatorChar => '/';
    public char AltDirectorySeparatorChar => '/';

    public string GetRandomFileName()
    {
        return Guid.NewGuid().ToString();
    }
}