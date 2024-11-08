namespace Services.Settings;

public class GeneralSetting
{
    [Setting(false)]
    public bool? TopMost{ get; set; }

    [Setting(true)]
    public bool? SaveWindowPosition { get; set; }
}
