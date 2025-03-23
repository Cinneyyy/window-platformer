using System.Collections.Generic;
using System.Linq;
using src.LevelSystem;
using src.Utility;

namespace src;

public static class WindowManager
{
    private const i32 WINDOW_ANIM_DT = 1;
    private const i32 ENTRY_ANIM_TIME = 375;
    private const i32 EXIT_ANIM_TIME = 300;

    public static readonly List<Window> windows = [];


    public static bool isBusy { get; private set; } = false;


    public static Window[] CreateWindows(WindowData[] data, bool entryAnim = true)
    {
        isBusy = true;

        Window[] wins = null;
        ThreadManager.RunOnWindowThread(() => wins = data.Select(d => new Window(d)).ToArray(), true);

        foreach(Window win in wins)
            Renderer.DrawWindow(win);

        if(entryAnim)
        {
            u32 startTime = SDL_GetTicks();

            while(SDL_GetTicks() - startTime < ENTRY_ANIM_TIME)
            {
                f32 t = Easing.Out.Cube((f32)(SDL_GetTicks() - startTime) / ENTRY_ANIM_TIME);

                for(i32 i = 0; i < wins.Length; i++)
                {
                    Window win = wins[i];
                    WindowData dat = data[i];

                    win.worldSize = V2f.Lerp(dat.entrySize, dat.size, t);
                    win.worldLoc = V2f.Lerp(dat.entryLoc, dat.loc, t);

                    win.UpdateWindowPos();
                    ThreadManager.RunOnWindowThread(win.UpdateWindowSize, true);

                    if(win.entryRedraw)
                        Renderer.DrawWindow(win);
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
                ThreadManager.RunOnWindowThread(win.UpdateWindowSize, true);

                Renderer.DrawWindow(win);
            }
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
            u32 animStart = SDL_GetTicks();
            V2f[] startPos = windows.Select(w => w.worldLoc).ToArray();
            V2f[] startSize = windows.Select(w => w.worldSize).ToArray();

            while(SDL_GetTicks() - animStart is u32 timePassed && timePassed < EXIT_ANIM_TIME)
            {
                f32 t = Easing.In.Cube((f32)timePassed / EXIT_ANIM_TIME);

                for(i32 i = 0; i < startPos.Length; i++)
                {
                    windows[i].worldSize = V2f.Lerp(startSize[i], windows[i].entrySize, t);
                    windows[i].worldLoc = V2f.Lerp(startPos[i], windows[i].entryLoc, t);

                    windows[i].UpdateWindowSize();
                    windows[i].UpdateWindowPos();
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