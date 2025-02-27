using System.Collections.Generic;
using System.Linq;
using WindowPlatformer;

namespace src;

public static class WindowEngine
{
    private const i32 WINDOW_ANIM_DT = 5;

    public static readonly List<Window> windows = [];


    public static bool isBusy { get; private set; } = false;


    public static Window CreateWindow(WindowData data)
    {
        isBusy = true;

        Window win = null;
        ThreadManager.RunOnEventThreadAndWait(() => win = new(data), true);

        windows.Add(win);

        isBusy = false;
        return win;
    }

    public static Window[] CreateWindows(WindowData[] data, bool entryAnim = true)
    {
        isBusy = true;

        Window[] wins = new Window[data.Length];
        ThreadManager.RunOnEventThreadAndWait(() =>
        {
            for(i32 i = 0; i < data.Length; i++)
            {
                if(entryAnim)
                {
                    WindowData d = data[i];
                    V2f loc = d.entryDir == V2f.zero ? d.loc : d.loc - d.entryDir;
                    wins[i] = new(d.title, loc, d.size, d.movable, d.resizable, new(d.color), loc);
                }
                else
                    wins[i] = new(data[i]);
            }
        }, true);

        if(entryAnim)
        {
            const u32 ANIM_TIME = 450;
            u32 animStart = SDL_GetTicks();
            V2f[] startPos = wins.Select(w => w.worldLoc).ToArray();
            while(SDL_GetTicks() - animStart is u32 timePassed && timePassed < ANIM_TIME)
            {
                f32 t = Easing.Out.Cube((f32)timePassed / ANIM_TIME);

                for(i32 i = 0; i < data.Length; i++)
                    if(data[i].entryDir != V2f.zero)
                    {
                        wins[i].worldLoc = V2f.Lerp(startPos[i], data[i].loc, t);
                        wins[i].UpdateWindowPos();
                        Renderer.DrawWindow(wins[i]);
                    }

                SDL_Delay(WINDOW_ANIM_DT);
            }
        }

        for(i32 i = 0; i < data.Length; i++)
        {
            wins[i].worldLoc = data[i].loc;
            wins[i].UpdateWindowPos();
        }

        windows.AddRange(wins);

        isBusy = false;
        return wins;
    }

    public static void DestroyWindow(Window win)
    {
        isBusy = true;

        windows.Remove(win);

        ThreadManager.RunOnEventThreadAndWait(win.Destroy, true);

        isBusy = false;
    }

    public static void DestroyAllWindows(bool exitAnim = true)
    {
        isBusy = true;

        if(exitAnim)
        {
            const u32 ANIM_TIME = 450;
            u32 animStart = SDL_GetTicks();
            V2f[] startPos = windows.Select(w => w.worldLoc).ToArray();
            foreach(var s in startPos)
                s.Log();
            while(SDL_GetTicks() - animStart is u32 timePassed && timePassed < ANIM_TIME)
            {
                f32 t = Easing.In.Cube((f32)timePassed / ANIM_TIME);

                for(i32 i = 0; i < startPos.Length; i++)
                {
                    windows[i].worldLoc = V2f.Lerp(startPos[i], windows[i].exitPos, t);
                    windows[i].UpdateWindowPos();
                    Renderer.DrawWindow(windows[i]);
                }

                SDL_Delay(WINDOW_ANIM_DT);
            }
        }

        ThreadManager.RunOnEventThreadAndWait(() =>
        {
            foreach(Window win in windows)
                win.Destroy();
        }, true);

        windows.Clear();

        isBusy = false;
    }

    public static Window GetWindowFromId(u32 id)
        => windows.Find(w => w.id == id);
}