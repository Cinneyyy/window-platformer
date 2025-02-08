using System;
using System.Collections.Generic;
using System.Linq;

namespace src;

public static class Ext
{
    public static bool LogSdlErr(this i32 sdlCode)
    {
        if(sdlCode < 0)
        {
            Console.WriteLine($"SDL Error: {SDL_GetError()}");
            return true;
        }

        return false;
    }
    public static void ThrowSdlErr(this i32 sdlCode)
    {
        if(sdlCode < 0)
            throw new(SDL_GetError());
    }

    public static i32 Round(this f32 f) => (i32)MathF.Round(f);
    public static i32 Floor(this f32 f) => (i32)MathF.Floor(f);
    public static i32 Ceil(this f32 f) => (i32)MathF.Ceiling(f);

    public static SDL_bool ToSdlBool(this bool b) => b ? SDL_bool.SDL_TRUE : SDL_bool.SDL_FALSE;

    public static T FirstWhere<T>(this IEnumerable<T> coll, Func<T, bool> predicate)
        => coll.Where(predicate).First();
    public static T? FirstOrDefaultWhere<T>(this IEnumerable<T> coll, Func<T, bool> predicate)
        => coll.Where(predicate).FirstOrDefault();
}