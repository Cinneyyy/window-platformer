using System;
using System.Threading;

namespace src;

internal class Program
{
    static bool running = true;
    static nint rend, win;


    static void Poll()
    {
        while(SDL_PollEvent(out SDL_Event evt) == 1)
        {
            if(evt.type == SDL_EventType.SDL_QUIT)
                running = false;
            else if(evt.type == SDL_EventType.SDL_KEYDOWN)
                evt.key.keysym.sym.Log();
        }
    }

    static void Iter()
    {
        SDL_SetRenderDrawColor(rend, 0, 0, 0, 0).ThrowSdlError();
        SDL_RenderClear(rend).ThrowSdlError();

        SDL_Rect rect = new()
        {
            w = 100,
            h = 100,
            x = (i32)(MathF.Cos(SDL_GetTicks() / 1000f) * 200 + 350),
            y = (i32)(MathF.Sin(SDL_GetTicks() / 1000f) * 200 + 250)
        };
        SDL_SetRenderDrawColor(rend, 0xff, 0, 0, 0xff).ThrowSdlError();
        SDL_RenderFillRect(rend, ref rect).ThrowSdlError();

        SDL_RenderPresent(rend);

        SDL_Delay(1);
    }


    private static void Main(string[] args)
    {
        bool ready = false;

        Thread thread = new(() =>
        {
            SDL_Init(SDL_INIT_VIDEO).ThrowSdlError();

            win = SDL_CreateWindow("wild", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, 800, 600, SDL_WindowFlags.SDL_WINDOW_SHOWN);

            rend = SDL_CreateRenderer(win, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

            ready = true;

            while(running)
                Poll();
        });

        thread.Start();

        while(!ready)
            continue;

        while(running)
        {
            Iter();
        }
    }
}
