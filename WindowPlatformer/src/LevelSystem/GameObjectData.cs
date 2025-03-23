namespace src.LevelSystem;

public readonly record struct GameObjectData
    (V2f loc, V2f size, ObjectType type, string text);