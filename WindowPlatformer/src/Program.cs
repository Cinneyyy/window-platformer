using System;
using System.Globalization;
using src;
using src.Debugging;
using src.Gui;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
AppDomain.CurrentDomain.UnhandledException += (_, e) =>
{
    LogError($"Unhandled {e.ExceptionObject.GetType()} encountered ({(Exception)e.ExceptionObject})\nPress any key to terminate...", LOG_INFO);
    FancyConsole.SetVisible(true);
    Console.ReadKey();
};

FancyConsole.SetVisible(false);

ThreadManager.Init();
ThreadManager.RunOnMainThread(MainMenu.Load, false);
ThreadManager.Run();