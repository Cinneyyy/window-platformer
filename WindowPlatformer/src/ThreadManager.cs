using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace src;

public static class ThreadManager
{
    private static volatile bool isRunning;
    private static readonly Thread eventThread = new(EventThread);
    private static readonly ConcurrentQueue<(Action action, TaskCompletionSource tcs)> eventThreadRequests = [];
    private static readonly ConcurrentQueue<(Action act, TaskCompletionSource tcs)> urgentEventThreadRequests = [];
    private static bool eventThreadReady;


    public static void Run()
    {
        if(isRunning)
        {
            LOG_INFO.Error($"Cannot call {nameof(Run)} multiple times.");
            return;
        }

        isRunning = true;
        eventThread.Start();

        while(isRunning)
        {
            if(WindowEngine.isBusy || !eventThreadReady)
            {
                SDL_Delay(1);
                continue;
            }

            SDL_Delay(1);
        }
    }

    public static void RunOnEventThread(Action action, bool ignoreBusyState = false)
        => (ignoreBusyState ? urgentEventThreadRequests : eventThreadRequests).Enqueue((action, null));

    public static async Task RunOnEventThreadAsync(Action action, bool ignoreBusyState = false)
    {
        TaskCompletionSource tcs = new();

        (ignoreBusyState ? urgentEventThreadRequests : eventThreadRequests).Enqueue((action, tcs));

        await tcs.Task;
    }


    private static void EventThread()
    {
        SDL_Init(SDL_INIT_VIDEO).ThrowSdlError();
        SDL_AddEventWatch(EventWatch, nint.Zero);

        Screen.Init();

        eventThreadReady = true;

        while(isRunning)
        {
            while(SDL_PollEvent(out _) == 1)
                continue;

            while(urgentEventThreadRequests.TryDequeue(out (Action action, TaskCompletionSource tcs) req))
            {
                req.action?.Invoke();
                req.tcs?.SetResult();
            }

            while(!WindowEngine.isBusy && eventThreadRequests.TryDequeue(out (Action action, TaskCompletionSource tcs) req))
            {
                req.action?.Invoke();
                req.tcs?.SetResult();
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
            default:
                return 1;
        }

        return 0;
    }
}