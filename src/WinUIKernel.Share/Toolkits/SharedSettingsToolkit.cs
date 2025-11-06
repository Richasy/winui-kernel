// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Windows.Storage;

namespace Richasy.WinUIKernel.Share.Toolkits;

/// <summary>
/// Settings toolkit.
/// </summary>
public class SharedSettingsToolkit : ISettingsToolkit
{
    /// <inheritdoc/>
    public bool IsSettingKeyExist(string settingName)
        => GetSettingContainer().Values.ContainsKey(settingName);

    /// <inheritdoc/>
    public T ReadLocalSetting<T>(string settingName, T defaultValue)
    {
        var settingContainer = GetSettingContainer();

        if (IsSettingKeyExist(settingName))
        {
            if (defaultValue is Enum)
            {
                var tempValue = settingContainer.Values[settingName].ToString();
                var isParsed = Enum.TryParse(typeof(T), tempValue, out var result);
                return !isParsed ? defaultValue : (T)result!;
            }
            else
            {
                return (T)settingContainer.Values[settingName];
            }
        }
        else
        {
            WriteLocalSetting(settingName, defaultValue);
            return defaultValue;
        }
    }

    /// <inheritdoc/>
    public void WriteLocalSetting<T>(string settingName, T value)
    {
        var settingContainer = GetSettingContainer();
        settingContainer.Values[settingName] = value is Enum ? value.ToString() : value;
    }

    /// <inheritdoc/>
    public void DeleteLocalSetting(string settingName)
    {
        var settingContainer = GetSettingContainer();

        if (IsSettingKeyExist(settingName))
        {
            settingContainer.Values.Remove(settingName);
        }
    }

    /// <summary>
    /// Get the setting container.
    /// </summary>
    /// <returns><see cref="ApplicationDataContainer"/>.</returns>
    protected virtual Microsoft.Windows.Storage.ApplicationDataContainer GetSettingContainer()
        => Microsoft.Windows.Storage.ApplicationData.GetDefault().LocalSettings;
}
