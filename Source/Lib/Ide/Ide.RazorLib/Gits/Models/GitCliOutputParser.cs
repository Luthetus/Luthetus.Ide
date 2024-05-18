using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using System.Text;
    private List<string> _branchList = new();
    private int? _behindByCommitCount;
    private int? _aheadByCommitCount;
    private StringBuilder? _logFileContentBuilder;
    public string? LogFileContent { get; private set; }
            GitCommandKind.GetBranchList => GetBranchListLine(output),
            GitCommandKind.LogFile => LogFileParseLine(output),
            else if (stringWalker.CurrentCharacter == 'Y' && stringWalker.PeekForSubstring("Your branch is behind "))
                // Found: "Your branch is behind 'origin/master' by 1 commit, and can be fast-forwarded."
                //
                // Read: "Your branch is behind " (literally)
                _ = stringWalker.ReadRange("Your branch is behind ".Length);

                if (stringWalker.CurrentCharacter != '\'')
                    return textSpanList;

                // Skip opening single-quote
                _ = stringWalker.ReadCharacter();

                // Skip until and including the closing single-quote
                    var character = stringWalker.ReadCharacter();
                    if (character == '\'')
                        break;
                }
                // Read: " by "
                _ = stringWalker.ReadRange(" by ".Length);
                // Read the unsigned-integer
                var syntaxTokenList = new List<ISyntaxToken>();
                LuthLexerUtils.LexNumericLiteralToken(stringWalker, syntaxTokenList);
                var numberTextSpan = syntaxTokenList.Single().TextSpan;
                var numberString = numberTextSpan.GetText();
                if (int.TryParse(numberString, out var localBehindByCommitCount))
                    _behindByCommitCount = localBehindByCommitCount;
                else
                    _behindByCommitCount = null;
                textSpanList.Add(numberTextSpan with
                {
                    DecorationByte = (byte)TerminalDecorationKind.StringLiteral,
                });
                return textSpanList;
            }
            else if (stringWalker.CurrentCharacter == 'Y' && stringWalker.PeekForSubstring("Your branch is ahead of "))
            {
                // Found: "Your branch is ahead of 'origin/master' by 1 commit."
                //
                // Read: "Your branch is ahead of " (literally)
                _ = stringWalker.ReadRange("Your branch is ahead of ".Length);
                if (stringWalker.CurrentCharacter != '\'')
                    return textSpanList;
                // Skip opening single-quote
                _ = stringWalker.ReadCharacter();
                // Skip until and including the closing single-quote
                while (!stringWalker.IsEof)
                {
                    var character = stringWalker.ReadCharacter();
                    if (character == '\'')
                        break;
                }
                // Read: " by "
                _ = stringWalker.ReadRange(" by ".Length);
                // Read the unsigned-integer
                var syntaxTokenList = new List<ISyntaxToken>();
                LuthLexerUtils.LexNumericLiteralToken(stringWalker, syntaxTokenList);
                var numberTextSpan = syntaxTokenList.Single().TextSpan;
                var numberString = numberTextSpan.GetText();
                if (int.TryParse(numberString, out var localBehindByCommitCount))
                    _aheadByCommitCount = localBehindByCommitCount;
                else
                    _aheadByCommitCount = null;
                textSpanList.Add(numberTextSpan with
                {
                    DecorationByte = (byte)TerminalDecorationKind.StringLiteral,
                });
                return textSpanList;

            if (_stageKind == StageKind.IsReadingUntrackedFiles ||
                _stageKind == StageKind.IsReadingStagedFiles ||
                _stageKind == StageKind.IsReadingUnstagedFiles)

                            if (_stageKind == StageKind.IsReadingStagedFiles ||
                                _stageKind == StageKind.IsReadingUnstagedFiles)
                                // Skip the git description
                                //
                                // Example: "new file:   BlazorApp4NetCoreDbg/Persons/Abc.cs"
                                //           ^^^^^^^^^^^^
                                while (!stringWalker.IsEof)
                                    if (stringWalker.CurrentCharacter == ':')
                                    {
                                        // Read the ':'
                                        _ = stringWalker.ReadCharacter();
                                        // Read the 3 ' ' characters (space characters)
                                        _ = stringWalker.ReadRange(3);
                                        break;
                                    }
                                    _ = stringWalker.ReadCharacter();
                                }
                            var gitDirtyReason =
                                _stageKind == StageKind.IsReadingUntrackedFiles ? GitDirtyReason.Untracked :
                                _stageKind == StageKind.IsReadingStagedFiles ? GitDirtyReason.Added :
                                _stageKind == StageKind.IsReadingUnstagedFiles ? GitDirtyReason.Added :
                                GitDirtyReason.None;

                            var gitFile = new GitFile(
                                gitDirtyReason);

                            if (_stageKind == StageKind.IsReadingUntrackedFiles)
                                UntrackedGitFileList.Add(gitFile);
                            else if (_stageKind == StageKind.IsReadingStagedFiles)
                                StagedGitFileList.Add(gitFile);
                            else if (_stageKind == StageKind.IsReadingUnstagedFiles)
                                UnstagedGitFileList.Add(gitFile);
            _ = stringWalker.ReadCharacter();
        }
        return textSpanList;
    }
    public List<TextEditorTextSpan> LogFileParseLine(string output)
    {
        /*
         git log ...redacted
         commit ...redacted
         Author: ...redacted
         Date:   ...redacted
         
             Abc123 3
         
         diff --git a/BlazorApp4NetCoreDbg/Shared/NavMenu.razor b/BlazorApp4NetCoreDbg/Shared/NavMenu.razor
         new file mode ...redacted
         index ...redacted
         --- /dev/null
         +++ ...redacted
         @@ -0,0 +1,39 @@
         +﻿<div class="top-row ps-3 navbar navbar-dark">
         +    <div class="container-fluid">
...For brevity much of the file's contents
...have been left out of this C# comment. 
         +        collapseNavMenu = !collapseNavMenu;
         +    }
         +}
         Process exited; Code: 0
         */

        // The output has every line from the file to start with a single '+' character.
        //
        // For brevity much of the file's contents were left out.
        //
        // But, the pattern can be seen that the file is written out as contiguous
        // lines, where each starts with a '+' character.
        var stringWalker = new StringWalker(new ResourceUri("/__LUTHETUS__/GitCliOutputParser.txt"), output);
        var textSpanList = new List<TextEditorTextSpan>();
        _logFileContentBuilder ??= new();
        // TODO: This code is incorrect. For example, the line "++1"...
        //       ...The first '+' is from the git CLI. But, then the file's text is "+1".
        //       This second '+' is erroneously taken to mean the line isn't the logged file's content.
        if (stringWalker.CurrentCharacter == '+' && stringWalker.NextCharacter != '+')
        {
            // Do not include the '+' as part of the logged file's content.
            _ = stringWalker.ReadCharacter();
            // Skip the 'UTF-8 with BOM'
            if (stringWalker.CurrentCharacter == '�' && stringWalker.PeekForSubstring("﻿"))
                _ = stringWalker.ReadRange("﻿".Length);
            var loggedLineTextSpan = new TextEditorTextSpan(
                stringWalker.PositionIndex,
                output.Length,
                (byte)TerminalDecorationKind.StringLiteral,
                new ResourceUri("/__LUTHETUS__/GitCliOutputParser.txt"),
                output);
            _logFileContentBuilder.Append(loggedLineTextSpan.GetText());
            textSpanList.Add(loggedLineTextSpan);
    
            _branch ??= output.Trim();
    
    public List<TextEditorTextSpan> GetBranchListLine(string output)
    {
        var stringWalker = new StringWalker(new ResourceUri("/__LUTHETUS__/GitCliOutputParser.txt"), output);
        var textSpanList = new List<TextEditorTextSpan>();

        // "* Abc"    <-- Line 1 with quotes added to show where it starts and ends
        // "  master" <-- Line 2 with quotes added to show where it starts and ends
        //
        // Every branch seems to start with 2 characters, where the first is whether it's the active branch,
        // and the second is just a whitespace to separate whether its the active branch from its name.
        //
        // Therefore, naively skip 2 characters then readline.
        var isValid = false;

        if (stringWalker.CurrentCharacter == '*' || stringWalker.CurrentCharacter == ' ')
        {
            if (stringWalker.NextCharacter == ' ')
                isValid = true;
        }

        if (!isValid)
            return textSpanList;

        _ = stringWalker.ReadRange(2);

        var startPositionInclusive = stringWalker.PositionIndex;

        while (!stringWalker.IsEof && !WhitespaceFacts.LINE_ENDING_CHARACTER_LIST.Contains(stringWalker.CurrentCharacter))
        {
            _ = stringWalker.ReadCharacter();
        }

        var textSpan = new TextEditorTextSpan(
            startPositionInclusive,
            stringWalker,
            (byte)TerminalDecorationKind.StringLiteral);

        textSpanList.Add(textSpan);
        _branchList.Add(textSpan.GetText());

        return textSpanList;
    }
                    _gitState.Repo,
                    _origin));
                    _gitState.Repo,
                    _branch));
                break;
            case GitCommandKind.GetBranchList:
                _dispatcher.Dispatch(new GitState.SetBranchListAction(
                    _gitState.Repo,
                    _branchList));
                _dispatcher.Dispatch(new GitState.SetStatusAction(
                    _gitState.Repo,
                    UntrackedGitFileList.ToImmutableList(),
                    StagedGitFileList.ToImmutableList(),
                    UnstagedGitFileList.ToImmutableList(),
                    _behindByCommitCount ?? 0,
                    _aheadByCommitCount ?? 0));
                break;
            case GitCommandKind.LogFile:
                if (_logFileContentBuilder is not null)
                    LogFileContent = _logFileContentBuilder.ToString();
        GetBranchList,
        PushToOriginWithTracking,
        Pull,
        Fetch,
        LogFile,