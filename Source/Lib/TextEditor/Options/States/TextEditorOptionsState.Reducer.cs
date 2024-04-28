using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;

namespace Luthetus.TextEditor.RazorLib.Options.States;

public partial class TextEditorOptionsState
{
    public class Reducer
    {
        [ReducerMethod]
        public static TextEditorOptionsState ReduceSetFontFamilyAction(
            TextEditorOptionsState inState,
            SetFontFamilyAction setFontFamilyAction)
        {
            return new TextEditorOptionsState
            {
                Options = inState.Options with
                {
                    CommonOptions = inState.Options.CommonOptions with
                    {
                        FontFamily = setFontFamilyAction.FontFamily
                    },
                    RenderStateKey = Key<RenderState>.NewKey(),
                },
            };
        }

        [ReducerMethod]
        public static TextEditorOptionsState ReduceSetFontSizeAction(
            TextEditorOptionsState inState,
            SetFontSizeAction setFontSizeAction)
        {
            return new TextEditorOptionsState
            {
                Options = inState.Options with
                {
                    CommonOptions = inState.Options.CommonOptions with
                    {
                        FontSizeInPixels = setFontSizeAction.FontSizeInPixels
                    },
                    RenderStateKey = Key<RenderState>.NewKey(),
                },
            };
        }

        [ReducerMethod]
        public static TextEditorOptionsState ReduceSetRenderStateKeyAction(
            TextEditorOptionsState inState,
            SetRenderStateKeyAction setRenderStateKeyAction)
        {
            return new TextEditorOptionsState
            {
                Options = inState.Options with
                {
                    RenderStateKey = setRenderStateKeyAction.RenderStateKey
                },
            };
        }

        [ReducerMethod]
        public static TextEditorOptionsState ReduceSetCursorWidthAction(
            TextEditorOptionsState inState,
            SetCursorWidthAction setCursorWidthAction)
        {
            return new TextEditorOptionsState
            {
                Options = inState.Options with
                {
                    CursorWidthInPixels = setCursorWidthAction.CursorWidthInPixels,
                    RenderStateKey = Key<RenderState>.NewKey(),
                },
            };
        }

        [ReducerMethod]
        public static TextEditorOptionsState ReduceSetHeightAction(
            TextEditorOptionsState inState,
            SetHeightAction setHeightAction)
        {
            return new TextEditorOptionsState
            {
                Options = inState.Options with
                {
                    TextEditorHeightInPixels = setHeightAction.HeightInPixels,
                    RenderStateKey = Key<RenderState>.NewKey(),
                },
            };
        }

        [ReducerMethod]
        public static TextEditorOptionsState ReduceSetThemeAction(
            TextEditorOptionsState inState,
            SetThemeAction setThemeAction)
        {
            return new TextEditorOptionsState
            {
                Options = inState.Options with
                {
                    CommonOptions = inState.Options.CommonOptions with
                    {
                        ThemeKey = setThemeAction.Theme.Key
                    },
                    RenderStateKey = Key<RenderState>.NewKey(),
                },
            };
        }

        [ReducerMethod]
        public static TextEditorOptionsState ReduceSetKeymapAction(
            TextEditorOptionsState inState,
            SetKeymapAction setKeymapAction)
        {
            return new TextEditorOptionsState
            {
                Options = inState.Options with
                {
                    Keymap = setKeymapAction.Keymap,
                    RenderStateKey = Key<RenderState>.NewKey(),
                },
            };
        }

        [ReducerMethod]
        public static TextEditorOptionsState ReduceSetShowWhitespaceAction(
            TextEditorOptionsState inState,
            SetShowWhitespaceAction setShowWhitespaceAction)
        {
            return new TextEditorOptionsState
            {
                Options = inState.Options with
                {
                    ShowWhitespace = setShowWhitespaceAction.ShowWhitespace,
                    RenderStateKey = Key<RenderState>.NewKey(),
                },
            };
        }

        [ReducerMethod]
        public static TextEditorOptionsState ReduceSetShowNewlinesAction(
            TextEditorOptionsState inState,
            SetShowNewlinesAction setShowNewlinesAction)
        {
            return new TextEditorOptionsState
            {
                Options = inState.Options with
                {
                    ShowNewlines = setShowNewlinesAction.ShowNewlines,
                    RenderStateKey = Key<RenderState>.NewKey(),
                },
            };
        }

        [ReducerMethod]
        public static TextEditorOptionsState ReduceSetUseMonospaceOptimizationsAction(
            TextEditorOptionsState inState,
            SetUseMonospaceOptimizationsAction setUseMonospaceOptimizationsAction)
        {
            return new TextEditorOptionsState
            {
                Options = inState.Options with
                {
                    UseMonospaceOptimizations = setUseMonospaceOptimizationsAction.UseMonospaceOptimizations,
                    RenderStateKey = Key<RenderState>.NewKey(),
                },
            };
        }
    }
}