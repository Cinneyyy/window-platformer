using System.Collections.Generic;
using System.Threading.Tasks;

namespace src;

public static class LevelManager
{
    public static bool isBusy { get; private set; }
    public static LevelData? loadedLevel { get; private set; } = null;
    public static LevelData? lastLoadedLevel { get; private set; } = null;
    public static List<Window> windows => WindowEngine.windows;
    public static List<GameObject> objs => GameObjectManager.objs;
    public static bool isLevelLoaded => loadedLevel is not null;


    public static async Task LoadLevelAsync(LevelData data)
    {
        if(isBusy || isLevelLoaded)
            throw new("Cannot load level while busy or one is already loaded.");

        isBusy = true;

        await WindowEngine.CreateWindowsAsync(data.windows);
        GameObjectManager.CreateMany(data.objs);

        loadedLevel = data;
        isBusy = false;
    }

    public static async Task UnloadLevelAsync()
    {
        if(isBusy || !isLevelLoaded)
            throw new($"Cannot unload level while busy or none is loaded.");

        isBusy = true;

        await WindowEngine.DestroyAllWindowsAsync();
        GameObjectManager.DestroyAll();

        lastLoadedLevel = loadedLevel;
        loadedLevel = null;
        isBusy = false;
    }

    public static async Task ReloadLevelAsync()
    {
        if(isBusy || !isLevelLoaded)
            throw new($"Cannot reload level while busy or none is loaded");

        await UnloadLevelAsync();
        await LoadLevelAsync((LevelData)lastLoadedLevel);
    }
}