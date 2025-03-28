using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using src.LevelSystem;
using src.Utility;

namespace src.Debugging;

public static class DevConsole
{
    public static readonly List<ConsoleCommand> commands = [
        new(["help", "?"], "help [<command>]", [0, 1], args => {
            if(args.Length == 0)
            {
                StringBuilder sb = new("### HELP LIST\n<...> means compulsory argument\n[<...>] means optional argument\n---\n### COMMANDS\n");

                foreach(ConsoleCommand cmd in commands)
                    sb.AppendLine($"{cmd.syntax}\n  -> Aliases: {string.Join(", ", cmd.aliases)}");

                Out(sb.ToString());
            }
            else if(args.Length == 1)
            {
                ConsoleCommand cmd = commands.Find(cmd => cmd.aliases.Contains(args[0].ToLower()));
                Out($"{cmd.syntax}\n  -> Aliases: {string.Join(", ", cmd.aliases)}");
            }
        }),
        new(["playerpos", "player"], "player <get|set> [<x/y>]", [1, 2], args =>
        {
            if(args[0] == "get")
                Out($"player[0] is at {PlayerController.playerObjs[0].obj.loc}");
            else
            {
                PlayerController.playerObjs[0].obj.loc = ParseV2f(args[1]);
                Out($"player[0] is now at {PlayerController.playerObjs[0].obj.loc}");
            }
        }),
        new(["setvar", "set", "="], "setvar <type> <field|property> <value>", [3], args =>
        {
            Type type = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Name.Equals(args[0], StringComparison.OrdinalIgnoreCase)).FirstOrDefault() ?? throw new($"Could not find type {args[0]}");

            object getValue(Type type)
                => type switch
                {
                    _ when type == typeof(i32) => i32.Parse(args[2]),
                    _ when type == typeof(f32) => f32.Parse(args[2]),
                    _ when type == typeof(string) => args[2][1..^1].Replace("%%", " "),
                    _ when type == typeof(bool) => args[2].Equals("true", StringComparison.OrdinalIgnoreCase),
                    _ when type == typeof(V2f) => new V2f(f32.Parse(args[2].Split('/')[0]), f32.Parse(args[2].Split('/')[1])),
                    _ when type == typeof(V2i) => new V2i(i32.Parse(args[2].Split('/')[0]), i32.Parse(args[2].Split('/')[1])),
                    _ => null
                };

            const BindingFlags FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.IgnoreCase;

            if(type.GetProperty(args[1], FLAGS) is PropertyInfo pInfo)
            {
                pInfo.SetValue(null, getValue(pInfo.PropertyType));
                Out($"The value of property {type.Namespace}.{type.Name}.{pInfo.Name} [{pInfo.PropertyType.Name}] is now {pInfo.GetValue(null).ToStringCatchCollection()}");
            }
            else if(type.GetField(args[1], FLAGS) is FieldInfo fInfo)
            {
                fInfo.SetValue(null, getValue(fInfo.FieldType));
                Out($"The value of field {type.Namespace}.{type.Name}.{fInfo.Name} [{fInfo.FieldType.Name}] is now {fInfo.GetValue(null).ToStringCatchCollection()}");
            }
            else
                throw new($"Could not find property or field with name {args[1]} in type {type.Namespace}.{type.Name}");
        }),
        new(["getvar", "get", "query", "q"], "getvar <type> <field|property>", [2], args =>
        {
            Type type = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Name.Equals(args[0], StringComparison.OrdinalIgnoreCase)).FirstOrDefault() ?? throw new($"Could not find type {args[0]}");

            const BindingFlags FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.IgnoreCase;

            if(type.GetProperty(args[1], FLAGS) is PropertyInfo pInfo)
                Out($"The value of property {type.Namespace}.{type.Name}.{pInfo.Name} [{pInfo.PropertyType.Name}] is currently {pInfo.GetValue(null).ToStringCatchCollection()}");
            else if(type.GetField(args[1], FLAGS) is FieldInfo fInfo)
                Out($"The value of field {type.Namespace}.{type.Name}.{fInfo.Name} [{fInfo.FieldType.Name}] is currently {fInfo.GetValue(null).ToStringCatchCollection()}");
            else
                throw new($"Could not find property or field with name {args[1]} in type {type.Namespace}.{type.Name}");
        }),
        new(["loadlevel", "loadlvl", "level", "lvl"], "level <index|name>", [1], args =>
        {
            LevelManager.UnloadLevel();

            if(args[0].All(char.IsDigit))
                LevelManager.LoadLevel(LevelManager.levelList[i32.Parse(args[0])]);
            else
                LevelManager.LoadLevel(LevelReader.ReadFile($"res/levels/{args[0]}.lvl"));
        })
    ];

    private static readonly Thread thread = new(Run) { IsBackground = true };


    public static void Init()
        => thread.Start();


    private static void Run()
    {
        Out("Development console initiated. To see a list of possible commands, enter 'help'");

        while(ThreadManager.isRunning)
        {
            try
            {
                string[] args = Console.ReadLine().Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);

                ConsoleCommand cmd = commands.Find(cmd => cmd.aliases.Contains(args[0].ToLower()));

                if(cmd.action is null)
                    throw new($"Unknown command");

                if(cmd.possibleArgCounts is not null && !cmd.possibleArgCounts.Contains(args.Length-1))
                    throw new("Invalid argument count.");

                cmd.action(args[1..]);
            }
            catch(Exception e)
            {
                //Log($"There was an error running your command: {e.Message}", LOG_DEV);
                Log($"There was an error running your command: {e}", LOG_DEV);
            }
        }
    }

    private static void Out(object obj)
        => Log(obj.ToString(), LOG_DEV);

    private static V2f ParseV2f(string str)
    {
        string[] xy = str.Split('/');
        return new(f32.Parse(xy[0]), f32.Parse(xy[1]));
    }
}