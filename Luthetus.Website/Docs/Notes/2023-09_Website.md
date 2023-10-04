2023-09 | Luthetus.Website | Notes

---

## 2023-09-15

I got this error:

> Uncaught TypeError: Cannot read properties of undefined (reading 'CursorIsIntersectingTuples') at IntersectionObserver.&lt;anonymous&gt; (luthetusTextEditor.js:414:51)

> luthetusTextEditor.js:414

```js
//for (let i = 0; i < entries.length; i++) {...
if (!intersectionObserverMapValue.CursorIsIntersectingTuples) {
    return;
}
```