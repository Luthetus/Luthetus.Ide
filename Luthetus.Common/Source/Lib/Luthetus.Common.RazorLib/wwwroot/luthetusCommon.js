window.luthetusCommon = {
    localStorageSetItem: function (key, value) {
        localStorage.setItem(key, value);
    },
    localStorageGetItem: function (key, value) {
        return localStorage.getItem(key);
    },
    readClipboard: async function () {
        // First, ask the Permissions API if we have some kind of access to
        // the "clipboard-read" feature.

        try {
            return await navigator.permissions.query({name: "clipboard-read"}).then(async (result) => {
                // If permission to read the clipboard is granted or if the user will
                // be prompted to allow it, we proceed.

                if (result.state === "granted" || result.state === "prompt") {
                    return await navigator.clipboard.readText().then((data) => {
                        return data;
                    });
                } else {
                    return "";
                }
            });
        } catch (e) {
            return "";
        }
    },
    setClipboard: function (value) {
        // Copies a string to the clipboard. Must be called from within an
        // event handler such as click. May return false if it failed, but
        // this is not always possible. Browser support for Chrome 43+,
        // Firefox 42+, Safari 10+, Edge and Internet Explorer 10+.
        // Internet Explorer: The clipboard feature may be disabled by
        // an administrator. By default a prompt is shown the first
        // time the clipboard is used (per session).
        if (window.clipboardData && window.clipboardData.setData) {
            // Internet Explorer-specific code path to prevent textarea being shown while dialog is visible.
            return window.clipboardData.setData("Text", text);

        } else if (document.queryCommandSupported && document.queryCommandSupported("copy")) {
            var textarea = document.createElement("textarea");
            textarea.textContent = value;
            textarea.style.position = "fixed";  // Prevent scrolling to bottom of page in Microsoft Edge.
            document.body.appendChild(textarea);
            textarea.select();
            try {
                return document.execCommand("copy");  // Security exception may be thrown by some browsers.
            } catch (ex) {
                console.warn("Copy to clipboard failed.", ex);
                return false;
            } finally {
                document.body.removeChild(textarea);
            }
        }
    },
    getTreeViewContextMenuFixedPosition: function (
        treeViewStateKey,
        treeViewNodeId) {

        let treeViewNode = document.getElementById(treeViewNodeId);
        let treeViewNodeBounds = treeViewNode.getBoundingClientRect();

        return {
            OccurredDueToMouseEvent: false,
            LeftPositionInPixels: treeViewNodeBounds.left,
            TopPositionInPixels: treeViewNodeBounds.top + treeViewNodeBounds.height
        }
    }
}

Blazor.registerCustomEventType('keydownwithpreventscroll', {
    browserEventName: 'keydown',
    createEventArgs: e => {

        let preventDefaultOnTheseKeys = [
            "ContextMenu",
            "ArrowLeft",
            "ArrowDown",
            "ArrowUp",
            "ArrowRight",
            "Home",
            "End",
            "Space",
            "Enter",
            "PageUp",
            "PageDown"
        ];

        let preventDefaultOnTheseCodes = [
            "Space",
            "Enter",
        ];

        if (preventDefaultOnTheseKeys.indexOf(e.key) !== -1 ||
            preventDefaultOnTheseCodes.indexOf(e.code) !== -1) {
            e.preventDefault();
        }

        return {
            Type: e.type,
            MetaKey: e.metaKey,
            AltKey: e.altKey,
            ShiftKey: e.shiftKey,
            CtrlKey: e.ctrlKey,
            Repeat: e.repeat,
            Location: e.location,
            Code: e.code,
            Key: e.key
        };
    }
});