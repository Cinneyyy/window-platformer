using System;
using System.Threading;

namespace src;

public static class Application
{
    private static bool initiated, isRunning;
    private static u32 lastFrame;
    private static f32 deltaTime, totalTime;
    private static Thread? gameThread;


    public static V2i screenSize { get; private set; }
    public static i32 screenWhDelta { get; private set; }


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


    private static unsafe i32 EventWatch(nint data, nint evtPtr)
    {
        SDL_Event *evt = (SDL_Event*)evtPtr;
        Console.WriteLine(evt->type);
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

            SDL_Delay(1);
        }
    }
}