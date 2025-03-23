namespace src;

public static class Renderer
{
    public static void DrawAllWindows()
    {
        foreach(Window win in WindowManager.windows)
        {
            Clear(win);
            DrawWindowOverlaps(win);
            DrawObjects(win);
            Present(win);
        }
    }

    public static void DrawWindow(Window win)
    {
        Clear(win);
        DrawWindowOverlaps(win);
        DrawObjects(win);
        Present(win);
    }
    public static void DrawWindow(Window win, V2i falseLoc)
    {
        Clear(win);
        DrawWindowOverlaps(win);
        DrawObjects(win, falseLoc);
        Present(win);
    }

    public static void Clear(Window win)
    {
        win.colors.background.GetRgb(out u8 r, out u8 g, out u8 b);
        SDL_SetRenderDrawColor(win.sdlRend, r, g, b, 0xff).ThrowSdlError();
        SDL_RenderClear(win.sdlRend).ThrowSdlError();
    }

    public static void DrawWindowOverlaps(Window win)
    {
        foreach(Window other in WindowManager.windows)
        {
            if(win == other)
                continue;

            V2i overlapTl = new(
                i32.Max(win.screenLoc.x, other.screenLoc.x),
                i32.Max(win.screenLoc.y, other.screenLoc.y)
            );
            V2i overlapBr = new(
                i32.Min(win.screenLoc.x + win.screenSize.x, other.screenLoc.x + other.screenSize.x),
                i32.Min(win.screenLoc.y + win.screenSize.y, other.screenLoc.y + other.screenSize.y)
            );

            if(overlapTl.x < overlapBr.x && overlapTl.y < overlapBr.y)
            {
                V2i overlapSize = overlapBr - overlapTl;
                ColorUtility.MixColorLinear(win.colors.background, other.colors.background).GetRgb(out u8 r, out u8 g, out u8 b);

                SDL_Rect rect = new()
                {
                    w = overlapSize.x,
                    h = overlapSize.y
                };

                (rect.x, rect.y) = overlapTl - win.screenLoc;
                SDL_SetRenderDrawColor(win.sdlRend, r, g, b, 0xff).ThrowSdlError();
                SDL_RenderFillRect(win.sdlRend, ref rect).ThrowSdlError();
            }
        }
    }

    public static void DrawObjects(Window win, V2i falseLoc)
    {
        foreach(GameObject obj in GameObjectManager.objs)
        {
            win.colors[obj.type].GetRgb(out u8 r, out u8 g, out u8 b);
            SDL_SetRenderDrawColor(win.sdlRend, r, g, b, 0xff).ThrowSdlError();

            SDL_Rect output = obj.output;
            output.x -= falseLoc.x;
            output.y -= falseLoc.y;
            SDL_RenderFillRect(win.sdlRend, ref output).ThrowSdlError();
        }
    }
    public static void DrawObjects(Window win)
        => DrawObjects(win, win.screenLoc);

    public static void Present(Window win)
        => SDL_RenderPresent(win.sdlRend);
}