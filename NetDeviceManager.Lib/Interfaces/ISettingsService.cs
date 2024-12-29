using NetDeviceManager.Lib.Model;

namespace NetDeviceManager.Lib.Interfaces;

public interface ISettingsService
{
    void SetConfigValue(string key, string value);
    string? GetConfigValue(string key);
    void UpdateSettings(SettingsUpdateModel settings);
    SettingsUpdateModel GetSettingsUpdateModel();
    SettingsModel GetSettings();
}