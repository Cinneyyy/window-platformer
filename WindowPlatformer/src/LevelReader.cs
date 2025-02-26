using System.Collections.Generic;
using System.IO;
using System;
using System.Text.RegularExpressions;

namespace src;

public static partial class LevelReader
{
    private enum Context
    {
        None,
        Window,
        Object
    }


    public static LevelData ReadFile(string path)
    {
        string file = File.ReadAllText(path);
        string[] lines = file.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Context ctx = Context.None;

        List<WindowData> windows = [];
        List<GameObjectData> objects = [];

        WindowData? currWindow = null;

        foreach(string rawLn in lines)
        {
            string ln = rawLn.Trim();

            if(string.IsNullOrWhiteSpace(ln))
                continue;

            if(ln == "Objects:" || ln == "Objs:" || ln == "Objects" || ln == "Objs")
            {
                ctx = Context.Object;
                continue;
            }

            if(ln == "Windows:" || ln == "Wins:" || ln == "Windows" || ln == "Wins")
            {
                ctx = Context.Window;
                continue;
            }

            if(ctx == Context.Window)
            {
                if(ln.StartsWith('-'))
                {
                    if(currWindow is not null)
                        windows.Add(currWindow.Value);

                    currWindow = new()
                    {
                        title = ""
                    };
                }
            }
        }
    }


    [GeneratedRegex(@"(?:(""[a-zA-Z0-9\s\-_]?"")|((?:[+-]?[0-9]+(?:\.[0-9]+)?)x(?:[+-]?[0-9]+(?:\.[0-9]+)?))|((?:[+-]?[0-9]+(?:\.[0-9]+)?)\+(?:[+-]?[0-9]+(?:\.[0-9]+)?))|((?:[+-]?[0-9]+(?:\.[0-9]+)?),(?:[+-]?[0-9]+(?:\.[0-9]+)?))|(immovable|movable)|(rigid|resizable)|(#[0-9a-fA-F]{6})|([A-Z][a-z]+))*")]
    private static partial Regex NewWindowEntryRegex();
}