window.luthetusTextEditor = {
    scrollElementIntoView: function (elementId) {

        let element = document.getElementById(elementId);

        if (!element) {
            return;
        }
        
        element.scrollIntoView({
            block: "nearest",
            inline: "nearest"
        });
    },
    preventDefaultOnWheelEvents: function (elementId) {
        
        let element = document.getElementById(elementId);

        if (!element) {
            return;
        }
        
        element.addEventListener('wheel', (event) => {
            event.preventDefault();
        }, {
            passive: false,
        });
        
        element.addEventListener('touchstart', (event) => {
            event.preventDefault();
        }, {
            passive: false,
        });
    },
    getCharAndLineMeasurementsInPixelsById: function (elementId, amountOfCharactersRendered) {
        let element = document.getElementById(elementId);

        if (!element) {
            return {
                CharacterWidth: 5,
                LineHeight: 5
            }
        }
        
        let fontWidth = element.offsetWidth / amountOfCharactersRendered;

        return {
            CharacterWidth: fontWidth,
            LineHeight: element.offsetHeight
        }
    },
    getRelativePosition: function (elementId, clientX, clientY) {
        let element = document.getElementById(elementId);

        if (!element) {
            return {
                RelativeX: 0,
                RelativeY: 0,
                RelativeScrollLeft: 0,
                RelativeScrollTop: 0
            }
        }

        let bounds = element.getBoundingClientRect();

        let x = clientX - bounds.left;
        let y = clientY - bounds.top;

        return {
            RelativeX: x,
            RelativeY: y,
            RelativeScrollLeft: element.scrollLeft,
            RelativeScrollTop: element.scrollTop
        }
    },
    setScrollPosition: function (textEditorBodyId, gutterElementId, scrollLeft, scrollTop) {
        let textEditorBody = document.getElementById(textEditorBodyId);
        let textEditorGutter = document.getElementById(gutterElementId);

        if (!textEditorBody) {
            return;
        }
        
		// 0 is falsey
        if (scrollLeft || scrollLeft === 0) {
            textEditorBody.scrollLeft = scrollLeft;
        }
        
		// 0 is falsey
        if (scrollTop || scrollTop === 0) {
            textEditorBody.scrollTop = scrollTop;
        }

        if (textEditorGutter) {
            textEditorGutter.scrollTop = textEditorBody.scrollTop;
        }
    },
    setGutterScrollTop: function (gutterElementId, scrollTop) {
        let textEditorGutter = document.getElementById(gutterElementId);

        if (!textEditorGutter) {
            return;
        }

        textEditorGutter.scrollTop = scrollTop;
    },
    getTextEditorMeasurementsInPixelsById: function (elementId) {
        let elementReference = document.getElementById(elementId);

        if (!elementReference) {
            return {
                Width: 0,
                Height: 0,
				BoundingClientRectLeft: 0,
				BoundingClientRectTop: 0,
            };
        }

		let boundingClientRect = elementReference.getBoundingClientRect();

        return {
            Width: Math.ceil(elementReference.offsetWidth),
            Height: Math.ceil(elementReference.offsetHeight),
			BoundingClientRectLeft: boundingClientRect.left,
			BoundingClientRectTop: boundingClientRect.top,
        };
    },
    getBoundingClientRect: function (elementId) {
        let element = document.getElementById(elementId);

        if (!element) {
            return {
                Left: -1,
                Top: -1,
            };
        }

        let boundingClientRect = element.getBoundingClientRect();

        return {
            Left: boundingClientRect.left,
            Top: boundingClientRect.top,
        };
    }
}