namespace src;

public readonly record struct ColorPalette(u32 background, u32 wall, u32 player, u32 danger, u32 goal, u32 breakable, u32 portal)
{
    public ColorPalette(u32 baseCol)
        : this(
            background: baseCol.MultiplyColor(0.255f),
            wall: baseCol.MultiplyColor(0.5f),
            player: baseCol.MixColorLinear(0xffffff),
            danger: baseCol.InvertColor(),
            goal: 0xffb300,
            breakable: baseCol.MixColorLinear(0xaaaaaa).MultiplyColor(0.35f),
            portal: 0x008000
        )
    {
    }


    public u32 this[ObjectType type] => type switch
    {
        ObjectType.Wall => wall,
        ObjectType.Player => player,
        ObjectType.Danger => danger,
        ObjectType.Goal => goal,
        ObjectType.Breakable => breakable,
        ObjectType.Portal => portal,
        _ => throw new($"Invalid object type: {type} ({(u8)type})")
    };
}