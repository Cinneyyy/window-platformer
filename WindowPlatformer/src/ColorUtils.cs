namespace src;

public static class ColorUtils
{
    public static u32 CombineRgb(u8 r, u8 g, u8 b)
        => 0xff000000 | ((u32)r << 16) | ((u32)g << 8) | b;
    public static u32 CombineArgb(u8 a, u8 r, u8 g, u8 b)
        => ((u32)a << 24) | ((u32)r << 16) | ((u32)g << 8) | b;

    public static u32 GetGray(u8 value)
        => CombineRgb(value, value, value);

    public static u32 MultiplyColor(this u32 col, f32 v)
    {
        col.GetRgb(out u8 r, out u8 g, out u8 b);

        return CombineArgb(
            col.GetAlpha(),
            (u8)(r * v),
            (u8)(g * v),
            (u8)(b * v)
        );
    }

    public static u32 InvertColor(this u32 color)
        => color ^ 0x00ffffff;

    public static void GetRgb(this u32 color, out u8 r, out u8 g, out u8 b)
    {
        r = (u8)(color >> 16);
        g = (u8)(color >> 8);
        b = (u8)color;
    }
    public static void GetArgb(this u32 color, out u8 a, out u8 r, out u8 g, out u8 b)
    {
        a = (u8)(color >> 24);
        r = (u8)(color >> 16);
        g = (u8)(color >> 8);
        b = (u8)color;
    }
    public static u8 GetAlpha(this u32 color)
        => (u8)(color >> 24);

    public static u32 MixColorLinear(this u32 c1, u32 c2)
    {
        c1.GetRgb(out u8 r1, out u8 g1, out u8 b1);
        c2.GetRgb(out u8 r2, out u8 g2, out u8 b2);

        return CombineRgb(
            (u8)((r1 + r2) / 2),
            (u8)((g1 + g2) / 2),
            (u8)((b1 + b2) / 2)
        );
    }
    public static u32 MixColorAdditive(this u32 c1, u32 c2)
    {
        c1.GetRgb(out u8 r1, out u8 g1, out u8 b1);
        c2.GetRgb(out u8 r2, out u8 g2, out u8 b2);

        return CombineRgb(
            (u8)i32.Clamp(r1 + r2, 0, 0xff),
            (u8)i32.Clamp(g1 + g2, 0, 0xff),
            (u8)i32.Clamp(b1 + b2, 0, 0xff)
        );
    }
}