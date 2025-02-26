using System.Collections.Generic;

namespace src;

public static class LevelManager
{
    public static bool isBusy { get; private set; }
    public static LevelData? loadedLevel { get; private set; } = null;
    public static LevelData? lastLoadedLevel { get; private set; } = null;
    public static List<Window> windows => WindowEngine.windows;
    public static List<GameObject> objs => GameObjectManager.objs;
    public static bool isLevelLoaded => loadedLevel is not null;
    public static bool ready => !isBusy && isLevelLoaded;


    public static void LoadLevel(LevelData data)
    {
        if(isBusy || isLevelLoaded)
            throw new("Cannot load level while busy or one is already loaded.");

        isBusy = true;

        WindowEngine.CreateWindows(data.windows);
        GameObjectManager.CreateMany(data.objects);

        PlayerController.OnLevelLoaded();

        loadedLevel = data;
        isBusy = false;
    }

    public static void UnloadLevel()
    {
        if(isBusy || !isLevelLoaded)
            throw new($"Cannot unload level while busy or none is loaded.");

        isBusy = true;

        WindowEngine.DestroyAllWindows();
        GameObjectManager.DestroyAll();

        lastLoadedLevel = loadedLevel;
        loadedLevel = null;
        isBusy = false;
    }

    public static void ReloadLevel()
    {
        if(isBusy || !isLevelLoaded)
            throw new($"Cannot reload level while busy or none is loaded");

        UnloadLevel();
        LoadLevel((LevelData)lastLoadedLevel);
    }
}