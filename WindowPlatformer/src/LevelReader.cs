using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

namespace src;

public static class LevelReader
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
                (string title, V2f size, V2f loc, bool movable, bool resizable, u32 color) winData = ("", V2f.zero, V2f.zero, false, false, 0xffffff);

                foreach(string token in ln.Split(", ", StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()))
                    switch(token)
                    {
                        case var _ when token.StartsWith('"') && token.EndsWith('"'):
                            winData.title = token[1..^2];
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
                    }

                windows.Add(new(winData.title, winData.loc, winData.size, winData.movable, winData.resizable, winData.color));
            }
            else if(ctx == Context.Object)
            {
                (V2f loc, V2f size, ObjectType type) objData = new(V2f.zero, V2f.zero, ObjectType.Wall);

                foreach(string token in ln.Split(", ", StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()))
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
                    }

                objects.Add(new(objData.loc, objData.size, objData.type));
            }
        }

        return new([..windows], [..objects]);
    }
}