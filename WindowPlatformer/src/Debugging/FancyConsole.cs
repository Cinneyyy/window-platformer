using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using src.Dev;

namespace src.Debugging;

#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0052 // Remove unread private members
public partial class FancyConsole
{
    private static readonly nint hWnd = GetConsoleWindow();
    private static readonly nint hStd = GetStdHandle(-11);


    public static string FormatColor(string msg)
        => (msg + "%(default)%")
        .Pipe(msg => FgRegex().Replace(msg, match => ParseColor(match.Groups[1].Value, true)))
        .Pipe(msg => BgRegex().Replace(msg, match => ParseColor(match.Groups[1].Value, false)))
        .Replace("%(default)%", "\x1b[0m")
        .Replace("%(def)%", "\x1b[0m");

    public static void WriteLine(string msg)
        => Console.WriteLine(FormatColor(msg));

    [GeneratedRegex(@"%F\(#([0-9A-F]{6}|[0-9a-f]{6})\)%")]
    private static partial Regex FgRegex();
    [GeneratedRegex(@"%B\(#([0-9A-F]{6}|[0-9a-f]{6})\)%")]
    private static partial Regex BgRegex();

    private static string ParseColor(string regexGroup, bool isFg)
        => $"\x1b[{(isFg ? "38" : "48")};2;{Convert.ToInt32(regexGroup[..2], 16)};{Convert.ToInt32(regexGroup[2..4], 16)};{Convert.ToInt32(regexGroup[4..], 16)}m";

    #region P/Invokes
    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.SysInt)]
    private static partial nint GetConsoleWindow();

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ShowWindow(nint hWnd, i32 cmdShow);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.SysInt)]
    private static partial nint PostMessage(nint hWnd, u32 Msg, nint wParam, nint lParam);

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetConsoleMode(nint hWnd, i32 mode);

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetConsoleMode(nint hWnd, out i32 mode);

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.SysInt)]
    private static partial nint GetStdHandle(i32 handle);
    #endregion


    internal static void Init()
    {
        GetConsoleMode(hStd, out i32 consoleMode);
        SetConsoleMode(hStd, consoleMode | 4);
    }
}