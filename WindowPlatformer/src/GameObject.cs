using src.LevelSystem;

namespace src;

public class GameObject
{
    internal GameObject(V2f loc, V2f size, ObjectType type)
    {
        this.type = type;
        this.size = size;
        this.loc = loc;
    }

    internal GameObject(GameObjectData data)
        : this(data.loc, data.size, data.type)
    {
    }


    public SDL_Rect output;
    public readonly ObjectType type;


    private V2f _loc;
    public V2f loc
    {
        get => _loc;
        set
        {
            _loc = value;
            (output.x, output.y) = Screen.WorldPointToScreen(new(
                value.x - size.x/2f,
                value.y + size.y/2f
            ));
        }
    }

    private V2f _size;
    public V2f size
    {
        get => _size;
        set
        {
            _size = value;
            (output.w, output.h) = Screen.WorldSizeToScreen(value);
        }
    }
}