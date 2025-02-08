namespace src;

public class Program
{
    private static void Main(string[] args)
    {
        Application.Init();

        LevelData level = new(
            [new(new(0, 0), new(1, 1), true, false, 0xff0000, "Test")],
            [new(new(0, -0.5f), new(1f, 0.1f), ObjectType.Wall)],
            new(V2f.zero, new(0.085f, 0.085f), ObjectType.Player)
        );

        GameState.LoadLevel(in level);

        Application.tick += PlayerController.Tick;
        Application.tick += dt =>
        {
            if(Input.KeyDown(Key.R))
                GameState.ReloadCurrentLevel();
        };

        Application.Run();
    }
}