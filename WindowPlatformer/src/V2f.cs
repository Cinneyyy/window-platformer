namespace src;

public record struct V2f(f32 x, f32 y)
{
    public V2f(f32 xy) : this(xy, xy) {}


    public f32 x = x, y = y;

    public static readonly V2f zero = new(0), one = new(1f);
    public static readonly V2f up = new(0f, 1f), down = new(0f, -1f), left = new(-1f, 0f), right = new(1f, 0f);


    public override readonly string ToString()
        => $"({x:0.00}, {y:0.00})";


    public static V2f Lerp(V2f a, V2f b, f32 t)
        => new(f32.Lerp(a.x, b.x, t), f32.Lerp(a.y, b.y, t));


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

    public static V2f operator +(f32 f, V2f v) => new(f + v.x, f + v.y);
    public static V2f operator -(f32 f, V2f v) => new(f - v.x, f - v.y);
    public static V2f operator *(f32 f, V2f v) => new(f * v.x, f * v.y);
    public static V2f operator /(f32 f, V2f v) => new(f / v.x, f / v.y);
    public static V2f operator %(f32 f, V2f v) => new(f % v.x, f % v.y);
}