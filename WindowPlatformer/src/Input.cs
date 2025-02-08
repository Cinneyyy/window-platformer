using System.Collections.Generic;
using System.Linq;

namespace src;

public static partial class Input
{
    private static readonly KeyState keyState = new(), prevKeyState = new();


    /// <summary>This function shall return true, should the key, appointed in its input parameter, at this moment witness the grace of human touch.</summary>
    public static bool KeyHeld(Key key) => keyState[key];
    /// <summary>This function shall return true, should the key, appointed in its input parameter, first witness the grace of human touch, albeit for a mere moment, for it shall return false again thereafter, ere such an event ocurreth again.</summary>
    public static bool KeyDown(Key key) => keyState[key] && !prevKeyState[key];
    /// <summary>This function shall return true, should the key, appointed in its input parameter, not feel the grace of human touch; hitherto, and henceforth, it shall eternally return false, till this predicate may be met again.</summary>
    public static bool KeyUp(Key key) => !keyState[key] && prevKeyState[key];

    public static bool KeyHeld(params Key[] keys) => keys.Any(KeyHeld);
    public static bool KeyDown(params Key[] keys) => keys.Any(KeyDown);
    public static bool KeyUp(params Key[] keys) => keys.Any(KeyUp);


    internal static partial void Init();
    internal static partial void Tick();
}