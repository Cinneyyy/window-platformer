using System;
using System.Collections.Concurrent;
using System.Threading;

namespace src;

public class WindowThread
{
    public WindowThread()
    {
        thread = new(ThreadRun);
        waitHandle = new(false, EventResetMode.ManualReset);
        eventRequests = [];
    }


    private readonly Thread thread;
    private readonly EventWaitHandle waitHandle;
    private readonly ConcurrentQueue<(Action action, Ref<bool> completed)> eventRequests;


    public void Start()
        => thread.Start();

    public Ref<bool> EnqueueTask(Action action)
    {
        Ref<bool> completed = new(false);
        eventRequests.Enqueue((action, completed));
        return completed;
    }

    public void Pause()
        => waitHandle.Reset();

    public void Resume()
        => waitHandle.Set();


    private unsafe i32 EventWatch(nint data, nint evtPtr)
    {
        SDL_Event* evt = (SDL_Event*)evtPtr;

        switch(evt->type)
        {
            case SDL_EventType.SDL_QUIT:
            {
                ThreadManager.Quit();
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

    private void ThreadRun()
    {
        SDL_AddEventWatch(EventWatch, nint.Zero);

        Log("Ran thread " + thread.ManagedThreadId);
        while(ThreadManager.runWindowThreads)
        {
            waitHandle.WaitOne();

            while(SDL_PollEvent(out _) == 1)
                continue;

            while(eventRequests.TryDequeue(out (Action action, Ref<bool> completed) request))
            {
                request.action?.Invoke();
                request.completed?.Set(true);
            }

            SDL_Delay(1);
        }
    }
}