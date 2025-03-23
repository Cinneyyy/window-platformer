using System;
using System.Collections.Generic;

namespace src.Utility;

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

    public static void Populate<T>(this T[] arr, Func<i32, T> initialize)
    {
        for(i32 i = 0; i < arr.Length; i++)
            arr[i] = initialize(i);
    }

    public static bool Contains(this SDL_Rect rect, V2i pt)
        => pt.x >= rect.x && pt.x < (rect.x + rect.w) &&
           pt.y >= rect.y && pt.y < (rect.y + rect.h);
    public static bool Contains(this (f32 x, f32 y, f32 w, f32 h) rect, V2f pt)
        => pt.x >= rect.x && pt.x < (rect.x + rect.w) &&
           pt.y >= rect.y && pt.y < (rect.y + rect.h);
    public static bool Contains(this (i32 x, i32 y, i32 w, i32 h) rect, V2i pt)
        => pt.x >= rect.x && pt.x < (rect.x + rect.w) &&
           pt.y >= rect.y && pt.y < (rect.y + rect.h);
}