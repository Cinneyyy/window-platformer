namespace src.Debugging;

public static class Logger
{
    public readonly record struct LogType(string name, string colorFormat)
    {
        public void Write<T>(T obj) => Log(obj, this);
        public void Error<T>(T obj) => LogError(obj, this);
        public void Warn<T>(T obj) => LogWarning(obj, this);
    }


    public static readonly LogType LOG_INFO = new("Info", "%F(#bfbfbf)%");
    public static readonly LogType LOG_DEBUG = new("Debug", "%F(#20bb10)%");
    public static readonly LogType LOG_SDL = new("SDL", "%F(#10dcff)%");
    public static readonly LogType LOG_DEV = new("Dev", "%F(#9310ff)%");


    public static void Log<T>(this T obj)
        => Log(obj, LOG_DEBUG);
    public static void Log<T>(this T obj, LogType log)
        => ConsoleWindow.WriteLine($"{log.colorFormat}[{log.name}] {obj}");
    public static void Log(string msg)
        => msg.Log();
    public static void Log(string msg, LogType log)
        => msg.Log(log);

    public static void LogError<T>(this T obj)
        => LogError(obj, LOG_DEBUG);
    public static void LogError<T>(this T obj, LogType log)
        => ConsoleWindow.WriteLine($"{log.colorFormat}%B(#500000)%[{log.name}] {obj}");
    public static void LogError(string msg)
        => msg.LogError();
    public static void LogError(string msg, LogType log)
        => msg.LogError(log);

    public static void LogWarning<T>(this T obj)
        => LogWarning(obj, LOG_DEBUG);
    public static void LogWarning<T>(this T obj, LogType log)
        => ConsoleWindow.WriteLine($"{log.colorFormat}%B(#2d3000)%[{log.name}] {obj}");
    public static void LogWarning(string msg)
        => msg.LogWarning();
    public static void LogWarning(string msg, LogType log)
        => msg.LogWarning(log);

    public static bool LogSdlError(this i32 code, string message = "")
    {
        if(code < 0)
        {
            LogError($"\"{SDL_GetError()}\"{(string.IsNullOrWhiteSpace(message) ? "" : $" ({message})")}", LOG_SDL);
            return true;
        }

        return false;
    }
    public static void LogSdlError(string message)
        => LogError($"\"{SDL_GetError()}\"{(string.IsNullOrWhiteSpace(message) ? "" : $" ({message})")}");

    public static i32 ThrowSdlError(this i32 code, string message = "")
    {
        if(code < 0)
            throw new SdlException($"SDL Error: \"{SDL_GetError()}\"{(string.IsNullOrWhiteSpace(message) ? "" : $" ({message})")}");

        return code;
    }
    public static nint ThrowSdlError(this nint ptr, string message)
    {
        if(ptr == nint.Zero)
            throw new SdlException($"SDL Error: \"{SDL_GetError()}\"{(string.IsNullOrWhiteSpace(message) ? "" : $" ({message})")}");

        return ptr;
    }
    public static void ThrowSdlError(string message = "")
        => throw new SdlException($"SDL Error: \"{SDL_GetError()}\"{(string.IsNullOrWhiteSpace(message) ? "" : $" ({message})")}");
}