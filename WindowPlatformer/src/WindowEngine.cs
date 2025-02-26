using System.Collections.Generic;

namespace src;

public static class WindowEngine
{
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

    public static Window[] CreateWindows(WindowData[] data)
    {
        isBusy = true;

        Window[] wins = new Window[data.Length];
        ThreadManager.RunOnEventThreadAndWait(() =>
        {
            for(i32 i = 0; i < data.Length; i++)
                wins[i] = new(data[i]);
        }, true);

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

    public static void DestroyAllWindows()
    {
        isBusy = true;

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