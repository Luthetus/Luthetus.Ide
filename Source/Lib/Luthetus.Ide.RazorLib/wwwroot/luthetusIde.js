window.luthetusIde = {
    localStorageSetItem: function (key, value) {
        localStorage.setItem(key, value);
    },
    localStorageGetItem: function (key, value) {
        return localStorage.getItem(key);
    },
    tryFocusHtmlElementById: function (elementId) {
        let element = document.getElementById(elementId);

        if (!element) {
            return false;
        }

        element.focus();
        return true;
    }
}

Blazor.registerCustomEventType('bskeydown', {
    browserEventName: 'keydown',
    createEventArgs: e => {
        if (e.code !== "Tab") {
            e.preventDefault();
        }

        return {
            "key": e.key,
            "code": e.code,
            "ctrlWasPressed": e.ctrlKey,
            "shiftWasPressed": e.shiftKey,
            "altWasPressed": e.altKey
        };
    }
});