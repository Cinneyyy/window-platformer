using System;
using System.Linq;

namespace src;

public static class GameState
{
    public static LevelData? loadedLevel { get; private set; }
    public static Window[]? windows { get; private set; }
    public static GameObject[]? objects { get; private set; }
    public static GameObject? player { get; private set; }
    public static bool isLevelLoading { get; private set; }
    public static bool isLevelLoaded => loadedLevel is not null;


    public static void LoadLevel(in LevelData data)
    {
        if(isLevelLoaded || isLevelLoading)
            throw new("Cannot load multiple levels at once.");

        isLevelLoading = true;

        windows = new Window[data.windows.Length];
        for(i32 i = 0; i < windows.Length; i++)
            windows[i] = new(in data.windows[i]);

        objects = new GameObject[data.objects.Length];
        for(i32 i = 0; i < objects.Length; i++)
            objects[i] = new(in data.objects[i]);

        player = new(data.player);

        isLevelLoading = false;
        loadedLevel = data;
    }

    public static void UnloadCurrentLevel()
    {
        if(!isLevelLoaded)
            throw new("Cannot unload level while none is load");

        if(isLevelLoading)
            throw new("Cannot unload level while one is currently loading");

        loadedLevel = null;

        foreach(Window w in windows!)
            (w as IDisposable).Dispose();

        player = null;
        objects = null;
        windows = null;
    }

    public static Window GetWindowFromId(u32 id)
    {
        if(loadedLevel is null)
            throw new("Cannot get window while no level is loaded");

        return windows!.FirstWhere(w => w.id == id);
    }
    public static Window? TryGetWindowFromId(u32 id)
    {
        if(loadedLevel is null)
            return null;

        return windows!.FirstOrDefaultWhere(w => w.id == id);
    }

    public static void KillPlayer()
    {
        Console.WriteLine("Player dead :(");
        LevelData lv = loadedLevel!.Value;
        UnloadCurrentLevel();
        LoadLevel(lv);
    }

    public static void WinLevel()
    {
        Console.WriteLine("Player win :)");
        LevelData lv = loadedLevel!.Value;
        UnloadCurrentLevel();
        LoadLevel(lv);
    }
}