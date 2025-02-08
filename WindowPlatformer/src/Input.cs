namespace src;

public static partial class Input
{
    private static readonly KeyState keyState = new(), prevKeyState = new();


    public static bool KeyHeld(Key key) => keyState[key];
    public static bool KeyDown(Key key) => keyState[key] && !prevKeyState[key];
    public static bool KeyUp(Key key) => !keyState[key] && prevKeyState[key];


    internal static partial void Init();
    internal static partial void Tick();
}