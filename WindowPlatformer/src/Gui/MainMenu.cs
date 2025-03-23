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

    public static void Quit()
    {
        ThreadManager.dynamicTick -= Tick;
        isActive = false;

        LevelManager.UnloadLevel();
        ThreadManager.Quit();
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
                        case "Begin": Begin(); break;
                        case "Quit": Quit(); break;
                        default: break;
                    }

                    break;
                }
            }
        }
    }

    private static void Begin()
    {
        ThreadManager.dynamicTick -= Tick;
        isActive = false;

        LevelManager.UnloadLevel();
        LevelManager.LoadLevel(LevelManager.levelList[0]);
    }
}