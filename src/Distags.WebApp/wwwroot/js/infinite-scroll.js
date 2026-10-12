export function initialize(scrollEndElement, dotNetReference) {
    const options = {
        root: getScrollableContainer(scrollEndElement),
    };

    const observer = new IntersectionObserver(async (entries) => {
        for (const entry of entries) {
            if (entry.isIntersecting) {
                await dotNetReference.invokeMethodAsync("ScrolledToEnd");
            }
        }
    }, options);

    observer.observe(scrollEndElement);

    return {
        dispose: () => dispose(observer),
    }
}

function dispose(observer) {
    observer.disconnect();
}

function getScrollableContainer(element) {
    while (element) {
        const style = getComputedStyle(element);
        if (style.overflowY !== "visible") {
            return element;
        }
        element = element.parentElement;
    }
    return null;
}