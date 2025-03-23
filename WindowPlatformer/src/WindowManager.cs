using System.Collections.Generic;
using System.Linq;
using src.LevelSystem;

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

        Window[] wins = new Window[data.Length];
        ThreadManager.RunOnWindowThread(() =>
        {
            for(i32 i = 0; i < data.Length; i++)
            {
                if(entryAnim)
                {
                    WindowData d = data[i];
                    V2f loc = d.loc - d.entryDir;
                    V2f size = d.size * d.entrySize;
                    wins[i] = new(d.title, loc, size, d.movable, d.resizable, new(d.color), loc, size);
                    Renderer.DrawObjects(wins[i], Screen.WorldPointToScreen(data[i].loc + new V2f(-size.x/2f, size.y/2f)));
                }
                else
                    wins[i] = new(data[i]);
            }
        }, true);

        if(entryAnim)
        {
            u32 animStart = SDL_GetTicks();
            V2f[] startPos = wins.Select(w => w.worldLoc).ToArray();
            V2f[] startSize = wins.Select(w => w.worldSize).ToArray();

            while(SDL_GetTicks() - animStart is u32 timePassed && timePassed < ENTRY_ANIM_TIME)
            {
                f32 t = Easing.Out.Cube((f32)timePassed / ENTRY_ANIM_TIME);

                for(i32 i = 0; i < data.Length; i++)
                {
                    if(data[i].entrySize == V2f.one && data[i].entryDir == V2f.zero)
                        continue;

                    if(data[i].entrySize != V2f.one)
                    {
                        wins[i].worldSize = V2f.Lerp(startSize[i], data[i].size, t);
                        wins[i].worldLoc = wins[i].worldLoc;
                        wins[i].UpdateWindowSize();
                    }

                    if(data[i].entryDir != V2f.zero)
                        wins[i].worldLoc = V2f.Lerp(startPos[i], data[i].loc, t);

                    wins[i].UpdateWindowPos();
                    Renderer.DrawWindow(wins[i], Screen.WorldPointToScreen(data[i].loc + new V2f(-wins[i].worldSize.x/2f, wins[i].worldSize.y/2f)));
                }

                SDL_Delay(WINDOW_ANIM_DT);
            }
        }

        for(i32 i = 0; i < data.Length; i++)
        {
            wins[i].worldSize = data[i].size;
            wins[i].worldLoc = data[i].loc;

            wins[i].UpdateWindowPos();

            if(data[i].entrySize != V2f.one)
            {
                wins[i].UpdateWindowSize();
                wins[i].RecreateRenderer();
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
                    windows[i].worldSize = V2f.Lerp(startSize[i], windows[i].exitSize, t);
                    windows[i].worldLoc = V2f.Lerp(startPos[i], windows[i].exitLoc, t);

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