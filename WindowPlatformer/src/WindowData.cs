namespace src;

public readonly record struct WindowData
    (string title, V2f loc, V2f size, bool movable, bool resizable, u32 color, V2f entryDir);