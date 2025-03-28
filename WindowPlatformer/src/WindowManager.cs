using System.Collections.Generic;
using System.Linq;
using src.LevelSystem;
using src.Utility;

namespace src;

public static class WindowManager
{
    private const i32 WINDOW_ANIM_DT = 1;

    public static readonly List<Window> windows = [];


    public static bool isBusy { get; private set; } = false;
    public static i32 entryAnimTime { get; set; } = 500;
    public static i32 exitAnimTime { get; set; } = 450;


    public static Window[] CreateWindows(WindowData[] data, bool entryAnim = true)
    {
        isBusy = true;

        Window[] wins = null;
        if(entryAnim)
            ThreadManager.RunOnWindowThread(() => wins = data.Select(d => new Window(d with { movable = false }, SDL_WindowFlags.SDL_WINDOW_HIDDEN)).ToArray(), true);
        else
            ThreadManager.RunOnWindowThread(() => wins = data.Select(d => new Window(d, 0)).ToArray(), true);

        foreach(Window win in wins)
            Renderer.DrawWindow(win);

        if(entryAnim)
        {
            foreach(Window win in wins)
            {
                win.worldLoc = win.entryLoc;
                win.UpdateWindowPos();
                SDL_ShowWindow(win.sdlWin);
            }

            u32 startTime = SDL_GetTicks();

            while(SDL_GetTicks() - startTime < entryAnimTime)
            {
                f32 t = Easing.Out.Cube((f32)(SDL_GetTicks() - startTime) / entryAnimTime);

                for(i32 i = 0; i < wins.Length; i++)
                {
                    Window win = wins[i];
                    WindowData dat = data[i];

                    win.worldSize = V2f.Lerp(dat.entrySize, dat.size, t);
                    win.worldLoc = V2f.Lerp(dat.entryLoc, dat.loc, t);

                    win.UpdateWindowPos();
                    win.UpdateWindowSize();

                    if(win.entryRedraw)
                        Renderer.DrawWindow(win);
                    else
                        Renderer.DrawWindow(win, Screen.WorldPointToScreen(dat.loc + new V2f(-dat.size.x/2f, dat.size.y/2f)));
                }

                SDL_Delay(WINDOW_ANIM_DT);
            }

            for(i32 i = 0; i < wins.Length; i++)
            {
                Window win = wins[i];
                WindowData dat = data[i];

                win.worldSize = dat.size;
                win.worldLoc = dat.loc;

                win.UpdateWindowPos();
                win.UpdateWindowSize();

                if(dat.movable)
                    SDL_SetWindowBordered(win.sdlWin, true.ToSdlBool());

                Renderer.DrawWindow(win);
            }

            SDL_RaiseWindow(wins.Last().sdlWin);
        }

        windows.AddRange(wins);

        isBusy = false;
        return wins;
    }

    public static void DestroyWindow(Window win)
    {
        isBusy = true;

        windows.Remove(win);
        win.Destroy();

        isBusy = false;
    }

    public static void DestroyAllWindows(bool exitAnim = true)
    {
        isBusy = true;

        if(exitAnim)
        {
            u32 startTime = SDL_GetTicks();
            (V2f size, V2f loc)[] startingValues = windows.Select(w => (w.worldSize, w.worldLoc)).ToArray();

            while(SDL_GetTicks() - startTime < exitAnimTime)
            {
                f32 t = Easing.In.Cube((f32)(SDL_GetTicks() - startTime) / exitAnimTime);

                for(i32 i = 0; i < windows.Count; i++)
                {
                    Window win = windows[i];

                    win.worldSize = V2f.Lerp(startingValues[i].size, win.entrySize, t);
                    win.worldLoc = V2f.Lerp(startingValues[i].loc, win.entryLoc, t);

                    win.UpdateWindowPos();
                    win.UpdateWindowSize();

                    if(win.entryRedraw)
                        Renderer.DrawWindow(win);
                }

                SDL_Delay(WINDOW_ANIM_DT);
            }
        }

        foreach(Window win in windows)
            win.Destroy();

        windows.Clear();

        isBusy = false;
    }

    public static Window GetWindowFromId(u32 id)
        => windows.Find(w => w.id == id);
}