using System.Collections.Generic;
using System.Threading.Tasks;

namespace src;

public static class WindowEngine
{
    public static readonly List<Window> windows = [];


    public static bool isBusy { get; private set; } = false;


    public static async Task<Window> CreateWindowAsync(WindowData data)
    {
        isBusy = true;

        Window win = null;
        await ThreadManager.RunOnEventThreadAsync(() => win = new(data), true);

        windows.Add(win);

        isBusy = false;
        return win;
    }

    public static async Task<Window[]> CreateWindowsAsync(WindowData[] data)
    {
        isBusy = true;

        Window[] wins = new Window[data.Length];
        await ThreadManager.RunOnEventThreadAsync(() =>
        {
            for(i32 i = 0; i < data.Length; i++)
                wins[i] = new(data[i]);
        }, true);

        windows.AddRange(wins);

        isBusy = false;
        return wins;
    }

    public static async Task DestroyWindowAsync(Window win)
    {
        isBusy = true;

        windows.Remove(win);

        await ThreadManager.RunOnEventThreadAsync(win.Destroy, true);

        isBusy = false;
    }

    public static async Task DestroyAllWindowsAsync()
    {
        isBusy = true;

        await ThreadManager.RunOnEventThreadAsync(() =>
        {
            foreach(Window win in windows)
                win.Destroy();
        });

        windows.Clear();

        isBusy = false;
    }
}