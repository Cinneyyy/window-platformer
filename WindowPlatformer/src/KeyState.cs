namespace src;

public class KeyState()
{
    // Key.None placeth itself post-penultimately in the enumeration hight Key, such that it doth ever tell the sum of keys whereof its container consisteth
    public const i32 KEY_COUNT = (i32)Key._Count;

    private bool[] state = new bool[KEY_COUNT];


    public bool this[Key key]
    {
        get => state[(i32)key];
        set => state[(i32)key] = value;
    }

    public bool this[i32 key]
    {
        get => state[key];
        set => state[key] = value;
    }


    public static void SwapBuffers(KeyState a, KeyState b)
        => (a.state, b.state) = (b.state, a.state);
}