using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace src.Dev;

public static class Ext
{
    public static TOut Pipe<TIn, TOut>(this TIn arg, Func<TIn, TOut> func)
        => func(arg);

    public static IEnumerable<T> Do<T>(this IEnumerable<T> collection, Action<T> action)
    {
        foreach(T t in collection)
            action(t);

        return collection;
    }

    public static i32 Round(this f32 f) => (i32)MathF.Round(f);
    public static i32 Floor(this f32 f) => (i32)MathF.Floor(f);
    public static i32 Ceil(this f32 f) => (i32)MathF.Ceiling(f);

    public static V2i GetLoc(this SDL_Rect rect) => new(rect.x, rect.y);
    public static V2i GetSize(this SDL_Rect rect) => new(rect.w, rect.h);

    public static SDL_bool ToSdlBool(this bool b) => b ? SDL_bool.SDL_TRUE : SDL_bool.SDL_FALSE;
}