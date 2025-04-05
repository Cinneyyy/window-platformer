using System.Collections.Generic;
using System.Linq;
using src.Debugging;

namespace src;

public static partial class Input
{
    private static readonly KeyState keyState = new(), prevKeyState = new();
    private static readonly List<(Key key, bool down)> simulatedKeys = [];
    private static bool takeInput;


    public static V2i mousePosScreen
    {
        get
        {
            _ = SDL_GetGlobalMouseState(out i32 x, out i32 y);
            return new(x, y);
        }
    }
    public static V2f mousePosWorld => Screen.WorldPointFromScreen(mousePosScreen);


    public static bool KeyHeld(Key key) => takeInput && keyState[key];
    public static bool KeyDown(Key key) => takeInput && keyState[key] && !prevKeyState[key];
    public static bool KeyUp(Key key) => takeInput && !keyState[key] && prevKeyState[key];

    public static bool KeyHeld(params Key[] keys) => keys.Any(KeyHeld);
    public static bool KeyDown(params Key[] keys) => keys.Any(KeyDown);
    public static bool KeyUp(params Key[] keys) => keys.Any(KeyUp);

    public static void SimulateKey(Key key, bool down) => simulatedKeys.Add((key, down));


    internal static void Tick()
    {
        Tick_Impl();

        foreach((Key key, bool down) in simulatedKeys)
            keyState[key] = down;
        simulatedKeys.Clear();

        takeInput = SDL_GetKeyboardFocus() != nint.Zero && !ConsoleWindow.hasFocus;
    }


    internal static partial void Init();
    internal static partial void Tick_Impl();
}