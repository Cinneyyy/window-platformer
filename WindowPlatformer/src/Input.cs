using System.Collections.Generic;
using System.Linq;

namespace src;

public static partial class Input
{
    private static readonly KeyState keyState = new(), prevKeyState = new();
    private static readonly List<(Key key, bool down)> simulatedKeys = [];


    public static V2i mousePosScreen
    {
        get
        {
            _ = SDL_GetGlobalMouseState(out i32 x, out i32 y);
            return new(x, y);
        }
    }
    public static V2f mousePosWorld => Screen.WorldPointFromScreen(mousePosScreen);


    public static bool KeyHeld(Key key) => keyState[key];
    public static bool KeyDown(Key key) => keyState[key] && !prevKeyState[key];
    public static bool KeyUp(Key key) => !keyState[key] && prevKeyState[key];

    public static bool KeyHeld(params Key[] keys) => keys.Any(KeyHeld);
    public static bool KeyDown(params Key[] keys) => keys.Any(KeyDown);
    public static bool KeyUp(params Key[] keys) => keys.Any(KeyUp);

    public static void SimulateKey(Key key, bool down) => simulatedKeys.Add((key, down));


    internal static partial void Init();
    internal static partial void Tick();
}