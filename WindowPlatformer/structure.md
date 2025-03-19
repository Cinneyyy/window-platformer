---
Potential fix for freezing when window dragging:
---

### Main Thread
- Init SDL
- Game Loop
- Rendering

### Window Thread I
- Create windows
- Manage those windows' events via EventWatch
- Poll and discard events

### Window Thread II
- "
- "
- "

=> Window Thread I/II create windows in an alternating fashion, with each level load/reload switching the Window Thread. This will (hopefully) lead to one thread always being free while a window is being dragged, such that new windows can be created during a drag. The main thread is always free this way.