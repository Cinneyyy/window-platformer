namespace src;

public class GameObject
{
    internal GameObject(V2f loc, V2f size, ObjectType type)
    {
        this.type = type;
        this.loc = loc;
        this.size = size;
    }

    internal GameObject(GameObjectData data)
        : this(data.loc, data.size, data.type)
    {
    }


    public SDL_Rect output;
    public bool enabled = true;
    public readonly ObjectType type;


    private V2f _loc;
    public V2f loc
    {
        get => _loc;
        set
        {
            _loc = value;
            (output.x, output.y) = Screen.WorldPointToScreen(value);
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