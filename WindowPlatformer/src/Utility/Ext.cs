using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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

    public static string ToStringFromCollection<T>(this IEnumerable<T> collection)
    {
        Func<T, string> toString = typeof(T).ImplementsInterface(typeof(IEnumerable<>)) ?
            t => (string)typeof(Ext).GetMethod(nameof(ToStringFromCollection)).MakeGenericMethod(typeof(T).GenericTypeArguments[0]).Invoke(null, [t]) :
            t => t.ToString();

        StringBuilder sb = new("[");

        foreach(T item in collection)
        {
            if(typeof(T) == typeof(string))
            {
                sb.Append('"');
                sb.Append(toString(item));
                sb.Append('"');
            }
            else
                sb.Append(toString(item));

            sb.Append(", ");
        }

        if(collection.Any())
            sb.Remove(sb.Length - 2, 2);

        sb.Append(']');

        return sb.ToString();
    }

    public static bool ImplementsInterface(this Type type, Type interfaceType)
        => type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType);

    /// <summary>Convert o to a string, but format it properly incase it is an IEnumerable</summary>
    public static string ToStringCatchCollection(this object o)
    {
        Log(o.GetType().FullName);

        if(o.GetType().ImplementsInterface(typeof(IEnumerable<>)))
        {
            MethodInfo toString = typeof(Ext).GetMethod(nameof(ToStringFromCollection));
            Type ienumerableType = o.GetType().GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)).First();
            Type ienumerableT = ienumerableType.GetGenericArguments().First();

            return toString.MakeGenericMethod(ienumerableT).Invoke(null, [o]) as string;
        }
        else
            return o.ToString();
    }
}