window.luthetusIde = {
    
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