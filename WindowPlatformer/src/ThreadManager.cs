using System;
using System.Collections.Concurrent;

namespace src;

public static class ThreadManager
{
    private const i32 WINDOW_THREAD_COUNT = 2;

    private static bool initiated;
    private static WindowThread[] windowThreads;
    private static i32 activeWindowThreadIndex;
    private static readonly ConcurrentQueue<(Action action, Ref<bool> completed)> mainThreadRequests = [];


    public static bool isRunning { get; private set; }
    public static bool runWindowThreads { get; private set; }
    public static WindowThread activeWindowThread => windowThreads[activeWindowThreadIndex];


    public static void Init()
    {
        if(initiated || isRunning)
        {
            LogError("Cannot call Init multiple times or while already running");
            return;
        }

        SDL_Init(SDL_INIT_VIDEO).ThrowSdlError();

        Screen.Init();
        Input.Init();

        runWindowThreads = true;
        windowThreads = new WindowThread[WINDOW_THREAD_COUNT];
        for(i32 i = 0; i < WINDOW_THREAD_COUNT; i++)
        {
            windowThreads[i] = new();
            windowThreads[i].Start();
        }

        initiated = true;
    }

    public static void Run()
    {
        if(!initiated || isRunning)
        {
            LogError("Cannot call Run before Init or while already running");
            return;
        }

        isRunning = true;

        activeWindowThread.Resume();

        u32 lastFrame = SDL_GetTicks();
        f32 deltaTime;

        while(isRunning)
        {
            if(WindowEngine.isBusy)
            {
                SDL_Delay(1);
                continue;
            }

            u32 now = SDL_GetTicks();
            deltaTime = (now - lastFrame) / 1000f;
            lastFrame = now;

            Input.Tick();

            PlayerController.Tick(deltaTime);

            while(mainThreadRequests.TryDequeue(out (Action action, Ref<bool> completed) request))
            {
                request.action?.Invoke();
                request.completed?.Set(true);
            }

            if(Input.KeyDown(Key.R))
                LevelManager.ReloadLevel();

            foreach(Window win in WindowEngine.windows)
                Renderer.DrawWindow(win);

            SDL_Delay(1);
        }
    }

    public static void Quit()
    {
        isRunning = false;
        runWindowThreads = false;
    }

    public static void RunOnWindowThread(Action action, bool waitUntilCompleted)
    {
        Ref<bool> completed = activeWindowThread.EnqueueTask(action);

        if(waitUntilCompleted)
            SDL_WaitUntil(completed.Get);
    }

    public static void RunOnMainThread(Action action, bool waitUntilCompleted)
    {
        Ref<bool> completed = new(false);
        mainThreadRequests.Enqueue((action, completed));

        if(waitUntilCompleted)
            SDL_WaitUntil(completed.Get);
    }

    public static void CycleWindowThread()
    {
        activeWindowThread.Pause();
        activeWindowThreadIndex = (activeWindowThreadIndex + 1) % WINDOW_THREAD_COUNT;
        activeWindowThread.Resume();
    }
}