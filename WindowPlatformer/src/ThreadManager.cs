using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace src;

public static class ThreadManager
{
    private static bool initiated;
    private static volatile bool isRunning;
    private static readonly Thread eventThread = new(EventThread);
    private static readonly ConcurrentQueue<(Action action, Ref<bool> completed)> eventThreadRequests = [];
    private static readonly ConcurrentQueue<(Action action, Ref<bool> completed)> urgentEventThreadRequests = [];
    private static bool eventThreadReady;
    private static bool eventThreadInitializationComplete;


    public static bool isOnEventThread => Thread.CurrentThread == eventThread;


    public static void Init()
    {
        if(initiated)
        {
            LOG_INFO.Error($"Cannot call {nameof(Init)} multiple times.");
            return;
        }

        if(isRunning)
        {
            LOG_INFO.Error($"Cannot call {nameof(Init)} after {nameof(Run)}.");
            return;
        }

        Input.Init();

        eventThread.Start();

        while(!eventThreadInitializationComplete)
            SDL_Delay(1);

        SDL_AddEventWatch(EventWatch, nint.Zero);

        initiated = true;
    }

    public static void Run()
    {
        if(!initiated)
        {
            LOG_INFO.Error($"Cannot call {nameof(Run)} before {nameof(Init)} was called.");
            return;
        }

        if(isRunning)
        {
            LOG_INFO.Error($"Cannot call {nameof(Run)} multiple times.");
            return;
        }

        isRunning = true;

        u32 lastFrame = SDL_GetTicks();
        f32 deltaTime = 0f;

        while(isRunning)
        {
            if(WindowEngine.isBusy || !eventThreadReady)
            {
                SDL_Delay(1);
                continue;
            }

            u32 now = SDL_GetTicks();
            deltaTime = (now - lastFrame) / 1000f;
            lastFrame = now;

            Input.Tick();

            PlayerController.Tick(deltaTime);

            if(Input.KeyDown(Key.R))
                LevelManager.ReloadLevel();

            foreach(Window win in WindowEngine.windows)
                Renderer.DrawWindow(win);

            SDL_Delay(1);
        }
    }

    public static void RunOnEventThread(Action action, bool ignoreBusyState = false)
    {
        if(isOnEventThread)
            action();
        else
            (ignoreBusyState ? urgentEventThreadRequests : eventThreadRequests).Enqueue((action, null));
    }

    public static void RunOnEventThreadAndWait(Action action, bool ignoreBusyState = false)
    {
        if(isOnEventThread)
            action();
        else
        {
            Ref<bool> completed = new(false);
            (ignoreBusyState ? urgentEventThreadRequests : eventThreadRequests).Enqueue((action, completed));

            while(!completed.value)
                SDL_Delay(1);
        }
    }


    private static void EventThread()
    {
        SDL_Init(SDL_INIT_VIDEO).ThrowSdlError();
        SDL_AddEventWatch(EventWatch, nint.Zero);

        Screen.Init();

        eventThreadInitializationComplete = true;

        // Wait until Run is called, as this already executes when Init is called
        while(!isRunning)
            SDL_Delay(1);

        eventThreadReady = true;

        while(isRunning)
        {
            while(SDL_PollEvent(out _) == 1)
                continue;

            while(urgentEventThreadRequests.TryDequeue(out (Action action, Ref<bool> completed) req))
            {
                req.action?.Invoke();
                req.completed?.Set(true);
            }

            while(!WindowEngine.isBusy && eventThreadRequests.TryDequeue(out (Action action, Ref<bool> completed) req))
            {
                req.action?.Invoke();
                req.completed?.Set(true);
            }

            SDL_Delay(1);
        }

        SDL_Quit();
    }

    private static unsafe i32 EventWatch(nint data, nint evtPtr)
    {
        SDL_Event* evt = (SDL_Event*)evtPtr;

        switch(evt->type)
        {
            case SDL_EventType.SDL_QUIT:
            {
                isRunning = false;
                break;
            }
            case SDL_EventType.SDL_WINDOWEVENT:
            {
                if(!LevelManager.ready)
                    break;

                Window win = WindowEngine.GetWindowFromId(evt->window.windowID);

                if(win is null)
                    break;

                switch(evt->window.windowEvent)
                {
                    case SDL_WindowEventID.SDL_WINDOWEVENT_MOVED:
                        win.screenLoc = new(evt->window.data1, evt->window.data2);
                        break;
                    case SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED:
                        win.screenSize = new(evt->window.data1, evt->window.data2);
                        break;
                    default:
                        break;
                }

                break;
            }
            default:
                return 1;
        }

        return 0;
    }
}