using src.Debugging;
using src.LevelSystem;
using src.Utility;

namespace src;

public class Window
{
    internal Window(string title, V2f loc, V2f size, bool movable, bool resizable, ColorPalette colors, V2f entryLoc, V2f entrySize, bool entryRedraw, SDL_WindowFlags flags = 0)
    {
        this.colors = colors;
        this.movable = movable;
        this.resizable = resizable;
        this.entryLoc = entryLoc;
        this.entrySize = entrySize;
        this.entryRedraw = entryRedraw;

        worldSize = size;
        worldLoc = loc;

        flags |= movable ? 0 : SDL_WindowFlags.SDL_WINDOW_BORDERLESS;
        sdlWin = SDL_CreateWindow(title, screenLoc.x - screenSize.x/2, screenLoc.y + screenSize.y/2, screenSize.x, screenSize.y, flags);
        if(sdlWin == nint.Zero)
            ThrowSdlError("Failed to create window [@ Window.ctor]");

        sdlRend = SDL_CreateRenderer(sdlWin, -1, SDL_RendererFlags.SDL_RENDERER_SOFTWARE);
        if(sdlRend == nint.Zero)
            ThrowSdlError("Failed to create renderer [@ Window.ctor]");

        SDL_SetWindowResizable(sdlWin, resizable.ToSdlBool());
        id = SDL_GetWindowID(sdlWin);

        colors.background.GetRgb(out u8 r, out u8 g, out u8 b);
        SDL_SetRenderDrawColor(sdlRend, r, g, b, 0xff).ThrowSdlError();
        SDL_RenderClear(sdlRend).ThrowSdlError();
        SDL_RenderPresent(sdlRend);
    }

    internal Window(WindowData data, SDL_WindowFlags flags = 0)
        : this(data.title, data.loc, data.size, data.movable, data.resizable, new(data.color), data.entryLoc, data.entrySize, data.entryRedraw, flags)
    {}


    public readonly nint sdlWin, sdlRend;
    public readonly u32 id;
    public readonly bool movable, resizable;
    public readonly ColorPalette colors;
    public readonly V2f entryLoc, entrySize;
    public readonly bool entryRedraw;


    private V2f _worldLoc;
    /// <summary>Also sets screenLoc to the appropriate values</summary>
    public V2f worldLoc
    {
        get => _worldLoc;
        set
        {
            _worldLoc = value;
            _screenLoc = Screen.WorldPointToScreen(value + new V2f(-worldSize.x/2f, worldSize.y/2f));
        }
    }

    private V2f _worldSize;
    /// <summary>Also sets screenSize to the appropriate values</summary>
    public V2f worldSize
    {
        get => _worldSize;
        set
        {
            _worldSize = value;
            _screenSize = Screen.WorldSizeToScreen(value);
        }
    }

    private V2i _screenLoc;
    /// <summary>Also sets worldLoc to the appropriate values</summary>
    public V2i screenLoc
    {
        get => _screenLoc;
        set
        {
            _screenLoc = value;
            _worldLoc = Screen.WorldPointFromScreen(value) + new V2f(worldSize.x/2f, -worldSize.y/2f);
        }
    }

    private V2i _screenSize;
    /// <summary>Also sets worldSize with the appropriate values</summary>
    public V2i screenSize
    {
        get => _screenSize;
        set
        {
            _screenSize = value;
            _worldSize = Screen.WorldSizeFromScreen(value);
        }
    }


    public void UpdateWindowPos()
        => SDL_SetWindowPosition(sdlWin, screenLoc.x, screenLoc.y);

    public void UpdateWindowSize()
        => SDL_SetWindowSize(sdlWin, screenSize.x, screenSize.y);


    internal void Destroy()
    {
        SDL_DestroyRenderer(sdlRend);
        SDL_DestroyWindow(sdlWin);
    }
}