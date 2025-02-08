namespace src;

public class Program
{
    private static void Main(string[] args)
    {
        Application.Init();

        LevelData level = new(
            [new(new(0, 0), new(1, 1), true, false, 0xff0000, "Test")],
            [],
            new()
        );

        GameState.LoadLevel(in level);

        Application.Run();
    }
}