using System;

namespace src.Utility;

public static class GlobalUtility
{
    public static void SDL_WaitUntil(Func<bool> predicate, u32 delayMs = 1)
    {
        while(!predicate())
            SDL_Delay(delayMs);
    }
}