using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using src.Gui;

namespace src.LevelSystem;

public static class LevelManager
{
    public const bool LOADING_ANIMATIONS = true;
    public const i32 LEVEL_COUNT = 3;


    public static readonly LevelData[] levelList =
        Enumerable.Range(0, LEVEL_COUNT)
        .Select(i => $"res/levels/{i}.lvl")
        .Select(LevelReader.ReadFile)
        .ToArray();


    public static bool isBusy { get; private set; }
    public static LevelData? loadedLevel { get; private set; } = null;
    public static LevelData? lastLoadedLevel { get; private set; } = null;
    public static List<Window> windows => WindowManager.windows;
    public static List<GameObject> objs => GameObjectManager.objs;
    public static bool isLevelLoaded => loadedLevel is not null;
    public static bool ready => !isBusy && isLevelLoaded;


    public static void LoadLevel(LevelData data)
    {
        if(isBusy|| isLevelLoaded)
            throw new("Cannot load level while busy or one is already loaded.");

        isBusy = true;

        GameObjectManager.CreateMany(data.objects);
        ThreadManager.CycleWindowThread();
        WindowManager.CreateWindows(data.windows, LOADING_ANIMATIONS);

        PlayerController.OnLevelLoaded();

        loadedLevel = data;
        isBusy = false;
    }

    public static void UnloadLevel()
    {
        if(isBusy || !isLevelLoaded)
            throw new($"Cannot unload level while busy or none is loaded.");

        isBusy = true;

        WindowManager.DestroyAllWindows(LOADING_ANIMATIONS);
        GameObjectManager.DestroyAll();

        lastLoadedLevel = loadedLevel;
        loadedLevel = null;
        isBusy = false;
    }

    public static void ReloadLevel(bool hardReload = false)
    {
        if(isBusy || !isLevelLoaded)
            throw new($"Cannot reload level while busy or none is loaded");

        UnloadLevel();

        if(hardReload)
            LoadLevel(Array.Find(levelList, l => Path.GetFullPath(l.filePath) == Path.GetFullPath(((LevelData)lastLoadedLevel).filePath)));
        else
            LoadLevel((LevelData)lastLoadedLevel);
    }

    public static void AdvanceLevel()
    {
        if(isBusy || !isLevelLoaded)
            throw new($"Cannot advance level while busy or none is loaded");

        UnloadLevel();

        i32 lastIndex = Array.IndexOf(levelList, (LevelData)lastLoadedLevel);

        if(lastIndex == levelList.Length-1)
            MainMenu.Load();
        else
            LoadLevel(levelList[lastIndex+1]);
    }

    public static void RereadLevelList()
    {
#if DEBUG
        const string RES_PATH = "../../../res";

        if(Directory.Exists(RES_PATH))
        {
            foreach(string dir in Directory.GetDirectories(RES_PATH, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(Path.GetRelativePath(RES_PATH, dir));

            foreach(string res in Directory.GetFiles(RES_PATH, "*", SearchOption.AllDirectories))
                File.Copy(res, "res/" + Path.GetRelativePath(RES_PATH, res), true);
        }
#endif

        for(i32 i = 0; i < LEVEL_COUNT; i++)
            levelList[i] = LevelReader.ReadFile($"res/levels/{i}.lvl");
    }
}