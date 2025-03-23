using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace src.Debugging;

public static class DevConsole
{
    public static readonly List<ConsoleCommand> commands = [
        new(["help", "?"], "help [<command>]", [0, 1], args => {
            if(args.Length == 0)
            {
                StringBuilder sb = new("### HELP LIST\n<...> means compulsory argument\n[<...>] means optional argument\n---\n### COMMANDS\n");

                foreach(ConsoleCommand cmd in commands)
                    sb.AppendLine($"{cmd.syntax}\n  -> Aliases:{string.Join(", ", cmd.aliases)}");

                Out(sb.ToString());
            }
            else if(args.Length == 1)
            {
                ConsoleCommand cmd = commands.Find(cmd => cmd.aliases.Contains(args[0].ToLower()));
                Out($"{cmd.syntax}\n  -> Aliases:{string.Join(", ", cmd.aliases)}");
            }
        }),
        new(["playerpos", "player"], "player <get|set> [<x/y>]", [1, 2], args =>
        {
            if(args[0] == "get")
                Out($"player[0] is at {PlayerController.playerObjs[0].loc}");
            else
            {
                PlayerController.playerObjs[0].loc = ParseV2f(args[1]);
                Out($"player[0] is now at {PlayerController.playerObjs[0].loc}");
            }
        })
    ];

    private static readonly Thread thread = new(Run);


    public static void Init()
        => thread.Start();


    private static void Run()
    {
        Out("Development console initiated; to enter a command press F2; to see a list of possible commands, enter 'help'");

        while(ThreadManager.isRunning)
        {
            if(Input.KeyDown(Key.F2))
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
                    Log($"There was an error running your command: {e.Message}", LOG_DEV);
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