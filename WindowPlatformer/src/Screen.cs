using src.Debugging;
using src.Utility;

namespace src;

public static class Screen
{
    public static V2i size { get; private set; }
    public static i32 whDelta { get; private set; }


    public static V2i WorldPointToScreen(V2f pt)
        => new(
            ((pt.x + 1f) / 2f * size.y + whDelta).Floor(),
            ((-pt.y + 1f) / 2f * size.y).Floor()
        );

    public static V2i WorldSizeToScreen(V2f sz)
        => new(
            (sz.x / 2f * size.y).Floor(),
            (sz.y / 2f * size.y).Floor()
        );

    public static V2f WorldPointFromScreen(V2i pt)
        => new(
            (f32)(pt.x - whDelta) / size.y * 2f - 1f,
            -((f32)pt.y / size.y * 2f - 1f)
        );

    public static V2f WorldSizeFromScreen(V2i sz)
        => new(
            (f32)sz.x / size.y * 2f,
            (f32)sz.y / size.y * 2f
        );


    internal static void Init()
    {
        SDL_GetDisplayBounds(0, out SDL_Rect bounds).ThrowSdlError();
        size = new(bounds.w, bounds.h);
        whDelta = (size.x - size.y) / 2;
    }
}