namespace src;

public record struct V2i(i32 x, i32 y)
{
    public i32 x = x, y = y;


    public override readonly string ToString()
        => $"({x}, {y})";


    public static V2i operator +(V2i a, V2i b) => new(a.x + b.x, a.y + b.y);
    public static V2i operator -(V2i a, V2i b) => new(a.x - b.x, a.y - b.y);
    public static V2i operator *(V2i a, V2i b) => new(a.x * b.x, a.y * b.y);
    public static V2i operator /(V2i a, V2i b) => new(a.x / b.x, a.y / b.y);
    public static V2i operator %(V2i a, V2i b) => new(a.x % b.x, a.y % b.y);

    public static V2i operator +(V2i v, i32 i) => new(v.x + i, v.y + i);
    public static V2i operator -(V2i v, i32 i) => new(v.x - i, v.y - i);
    public static V2i operator *(V2i v, i32 i) => new(v.x * i, v.y * i);
    public static V2i operator /(V2i v, i32 i) => new(v.x / i, v.y / i);
    public static V2i operator %(V2i v, i32 i) => new(v.x % i, v.y % i);
}