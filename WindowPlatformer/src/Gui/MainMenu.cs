using src.LevelSystem;
using src.Utility;

namespace src.Gui;

public static class MainMenu
{
    private static readonly LevelData levelData = LevelReader.ReadFile("res/levels/main_menu.lvl");


    public static bool isActive { get; private set; }


    public static void Load()
    {
        if(LevelManager.isLevelLoaded)
            LevelManager.UnloadLevel();

        LevelManager.LoadLevel(levelData);

        ThreadManager.dynamicTick += Tick;
        isActive = true;
    }

    public static void Unload()
    {
        ThreadManager.dynamicTick -= Tick;
        isActive = false;
        LevelManager.UnloadLevel();
    }

    public static void OnQuit()
    {
        WindowManager.exitAnimTime = 1000;
        Unload();
        ThreadManager.Quit();
    }

    public static void OnBegin()
    {
        Unload();
        LevelManager.LoadLevel(LevelManager.levelList[0]);
    }


    private static void Tick()
    {
        if(Input.KeyDown(Key.Lmb))
        {
            foreach(Window win in WindowManager.windows)
            {
                (i32 x, i32 y, i32 w, i32 h) rect = (win.screenLoc.x, win.screenLoc.y, win.screenSize.x, win.screenSize.y);

                if(rect.Contains(Input.mousePosScreen))
                {
                    switch(SDL_GetWindowTitle(win.sdlWin))
                    {
                        case "Begin": OnBegin(); break;
                        case "Quit": OnQuit(); break;
                        default: break;
                    }

                    break;
                }
            }
        }
    }
}