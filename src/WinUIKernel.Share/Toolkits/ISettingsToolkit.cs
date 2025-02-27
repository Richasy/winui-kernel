// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

namespace Richasy.WinUIKernel.Share.Toolkits;

/// <summary>
/// Settings toolkit.
/// </summary>
public interface ISettingsToolkit
{
    /// <summary>
    /// Read local setting.
    /// </summary>
    /// <typeparam name="T">Type of read value.</typeparam>
    /// <param name="settingName">Setting name.</param>
    /// <param name="defaultValue">Default value provided when the setting does not exist.</param>
    /// <returns>Setting value obtained.</returns>
    T ReadLocalSetting<T>(string settingName, T defaultValue);

    /// <summary>
    /// Write local setting.
    /// </summary>
    /// <typeparam name="T">Type of written value.</typeparam>
    /// <param name="settingName">Setting name.</param>
    /// <param name="value">Setting value.</param>
    void WriteLocalSetting<T>(string settingName, T value);

    /// <summary>
    /// Delete local setting.
    /// </summary>
    /// <param name="settingName">Setting name.</param>
    void DeleteLocalSetting(string settingName);

    /// <summary>
    /// Whether the setting to be read has been created locally.
    /// </summary>
    /// <param name="settingName">Setting name.</param>
    /// <returns><c>true</c> means the local setting exists, <c>false</c> means it does not exist.</returns>
    bool IsSettingKeyExist(string settingName);
}
