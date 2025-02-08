namespace src;

public class GameObject
{
    public GameObject(V2f loc, V2f size, ObjectType type)
    {
        this.type = type;
        enabled = true;

        this.loc = loc;
        this.size = size;
        OnMove();
        OnResize();
    }

    public GameObject(in ObjectData data)
        : this(data.loc, data.size, data.type)
    {
    }


    public V2f loc, size;
    public SDL_Rect output;
    public bool enabled;
    public readonly ObjectType type;


    public void OnMove()
        => (output.x, output.y) = Screen.WorldPointToScreen(new(
            loc.x - size.x/2f,
            loc.y + size.y/2f
        ));

    public void OnResize()
        => (output.w, output.h) = Screen.WorldSizeToScreen(size);

    // This function calleth not upon SDL_RenderPresent, such that a plethora of objects may be drawn in just one instant, whenceafter SDL_RenderPresent shall be invoked.
    public void Draw(Window win)
    {
        if(!enabled)
            return;

        win.colors[type].GetRgb(out u8 r, out u8 g, out u8 b);

        SDL_Rect output = this.output;
        output.x -= win.screenLoc.x;
        output.y -= win.screenLoc.y;

        SDL_SetRenderDrawColor(win.sdlRend, r, g, b, 0xff).ThrowSdlErr();
        SDL_RenderFillRect(win.sdlRend, ref output).ThrowSdlErr();
    }
}