# UI

The game's UI is uGUI with TextMeshPro. The main menu is a world-space canvas on a sign in the scene, not a screen overlay. IMGUI (`OnGUI`) suits small mod windows and debug popups.

## Adding to game UI (uGUI)

- **Clone a game element instead of building one.** `Instantiate(source, parent, worldPositionStays: false)`, then change the text, icon and colour (`Image.color`). Find a source through the panel's own fields. To find a button by what it does, check `onClick.GetPersistentMethodName(i)`.
- **A cloned button keeps its inspector listeners.** `onClick.AddListener` alone leaves the old action running. Replace the event: `button.onClick = new UIButton.ButtonClickedEvent();` and add yours.
- **Look at the runtime hierarchy before you write layout code.** Layout comes from prefab data and differs between panels: some use layout groups, others place children by hand. Handle both cases. If the structure isn't what you expect, log it once and skip the change; don't guess.
- **Put new elements under the panel root, not next to a grid.** A grid often sits in a masked scroll area that clips anything placed beside it.
- **When you place an element by hand inside a layout group,** set `LayoutElement.ignoreLayout = true`.
- **Before you lay out new rows, `SetActive(false)` the old ones.** `Destroy` is deferred, so otherwise old and new rows share one frame of layout.
- **A mask hides pixels, not raycasts.** Scrolled-out or hidden children still receive clicks and can steal them from buttons drawn above. Skip them in the handler's lookup, for example with `RectTransformUtility.RectangleContainsScreenPoint` against the viewport.
- **Make an area scrollable by wrapping it.** Add a viewport with `RectMask2D` and move the original children into a content rect. Save every `anchorMin`/`anchorMax`/`pivot`/`anchoredPosition`/`sizeDelta` first and restore them in the same order. Sibling order is draw order.
- **Forward the mouse wheel.** A component that implements `IScrollHandler` but doesn't use the event must pass it on with `ExecuteEvents.ExecuteHierarchy(parent.gameObject, eventData, ExecuteEvents.scrollHandler)`. Otherwise the list stops scrolling.
- **Hover popups.** Show and hide on pointer enter and exit restarts the appear animation and makes the popup unreachable. Keep it open while the mouse is over the source or the popup, and hide it about 0.3 s after the mouse leaves both. Use `Time.unscaledTime`.
- **Units across canvases.** When you move a neighbour to reclaim space, convert sizes through the parents' `lossyScale`. Keep the original values and restore them on unload.
- **Gamepad navigation doesn't know new elements.** The game's navigation handlers list neighbours by hand. Either patch them, or state that the feature is mouse-only and add a test that "nothing breaks with a gamepad".
- **Reuse game components.** The game's tooltip component and the hover sound copied from an existing element make a mod button feel native. A tooltip can also explain why something is disabled.
- **Long labels:** set `enableAutoSizing` on TMP text in fixed-size buttons.

## Icons and sprites

- **Ship your own icons as embedded PNGs:** `GetManifestResourceStream`, `ImageConversion.LoadImage`, `Sprite.Create`. Cache the sprites, and log an error if a resource is missing.
- **Game sprites found by name** (`Resources.FindObjectsOfTypeAll<Sprite>()`) may not be loaded yet when a panel starts. Don't poll for them. Use your own sprites or a hook that runs after they load.

## IMGUI

- **Cache `GUIStyle`s, but rebuild them when the skin changes.** `OnGUI` runs several times per frame, once per event, so do no `GetComponent` calls or big string building there. Cap long lists.
- **Layout and event passes must build the same controls in the same order** within a frame. Decide visibility from values that don't change during the frame.
- **`GUI.Window` doesn't size itself.** Use `GUILayout.Window` for small windows. Draw the title yourself if the title bar is tall: the window style clips it.
- **Popups inside a window.** A second window or `GUI.ModalWindow` can end up behind the first. Draw the popup inside the same window callback: take its input at the start and paint it at the end.
- **`GUI.Button` fires on MouseUp.** A "click outside closes it" handler on MouseDown runs first.
- **A horizontal scrollbar means some row is wider than the view.** Text that doesn't word-wrap can't shrink.
- **The default font lacks many symbols.** `×` and `✓` work; `✕` and `▲▼` may not.
- **To keep clicks from reaching the game,** disable `EventSystem.current.currentInputModule` while your window is open. Input the game reads directly through `Input` still gets through.

## Before you hand over UI

Walk through every state: empty, full, disabled, hovered, opened and closed, scrolled, a narrow window, a large font, and after a save load. Look at a screenshot if one can be had. Small layout slips are the most common UI bug and cost the tester a game restart each.
