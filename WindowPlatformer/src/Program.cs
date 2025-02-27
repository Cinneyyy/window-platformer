using System.Globalization;
using src;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

ThreadManager.Init();

LevelData level = LevelReader.ReadFile("res/levels/0.lvl");
ThreadManager.RunOnEventThread(() => LevelManager.LoadLevel(level));

ThreadManager.Run();