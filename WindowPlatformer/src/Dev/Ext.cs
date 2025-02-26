using System;
using System.Collections;
using System.Collections.Generic;

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
}