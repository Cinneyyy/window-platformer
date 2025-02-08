using System;

namespace src;

public class Window : IDisposable
{
    public Window(string title, V2f loc, V2f size, bool movable, bool resizable, ColorPalette colors)
    {
        this.colors = colors;
        this.movable = movable;
        this.resizable = resizable;

        worldLoc = loc;
        worldSize = size;
        screenLoc = Screen.WorldPointToScreen(new(loc.x - size.x/2f, loc.y + size.y/2f));
        screenSize = Screen.WorldSizeToScreen(size);

        sdlWin = SDL_CreateWindow(title, screenLoc.x, screenLoc.y, screenSize.x, screenSize.y, movable ? SDL_WindowFlags.SDL_WINDOW_SHOWN : SDL_WindowFlags.SDL_WINDOW_BORDERLESS);
        if(sdlWin == nint.Zero)
            throw new(SDL_GetError());

        sdlRend = SDL_CreateRenderer(sdlWin, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
        if(sdlRend == nint.Zero)
            throw new(SDL_GetError());

        id = SDL_GetWindowID(sdlWin);

        SDL_SetWindowResizable(sdlWin, resizable.ToSdlBool());

        colors.background.GetRgb(out byte r, out byte g, out byte b);
        SDL_SetRenderDrawColor(sdlRend, r, g, b, 0xff).ThrowSdlErr();
        SDL_RenderClear(sdlRend).ThrowSdlErr();
        SDL_RenderPresent(sdlRend);
    }

    public Window(string title, V2f loc, V2f size, bool movable, bool resizable, u32 baseColor)
        : this(title, loc, size, movable, resizable, new ColorPalette(baseColor))
    {
    }

    public Window(in WindowData data)
        : this(data.title, data.loc, data.size, data.movable, data.resizable, data.color)
    {
    }


    ~Window() => (this as IDisposable).Dispose();


    public readonly nint sdlWin, sdlRend;
    public readonly u32 id;
    public readonly bool movable, resizable;
    public readonly ColorPalette colors;
    public V2f worldLoc, worldSize;
    public V2i screenLoc, screenSize;

    private bool disposed;


    public void Redraw()
    {
        colors.background.GetRgb(out byte r, out byte g, out byte b);
        SDL_SetRenderDrawColor(sdlRend, r, g, b, 0xff).ThrowSdlErr();
        SDL_RenderClear(sdlRend).ThrowSdlErr();

        foreach(GameObject o in GameState.objects!)
            o.Draw(this);

        GameState.player!.Draw(this);

        SDL_RenderPresent(sdlRend);
    }

    public void SetWorldLoc(V2f wPt)
    {
        worldLoc = wPt;
        screenLoc = Screen.WorldPointToScreen(wPt);
        SDL_SetWindowPosition(sdlWin, screenLoc.x, screenLoc.y);
    }

    public void SetWorldSize(V2f wSz)
    {
        worldSize = wSz;
        screenSize = Screen.WorldSizeToScreen(wSz);
        SDL_SetWindowSize(sdlWin, screenSize.x, screenSize.y);
    }

    public void SetScreenLoc(V2i sPt)
    {
        screenLoc = sPt;
        worldLoc = Screen.WorldPointFromScreen(sPt);
    }

    public void SetScreenSize(V2i sSz)
    {
        screenSize = sSz;
        worldSize = Screen.WorldSizeFromScreen(sSz);
    }

    public void SetTitle(string title)
        => SDL_SetWindowTitle(sdlWin, title);


    void IDisposable.Dispose()
    {
        if(disposed)
            return;

        disposed = true;
        GC.SuppressFinalize(this);

        SDL_DestroyRenderer(sdlRend);
        SDL_DestroyWindow(sdlWin);
    }
}