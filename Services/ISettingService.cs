﻿using System.IO;
using System.Reflection;
using Common.Helper;
using Services.Settings;

namespace Services;

public interface ISettingService
{
    public GeneralSetting? GeneralSetting { get; }
    public PlaySetting? PlaySetting { get;  }
    public WindowSetting? WindowSetting { get; }

    public void SaveSetting();
}

public class SettingService : ISettingService
{
    private GeneralSetting? _generalSetting;
    private PlaySetting? _playSetting;
    private WindowSetting? _windowSetting;

    public GeneralSetting? GeneralSetting => _generalSetting;

    public PlaySetting? PlaySetting => _playSetting;

    public WindowSetting? WindowSetting => _windowSetting;

    public SettingService()
    {
        if (File.Exists(PathHelper.GetLocalDirectory("Settings.xml")))
        {
            SettingsProvider settingsProvider = SerializeHelper.ReadDataFromXmlFile<SettingsProvider>(PathHelper.GetLocalDirectory("Settings.xml"), true);
            _generalSetting = settingsProvider.generalSetting ?? new GeneralSetting();
            _playSetting = settingsProvider.playSetting ?? new PlaySetting();
            _windowSetting = settingsProvider.windowSetting ?? new WindowSetting();
        }
        else
        {
            _generalSetting = new GeneralSetting();
            _playSetting = new PlaySetting();
            _windowSetting = new WindowSetting();
            SaveSetting();
        }

        // 설정 기본값 적용
        SetDefaultValue();
    }

    private void SetDefaultValue()
    {
        // 일반 설정 기본 값 적용
        PropertyInfo[] propertyInfoArr = _generalSetting.GetType().GetProperties();
        foreach (PropertyInfo pi in propertyInfoArr)
        {
            if (pi.GetValue(_generalSetting) == null ||
                (pi.GetValue(_generalSetting) is int && ((int)pi.GetValue(_generalSetting)) == 0) ||
                (pi.GetValue(_generalSetting) is double && ((double)pi.GetValue(_generalSetting)) == 0) ||
                (pi.GetValue(_generalSetting) is float && ((float)pi.GetValue(_generalSetting)) == 0))
            {
                SettingAttribute settingAtt = pi.GetCustomAttribute<SettingAttribute>();
                pi.SetValue(_generalSetting, settingAtt.DefaultValue);
            }
        }

        // 재생 설정 기본 값 적용
        propertyInfoArr = _playSetting.GetType().GetProperties();
        foreach (PropertyInfo pi in propertyInfoArr)
        {
            if (pi.GetValue(_playSetting) == null ||
                (pi.GetValue(_playSetting) is int && ((int)pi.GetValue(_playSetting)) == 0) ||
                (pi.GetValue(_playSetting) is double && ((double)pi.GetValue(_playSetting)) == 0) ||
                (pi.GetValue(_playSetting) is float && ((float)pi.GetValue(_playSetting)) == 0))
            {
                SettingAttribute settingAtt = pi.GetCustomAttribute<SettingAttribute>();
                pi.SetValue(_playSetting, settingAtt.DefaultValue);
            }
        }

        // Window 설정 기본 값 적용
        propertyInfoArr = _windowSetting.GetType().GetProperties();
        foreach (PropertyInfo pi in propertyInfoArr)
        {
            if (pi.GetValue(_windowSetting) == null ||
                (pi.GetValue(_windowSetting) is int && ((int)pi.GetValue(_windowSetting)) == 0) ||
                (pi.GetValue(_windowSetting) is double && ((double)pi.GetValue(_windowSetting)) == 0) ||
                (pi.GetValue(_windowSetting) is float && ((float)pi.GetValue(_windowSetting)) == 0))
            {
                SettingAttribute settingAtt = pi.GetCustomAttribute<SettingAttribute>();
                pi.SetValue(_windowSetting, settingAtt.DefaultValue);
            }
        }
    }

    public void SaveSetting()
    {
        SettingsProvider settingsProvider = new SettingsProvider();
        settingsProvider.generalSetting = GeneralSetting;
        settingsProvider.playSetting = PlaySetting;
        settingsProvider.windowSetting = WindowSetting;

        SerializeHelper.SaveDataToXml<SettingsProvider>(PathHelper.GetLocalDirectory("Settings.xml"), settingsProvider, true);
    }
}
