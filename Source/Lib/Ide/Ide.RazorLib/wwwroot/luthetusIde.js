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