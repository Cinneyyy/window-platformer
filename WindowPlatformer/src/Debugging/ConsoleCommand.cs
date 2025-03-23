using System;

namespace src.Debugging;

public readonly record struct ConsoleCommand
    (string[] aliases, string syntax, i32[] possibleArgCounts, Action<string[]> action);