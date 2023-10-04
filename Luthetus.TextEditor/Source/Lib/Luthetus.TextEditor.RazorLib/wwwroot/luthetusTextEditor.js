window.luthetusTextEditor = {
    focusHtmlElementById: function (elementId) {
        let element = document.getElementById(elementId);

        if (!element) {
            return;
        }

        element.focus();
    },
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
    measureCharacterWidthAndRowHeight: function (elementId, amountOfCharactersRendered) {
        let element = document.getElementById(elementId);

        if (!element) {
            return {
                CharacterWidthInPixels: 5,
                RowHeightInPixels: 5
            }
        }
        
        let fontWidth = element.offsetWidth / amountOfCharactersRendered;

        return {
            CharacterWidthInPixels: fontWidth,
            RowHeightInPixels: element.offsetHeight
        }
    },
    measureWidthAndHeightOfTextEditor: function (elementId) {
        let element = document.getElementById(elementId);

        if (!element) {
            return {
                WidthInPixels: 0,
                HeightInPixels: 0
            }
        }
        
        return {
            WidthInPixels: element.offsetWidth,
            HeightInPixels: element.offsetHeight
        }
    },
    escapeHtml: function(input)
    {
        return input
            .replaceAll("&", "&amp;")
            .replaceAll("<", "&lt;")
            .replaceAll(">", "&gt;")
            .replaceAll("\t", "&nbsp;&nbsp;&nbsp;&nbsp;")
            .replaceAll(" ", "&nbsp;")
            .replaceAll("\r\n", "<br/>")
            .replaceAll("\n", "<br/>")
            .replaceAll("\r", "<br/>")
            .replaceAll("\"", "&quot;")
            .replaceAll("'", "&#39;");
    },
    calculateProportionalColumnIndex:
        function (
            containerElementId,
            parentElementId,
            cursorElementId,
            positionXInPixels,
            characterWidthInPixels,
            textOnRow) {
        
        let containerElement = document
            .getElementById(containerElementId);

        if (!containerElement) {
            return 0;
        }
        
        let parentElement = document
            .createElement("div");

        parentElement.id = parentElementId;
        parentElement.style.minHeight = "1ch";

        containerElement.append(parentElement);

        let cursorElement = document
            .createElement("span");
        
        cursorElement.id = cursorElementId;

        parentElement.append(cursorElement);
            
        let columnIndex = Math.trunc(
            positionXInPixels / characterWidthInPixels);
        
        let columnIndexWasOutOfBoundsTooBig = false;
        let columnIndexWasOutOfBoundsTooSmall = false;
        
        if (columnIndex > textOnRow.length) {
            columnIndexWasOutOfBoundsTooBig = true;
            columnIndex = textOnRow.length;
        }
        else if (columnIndex < 0) {
            columnIndexWasOutOfBoundsTooSmall = true;
            columnIndex = 0;
        }

        let lowerBoundColumnIndex = null;
        let lowerBoundLeftOffset = null;
        
        let upperBoundColumnIndex = null;
        let upperBoundLeftOffset = null;
        
        let maxLoopCount = textOnRow.length + 1;
        let loopCount = 0;
        
        while(loopCount++ < maxLoopCount) {
            if (lowerBoundColumnIndex && upperBoundColumnIndex) {
                break;
            }
            
            let escapedText = this.escapeHtml(
                textOnRow.substring(
                    0,
                    columnIndex));

            let leftOffset = this.calculateProportionalLeftOffset(
                containerElementId,
                parentElementId,
                cursorElementId,
                escapedText,
                false
            );
            
            if (leftOffset < positionXInPixels) {
                if (columnIndexWasOutOfBoundsTooBig) {
                    return columnIndex;
                }
                
                lowerBoundColumnIndex = columnIndex++;
                lowerBoundLeftOffset = leftOffset; 
            }
            else {
                if (columnIndexWasOutOfBoundsTooSmall) {
                    return columnIndex;
                }
                
                upperBoundColumnIndex = columnIndex--;
                upperBoundLeftOffset = leftOffset;
            }
        }

        parentElement.removeChild(cursorElement);
        containerElement.removeChild(parentElement);

        if (!lowerBoundColumnIndex || !upperBoundColumnIndex) {
            return -1;
        }
        
        let lowerBoundMissingBy = positionXInPixels - lowerBoundLeftOffset;
        let upperBoundMissingBy = upperBoundLeftOffset - positionXInPixels;
        
        return lowerBoundMissingBy < upperBoundMissingBy
            ? lowerBoundColumnIndex
            : upperBoundColumnIndex;
    },
    calculateProportionalLeftOffset:
        function (
            containerElementId,
            parentElementId,
            cursorElementId,
            textOffsettingCursor,
            shouldCreateElements) {

        let containerElement = document
            .getElementById(containerElementId);

        if (!containerElement) {
            return 0;
        }
        
        if (shouldCreateElements) {
            let parentElement = document
                .createElement("div");

            parentElement.id = parentElementId;
            parentElement.style.minHeight = "1ch";

            containerElement.append(parentElement);

            let cursorElement = document
                .createElement("span");

            cursorElement.id = cursorElementId;

            parentElement.append(cursorElement);
        }
        
        let parentElement = document.getElementById(parentElementId);
        let cursorElement = document.getElementById(cursorElementId);

        if (!parentElement || !cursorElement) {
            return 0;
        }

        let span = document
            .createElement("span");

        span.innerHTML = textOffsettingCursor;
        span.style.display = "inline-block";

        parentElement.insertBefore(
            span,
            parentElement.children[0]);

        let parentBoundingClientRect = parentElement.getBoundingClientRect();
        let targetBoundingClientRect = cursorElement.getBoundingClientRect();
        
        parentElement.removeChild(span);

        if (shouldCreateElements) {
            parentElement.removeChild(cursorElement);
            containerElement.removeChild(parentElement);
        }
        
        return targetBoundingClientRect.left - parentBoundingClientRect.left;
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
    mutateScrollVerticalPositionByPixels: function (textEditorBodyId, gutterElementId, pixels) {
        let textEditorBody = document.getElementById(textEditorBodyId);
        let textEditorGutter = document.getElementById(gutterElementId);
        
        if (!textEditorBody) {
            return;
        }
        
        textEditorBody.scrollTop += pixels;

        this.validateTextEditorBodyScrollPosition(textEditorBody);
        
        if (textEditorGutter) {
            textEditorGutter.scrollTop = textEditorBody.scrollTop;
        }
    },
    mutateScrollHorizontalPositionByPixels: function (textEditorBodyId, gutterElementId, pixels) {
        let textEditorBody = document.getElementById(textEditorBodyId);

        if (!textEditorBody) {
            return;
        }
        
        textEditorBody.scrollLeft += pixels;

        this.validateTextEditorBodyScrollPosition(textEditorBody);
    },
    setScrollPosition: function (textEditorBodyId, gutterElementId, scrollLeft, scrollTop) {
        let textEditorBody = document.getElementById(textEditorBodyId);
        let textEditorGutter = document.getElementById(gutterElementId);

        if (!textEditorBody) {
            return;
        }
        
        if (scrollLeft || scrollLeft === 0) {
            textEditorBody.scrollLeft = scrollLeft;
        }
        
        if (scrollTop || scrollTop === 0) {
            textEditorBody.scrollTop = scrollTop;
        }

        this.validateTextEditorBodyScrollPosition(textEditorBody);

        if (textEditorGutter) {
            textEditorGutter.scrollTop = textEditorBody.scrollTop;
        }
    },
    validateTextEditorBodyScrollPosition: function (textEditorBodyElement) {
        // Validate scrollLeft
        let currentLargestLeftPosition = 
            textEditorBodyElement.scrollLeft + textEditorBodyElement.offsetWidth;

        if (currentLargestLeftPosition > textEditorBodyElement.scrollWidth) {
            textEditorBodyElement.scrollLeft =
                textEditorBodyElement.scrollWidth - textEditorBodyElement.offsetWidth;
        }

        // Validate scrollTop
        let currentLargestTopPosition =
            textEditorBodyElement.scrollTop + textEditorBodyElement.offsetHeight;

        if (currentLargestTopPosition > textEditorBodyElement.scrollHeight) {
            textEditorBodyElement.scrollTop =
                textEditorBodyElement.scrollHeight - textEditorBodyElement.offsetHeight;
        }
    }, 
    setGutterScrollTop: function (gutterElementId, scrollTop) {
        let textEditorGutter = document.getElementById(gutterElementId);

        if (!textEditorGutter) {
            return;
        }

        textEditorGutter.scrollTop = scrollTop;
    },
    getElementMeasurementsInPixelsById: function (elementId) {
        let elementReference = document.getElementById(elementId);

        // The function "getElementMeasurementsInPixelsByElementReference"
        // is safe to pass a null value to. Therefore no null check is being made here.
        return this.getElementMeasurementsInPixelsByElementReference(elementReference);
    },
    getElementMeasurementsInPixelsByElementReference: function (elementReference) {
        if (!elementReference) {
            return {
                ScrollLeft: 0,
                ScrollTop: 0,
                ScrollWidth: 0,
                ScrollHeight: 0,
                Width: 0,
                Height: 0,
            };
        }

        return {
            ScrollLeft: elementReference.scrollLeft,
            ScrollTop: elementReference.scrollTop,
            ScrollWidth: elementReference.scrollWidth,
            ScrollHeight: elementReference.scrollHeight,
            Width: elementReference.offsetWidth,
            Height: elementReference.offsetHeight,
        };
    },
    cursorIntersectionObserverMap: new Map(),
    initializeTextEditorCursorIntersectionObserver: function (intersectionObserverMapKey,
                                                              virtualizationDisplayDotNetObjectReference,
                                                              scrollableParentElementId,
                                                              cursorElementId) {

        let scrollableParent = document.getElementById(scrollableParentElementId);

        if (!scrollableParent) {
            return;
        }
        
        // The IntersectionObserver's options are readonly.
        // Therefore "rootMargin: '-60px'" is awkwardly hardcoded here as I find it 'feels correct'
        // for any font-size one might use. Because one cannot dynamically change the 'rootMargin'
        let options = {
            root: scrollableParent,
            rootMargin: '-60px',
            threshold: [ 0 ]
        }

        let intersectionObserver = new IntersectionObserver((entries) => {
            let intersectionObserverMapValue = this.cursorIntersectionObserverMap
                .get(intersectionObserverMapKey);

            if (!intersectionObserverMapValue) {
                return;
            }

            for (let i = 0; i < entries.length; i++) {

                let entry = entries[i];

                if (!intersectionObserverMapValue.CursorIsIntersectingTuples) {
                    return;
                }

                let cursorTuple = intersectionObserverMapValue.CursorIsIntersectingTuples
                    .find(x => x.CursorElementId === entry.target.id);
                
                if (!cursorTuple) {
                    virtualizationDisplayDotNetObjectReference
                        .invokeMethodAsync("OnCursorPassedIntersectionThresholdAsync",
                            false);
                    
                    return;
                }

                if (cursorTuple.IsIntersecting !== entry.isIntersecting) {
                    
                    cursorTuple.IsIntersecting = entry.isIntersecting;

                    virtualizationDisplayDotNetObjectReference
                        .invokeMethodAsync("OnCursorPassedIntersectionThresholdAsync",
                            cursorTuple.IsIntersecting);
                }
            }
        }, options);

        let cursorIsIntersectingTuples = [];

        let cursorElement = document.getElementById(cursorElementId);

        if (!cursorElement) {
            return;
        }
        
        intersectionObserver.observe(cursorElement);

        cursorIsIntersectingTuples.push({
            CursorElementId: cursorElementId,
            IsIntersecting: false
        });

        this.cursorIntersectionObserverMap.set(intersectionObserverMapKey, {
            IntersectionObserver: intersectionObserver,
            CursorIsIntersectingTuples: cursorIsIntersectingTuples
        });
    },
    revealCursor: function (intersectionObserverMapKey,
                            cursorElementId) {

        let intersectionObserverMapValue = this.cursorIntersectionObserverMap
            .get(intersectionObserverMapKey);

        let cursorTuple = intersectionObserverMapValue.CursorIsIntersectingTuples
            .find(x => x.CursorElementId === cursorElementId);

        if (!cursorTuple) {
            return;
        }
        
        if (!cursorTuple.IsIntersecting) {
            let cursorElement = document.getElementById(cursorElementId);

            if (!cursorElement) {
                return;
            }
            
            cursorElement.scrollIntoView({
                block: "nearest",
                inline: "nearest"
            });
        }
    },
    disposeTextEditorCursorIntersectionObserver: function (intersectionObserverMapKey) {

        let intersectionObserverMapValue = this.cursorIntersectionObserverMap
            .get(intersectionObserverMapKey);
        
        if (!intersectionObserverMapValue) {
            return;
        }

        let intersectionObserver = intersectionObserverMapValue.IntersectionObserver;

        this.cursorIntersectionObserverMap.delete(intersectionObserverMapKey);

        intersectionObserver.disconnect();
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