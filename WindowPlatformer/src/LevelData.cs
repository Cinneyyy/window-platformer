namespace src;

public readonly record struct WindowData
    (V2f loc, V2f size, bool movable, bool resizable, u32 color, string title);

public readonly record struct ObjectData
    (V2f loc, V2f size, ObjectType type);

public readonly record struct LevelData
    (WindowData[] windows, ObjectData[] objects, ObjectData player);