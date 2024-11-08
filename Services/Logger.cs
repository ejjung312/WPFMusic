using LogHelper;

namespace Services;

public static class Logger
{
    public static ILog Log
    {
        get => LogHelper.LogHelper.Log(LogHelperType.Default);
    }
}