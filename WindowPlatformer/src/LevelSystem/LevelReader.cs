using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

namespace src.LevelSystem;

public static class LevelReader
{
    private enum Context
    {
        None,
        Window,
        Object
    }


    public const f32 WINDOW_ENTRY_MUL = 1f;


    public static LevelData ReadFile(string path)
    {
        string file = File.ReadAllText(path);
        string[] lines = file.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Context ctx = Context.None;

        List<WindowData> windows = [];
        List<GameObjectData> objects = [];

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

            if(!ln.StartsWith("- "))
                continue;
            else
                ln = ln[2..];

            if(ctx == Context.Window)
            {
                (string title, V2f size, V2f loc, bool movable, bool resizable, u32 color, V2f entryDir, V2f entrySize, bool entryRedraw) winData = ("", V2f.zero, V2f.zero, false, false, 0xffffff, V2f.zero, new(1f), true);

                foreach(string token in ln
                    .Split(", ", StringSplitOptions.RemoveEmptyEntries) // Split by comma
                    .SelectMany(t => t.Split(' ', '\t'))                // Split by space & tab
                    .Select(t => t
                        .Trim()                                         // Trim
                        .Replace("%%", " ")))                           // Substitute "%%" with space
                    switch(token)
                    {
                        case var _ when token.StartsWith('"') && token.EndsWith('"'):
                            winData.title = token[1..^1];
                            break;
                        case var _ when token.StartsWith('#') && token.Length == 7:
                            winData.color = Convert.ToUInt32(token[1..], 16);
                            break;
                        case "m:true" or "m:false":
                            winData.movable = token == "m:true";
                            break;
                        case "r:true" or "r:false":
                            winData.resizable = token == "r:true";
                            break;
                        case "er:true" or "er:false":
                            winData.entryRedraw = token == "er:true";
                            break;
                        case var _ when token.Contains('x'):
                        {
                            string[] xy = token.Split('x');
                            winData.size = new(f32.Parse(xy[0]), f32.Parse(xy[1]));
                            break;
                        }
                        case var _ when token.Contains('/'):
                        {
                            string[] xy = token.Split('/');
                            winData.loc = new(f32.Parse(xy[0]), f32.Parse(xy[1]));
                            break;
                        }
                        case var _ when token.StartsWith('^'):
                        {
                            string t = token[1..];

                            if(t.Contains('^'))
                            {
                                string[] xy = t.Split('^');
                                winData.entryDir = new(f32.Parse(xy[0]), f32.Parse(xy[1]));
                                break;
                            }

                            winData.entryDir = WINDOW_ENTRY_MUL * t switch
                            {
                                "u" or "up" => V2f.up,
                                "d" or "down" => V2f.down,
                                "l" or "left" => V2f.left,
                                "r" or "right" => V2f.right,
                                "n" or "none" or "0" => V2f.zero,
                                _ => throw new($"Invalid entry direction: {t}")
                            };

                            break;
                        }
                        case var _ when token.StartsWith('*'):
                        {
                            string[] xy = token[1..].Split('*');
                            winData.entrySize = new(f32.Parse(xy[0]), f32.Parse(xy[1]));
                            break;
                        }
                        default: break;
                    }

                windows.Add(new(winData.title, winData.loc, winData.size, winData.movable, winData.resizable, winData.color, winData.loc - winData.entryDir, winData.size * winData.entrySize, winData.entryRedraw));
            }
            else if(ctx == Context.Object)
            {
                (V2f loc, V2f size, ObjectType type, string text) objData = new(V2f.zero, V2f.zero, ObjectType.Wall, null);

                foreach(string token in ln
                    .Split(", ", StringSplitOptions.RemoveEmptyEntries) // Split by comma
                    .SelectMany(t => t.Split(' ', '\t'))                // Split by space & tab
                    .Select(t => t.Trim()))                             // Trim
                    switch(token)
                    {
                        case var _ when token.All(char.IsLetter):
                            objData.type = Enum.Parse<ObjectType>(token, true);
                            break;
                        case var _ when token.Contains('x'):
                        {
                            string[] xy = token.Split('x');
                            objData.size = new(f32.Parse(xy[0]), f32.Parse(xy[1]));
                            break;
                        }
                        case var _ when token.Contains('/'):
                        {
                            string[] xy = token.Split('/');
                            objData.loc = new(f32.Parse(xy[0]), f32.Parse(xy[1]));
                            break;
                        }
                        case var _ when token.StartsWith('"') && token.EndsWith('"'):
                        {
                            objData.text = token[1..^1];
                            break;
                        }
                        default: break;
                    }

                if(objData.text is not null)
                    objData.type = ObjectType.Text;

                objects.Add(new(objData.loc, objData.size, objData.type, objData.text));
            }
        }

        return new([..windows], [..objects], path);
    }
}