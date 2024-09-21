// https://stackoverflow.com/questions/75988682/debounce-in-javascript
// https://stackoverflow.com/a/75988895/19310517
const luthetusCommonDebounce = (callback, wait) => {
  let timeoutId = null;
  return (...args) => {
    window.clearTimeout(timeoutId);
    timeoutId = window.setTimeout(() => {
      callback(...args);
    }, wait);
  };
}

const luthetusCommonOnWindowSizeChanged = luthetusCommonDebounce(() => {
    var localBrowserResizeInteropDotNetObjectReference = luthetusCommon.browserResizeInteropDotNetObjectReference;

    if (!localBrowserResizeInteropDotNetObjectReference) {
    	return;
    }
    
	localBrowserResizeInteropDotNetObjectReference
		.invokeMethodAsync("OnBrowserResize")
		.then(data => data);
}, 300);

window.luthetusCommon = {
	browserResizeInteropDotNetObjectReference: null,
    subscribeWindowSizeChanged: function (browserResizeInteropDotNetObjectReference) {
    	// https://github.com/chrissainty/BlazorBrowserResize/blob/master/BrowserResize/BrowserResize/wwwroot/js/browser-resize.js
    	luthetusCommon.browserResizeInteropDotNetObjectReference = browserResizeInteropDotNetObjectReference;
        window.addEventListener("resize", luthetusCommonOnWindowSizeChanged);
    },
    disposeWindowSizeChanged: function () {
    	luthetusCommon.browserResizeInteropDotNetObjectReference = null;
        window.removeEventListener("resize", luthetusCommonOnWindowSizeChanged);
    },
    focusHtmlElementById: function (elementId, preventScroll) {
        let element = document.getElementById(elementId);

        if (!element) {
            return;
        }

		if (preventScroll) {
			element.focus({preventScroll: true});
		}
		else {
			element.focus();
		}
    },
    tryFocusHtmlElementById: function (elementId) {
        let element = document.getElementById(elementId);

        if (!element) {
            return false;
        }

        element.focus();
        return true;
    },
    localStorageSetItem: function (key, value) {
        localStorage.setItem(key, value);
    },
    localStorageGetItem: function (key) {
        return localStorage.getItem(key);
    },
    getTreeViewContextMenuFixedPosition: function (nodeElementId) {

        let treeViewNode = document.getElementById(nodeElementId);
        let treeViewNodeBounds = treeViewNode.getBoundingClientRect();

        return {
            OccurredDueToMouseEvent: false,
            LeftPositionInPixels: treeViewNodeBounds.left,
            TopPositionInPixels: treeViewNodeBounds.top + treeViewNodeBounds.height
        }
    },
    measureElementById: function (elementId) {
        let element = document.getElementById(elementId);

        if (!element) {
            return {
                WidthInPixels: 0,
                HeightInPixels: 0,
                LeftInPixels: 0,
                TopInPixels: 0,
                ZIndex: 0,
            }
        }

        let boundingClientRect = element.getBoundingClientRect();

        return {
            WidthInPixels: boundingClientRect.width,
            HeightInPixels: boundingClientRect.height,
            LeftInPixels: boundingClientRect.left,
            TopInPixels: boundingClientRect.top,
            ZIndex: 0,
        }
    },
    readClipboard: async function () {
        // domexception-on-calling-navigator-clipboard-readtext
        // https://stackoverflow.com/q/56306153/14847452
        // ----------------------------------------------------
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
            // Debugging Linux-Ubuntu (2024-04-28)
            // -----------------------------------
            // Reading clipboard is not working.
            //
            // Fixed with the following inner-try/catch block.
            //
            // This fix upsets me. Seemingly the permission
            // "clipboard-read" doesn't exist for some user-agents
            // But so long as you don't check for permission it lets you read
            // the clipboard?
            try {
                return navigator.clipboard
                    .readText()
                    .then((clipText) => {
                        return clipText;
                    });
            } catch (innerException) {
                return "";
            }
        }
    },
    setClipboard: function (value) {
        // how-do-i-copy-to-the-clipboard-in-javascript:
        // https://stackoverflow.com/a/33928558/14847452
        // ---------------------------------------------
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