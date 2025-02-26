using System.Threading.Tasks;

namespace src;

internal class Program
{
    private static void Main(string[] args)
    {
        ThreadManager.Init();

        LevelData level = new()
        {
            windows = [
                new("Grr", V2f.zero, new(2f, 1.8f), true, false, 0xff0000)
            ],
            objects = [
                new(V2f.zero, new(0.1f, 0.1f), ObjectType.Player),
                new(new(0f, -0.5f), new(1.5f, 0.2f), ObjectType.Wall),
            ]
        };

        ThreadManager.RunOnEventThread(() => LevelManager.LoadLevel(level));

        ThreadManager.Run();
    }
}
