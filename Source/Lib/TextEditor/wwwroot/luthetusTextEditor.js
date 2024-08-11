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
    virtualizationIntersectionObserverMap: new Map(),
    initializeVirtualizationIntersectionObserver: function (intersectionObserverMapKey,
                                                            virtualizationDisplayDotNetObjectReference,
                                                            scrollableParentFinder,
                                                            boundaryIds) {

        let scrollableParent = scrollableParentFinder.parentElement;

        if (!scrollableParent) {
            return;
        }
        
        scrollableParent.addEventListener("scroll", (event) => {
            let hasIntersectingBoundary = false;

            let intersectionObserverMapValue = this.virtualizationIntersectionObserverMap
                .get(intersectionObserverMapKey);

            if (!intersectionObserverMapValue) {
                // Received an error that intersectionObserverMapValue was
                // undefined after closing a tab in the editor
                return;
            }

            for (let i = 0; i < intersectionObserverMapValue.BoundaryIdIsIntersectingTuples.length; i++) {
                let boundaryTuple = intersectionObserverMapValue.BoundaryIdIsIntersectingTuples[i];

                if (boundaryTuple.IsIntersecting) {
                    hasIntersectingBoundary = true;
                }
            }

            if (hasIntersectingBoundary) {
                virtualizationDisplayDotNetObjectReference
                    .invokeMethodAsync("OnScrollEventAsync", {
                        ScrollLeftInPixels: scrollableParent.scrollLeft,
                        ScrollTopInPixels: scrollableParent.scrollTop
                    });
            }
        }, true);

        let options = {
            root: scrollableParent,
            rootMargin: '0px',
            threshold: 0
        }

        let intersectionObserver = new IntersectionObserver((entries) => {
            let hasIntersectingBoundary = false;

            let intersectionObserverMapValue = this.virtualizationIntersectionObserverMap
                .get(intersectionObserverMapKey);

            for (let i = 0; i < entries.length; i++) {

                let entry = entries[i];

                let boundaryTuple = intersectionObserverMapValue.BoundaryIdIsIntersectingTuples
                    .find(x => x.BoundaryId === entry.target.id);
                
                if (!boundaryTuple) {
                    return;
                }

                boundaryTuple.IsIntersecting = entry.isIntersecting;

                if (boundaryTuple.IsIntersecting) {
                    hasIntersectingBoundary = true;
                }
            }

            if (hasIntersectingBoundary) {
                virtualizationDisplayDotNetObjectReference
                    .invokeMethodAsync("OnScrollEventAsync", {
                        ScrollLeftInPixels: scrollableParent.scrollLeft,
                        ScrollTopInPixels: scrollableParent.scrollTop
                    });
            }
        }, options);

        let boundaryIdIsIntersectingTuples = [];

        for (let i = 0; i < boundaryIds.length; i++) {

            let boundaryElement = document.getElementById(boundaryIds[i]);

            if (boundaryElement) {
                intersectionObserver.observe(boundaryElement);

                boundaryIdIsIntersectingTuples.push({
                    BoundaryId: boundaryIds[i],
                    IsIntersecting: false
                });
            }
        }

        this.virtualizationIntersectionObserverMap.set(intersectionObserverMapKey, {
            IntersectionObserver: intersectionObserver,
            BoundaryIdIsIntersectingTuples: boundaryIdIsIntersectingTuples
        });

        virtualizationDisplayDotNetObjectReference
            .invokeMethodAsync("OnScrollEventAsync", {
                ScrollLeftInPixels: scrollableParent.scrollLeft,
                ScrollTopInPixels: scrollableParent.scrollTop
            });
    },
    disposeVirtualizationIntersectionObserver: function (intersectionObserverMapKey) {

        let intersectionObserverMapValue = this.virtualizationIntersectionObserverMap
            .get(intersectionObserverMapKey);

        let intersectionObserver = intersectionObserverMapValue.IntersectionObserver;

        this.virtualizationIntersectionObserverMap.delete(intersectionObserverMapKey);

        intersectionObserver.disconnect();
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