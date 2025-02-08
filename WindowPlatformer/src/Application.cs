using System.Threading;

namespace src;

public static class Application
{
    public delegate void Tick(float dt);


    public static event Tick? tick;

    private static bool initiated, isRunning;
    private static u32 lastFrame;
    private static Thread? gameThread;


    public static f32 deltaTime { get; private set; }
    public static f32 totalTime { get; private set; }


    public static void Init()
    {
        if(initiated)
            throw new("Cannot initiate application multiple times.");

        SDL_Init(SDL_INIT_VIDEO).ThrowSdlErr();
        SDL_AddEventWatch(EventWatch, nint.Zero);

        Screen.Init();

        initiated = true;
    }

    public static void Run()
    {
        if(isRunning)
            throw new("Cannot start main loop, while it is already active.");

        isRunning = true;

        lastFrame = 0;
        deltaTime = 0f;
        totalTime = 0f;

        gameThread = new Thread(GameThreadRun);
        gameThread.Start();

        while(isRunning)
            while(SDL_PollEvent(out _) > 0)
                continue;

        SDL_Quit();
    }

    public static void Quit()
        => isRunning = false;


    private static unsafe i32 EventWatch(nint data, nint evtPtr)
    {
        SDL_Event *evt = (SDL_Event*)evtPtr;

        switch(evt->type)
        {
            case SDL_EventType.SDL_QUIT:
            {
                if(GameState.isLevelLoaded)
                {
                    GameState.UnloadCurrentLevel();
                    // TODO: load main menu
                    Quit(); // temp.
                }
                else
                    Quit();

                break;
            }
            case SDL_EventType.SDL_WINDOWEVENT:
            {
                if(!GameState.isLevelLoaded)
                    break;

                Window? win = GameState.TryGetWindowFromId(evt->window.windowID);

                if(win is null)
                    break;

                if(evt->window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_MOVED)
                    win.SetScreenLoc(new(evt->window.data1, evt->window.data2));
                else if(evt->window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED)
                    win.SetScreenSize(new(evt->window.data1, evt->window.data2));

                break;
            }
        }

        return 0;
    }

    private static void GameThreadRun()
    {
        while(isRunning)
        {
            u32 now = SDL_GetTicks();
            deltaTime = (now - lastFrame) / 1000f;
            totalTime = now / 1000f;
            lastFrame = now;

            tick?.Invoke(deltaTime);

            if(GameState.isLevelLoaded)
                foreach(Window w in GameState.windows!)
                    w.Redraw();

            SDL_Delay(1);
        }
    }
}