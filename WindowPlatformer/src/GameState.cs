using System;

namespace src;

public static class GameState
{
    public static LevelData? loadedLevel { get; private set; }
    public static Window[]? windows { get; private set; }
    public static GameObject[]? objects { get; private set; }
    public static GameObject? player { get; private set; }
    public static bool isLevelLoading { get; private set; }
    public static bool levelLoaded => loadedLevel is not null;


    public static void LoadLevel(in LevelData data)
    {
        if(loadedLevel is not null)
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
    }

    public static void UnloadCurrentLevel()
    {
        if(loadedLevel is null)
            throw new("Cannot unload level while none is load");

        foreach(Window w in windows!)
            (w as IDisposable).Dispose();

        player = null;
        objects = null;
        windows = null;
        loadedLevel = null;
    }
}