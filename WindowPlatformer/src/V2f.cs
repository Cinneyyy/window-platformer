namespace src;

public record struct V2f(f32 x, f32 y)
{
    public f32 x = x, y = y;


    public static V2f operator +(V2f a, V2f b) => new(a.x + b.x, a.y + b.y);
    public static V2f operator -(V2f a, V2f b) => new(a.x - b.x, a.y - b.y);
    public static V2f operator *(V2f a, V2f b) => new(a.x * b.x, a.y * b.y);
    public static V2f operator /(V2f a, V2f b) => new(a.x / b.x, a.y / b.y);
    public static V2f operator %(V2f a, V2f b) => new(a.x % b.x, a.y % b.y);

    public static V2f operator +(V2f v, f32 f) => new(v.x + f, v.y + f);
    public static V2f operator -(V2f v, f32 f) => new(v.x - f, v.y - f);
    public static V2f operator *(V2f v, f32 f) => new(v.x * f, v.y * f);
    public static V2f operator /(V2f v, f32 f) => new(v.x / f, v.y / f);
    public static V2f operator %(V2f v, f32 f) => new(v.x % f, v.y % f);
}