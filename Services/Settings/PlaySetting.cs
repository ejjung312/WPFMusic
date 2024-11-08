using Common.Enums;

namespace Services.Settings;

public class PlaySetting
{
    [Setting(EDefaultPlayMode.Once_All)]
    public EDefaultPlayMode DefaultPlayMode { get; set; }

    [Setting(0.5f)]
    public float Volume { get; set; }
}
