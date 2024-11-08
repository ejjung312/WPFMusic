using LogHelper;

namespace Common;

public static class Logger
{
    public static ILog Log
    {
        get => LogHelper.LogHelper.Log(LogHelperType.Default);
    }
}