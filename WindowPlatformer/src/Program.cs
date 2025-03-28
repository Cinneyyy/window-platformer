using System;
using System.Globalization;
using System.IO;
using src;
using src.Debugging;
using src.Gui;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
AppDomain.CurrentDomain.UnhandledException += (_, e) =>
{
    LogError($"Unhandled {e.ExceptionObject.GetType()} encountered ({(Exception)e.ExceptionObject})\nPress any key to terminate...", LOG_INFO);
    ConsoleWindow.SetVisible(true);
    Console.ReadKey();
};

ConsoleWindow.SetVisible(false);

ThreadManager.Init();
ThreadManager.RunOnMainThread(MainMenu.Load, false);
ThreadManager.Run();