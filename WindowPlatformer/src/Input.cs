using System.Linq;

namespace src;

public static partial class Input
{
    private static readonly KeyState keyState = new(), prevKeyState = new();


    public static bool KeyHeld(Key key) => keyState[key];
    public static bool KeyDown(Key key) => keyState[key] && !prevKeyState[key];
    public static bool KeyUp(Key key) => !keyState[key] && prevKeyState[key];

    public static bool KeyHeld(params Key[] keys) => keys.Any(KeyHeld);
    public static bool KeyDown(params Key[] keys) => keys.Any(KeyDown);
    public static bool KeyUp(params Key[] keys) => keys.Any(KeyUp);


    internal static partial void Init();
    internal static partial void Tick();
}