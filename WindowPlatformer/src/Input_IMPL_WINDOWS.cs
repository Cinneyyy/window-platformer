#if OS_WINDOWS
using System.Runtime.InteropServices;

namespace src;

public static partial class Input
{
    internal static partial void Init()
    {
    }

    internal static partial void Tick()
    {
        KeyState.SwapBuffers(keyState, prevKeyState);

        for(i32 i = 0; i < KeyState.KEY_COUNT; i++)
            keyState[i] = (GetAsyncKeyState(KeyToVKey((Key)i)) & 0x8000) != 0;
    }


    [LibraryImport("user32.dll")]
    private static partial i16 GetAsyncKeyState(i32 vKey);

#pragma warning disable
    private static i32 KeyToVKey(Key key)
        => key switch
        {
            >= Key.A and <= Key.Z => key - Key.A + 0x41, // A-Z
            >= Key.F1 and <= Key.F12 => key - Key.F1 + 0x70, // F1-F12
            >= Key.Num1 and <= Key.Num9 => key - Key.Num1 + 0x31, // 1-9
            Key.Num0 => 0x30,
            Key.Left => 0x25,
            Key.Right => 0x27,
            Key.Up => 0x26,
            Key.Down => 0x28,
            Key.Ctrl => 0xa2,
            Key.Shift => 0xa0,
            Key.Alt => 0xa4,
            Key.Tab => 0x9,
            Key.Return => 0xd,
            Key.Space => 0x20,
            _ => 0
        };
#pragma warning restore
}
#endif