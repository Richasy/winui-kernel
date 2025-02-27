// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json.Serialization.Metadata;
using Windows.Storage;

namespace Richasy.WinUIKernel.Share.Toolkits;

/// <summary>
/// File toolkit.
/// </summary>
public interface IFileToolkit
{
    /// <summary>
    /// 选择文件.
    /// </summary>
    /// <param name="extension">扩展名.</param>
    /// <param name="windowInstance">窗口对象.</param>
    /// <returns>文件对象（可能为空）.</returns>
    Task<StorageFile> PickFileAsync(string extension, object windowInstance);

    /// <summary>
    /// 选择多个文件.
    /// </summary>
    /// <param name="extension">扩展名.</param>
    /// <param name="windowInstance">窗口对象.</param>
    /// <returns>文件对象（可能为空）.</returns>
    Task<IReadOnlyList<StorageFile>?> PickMultipleFileAsync(string extension, object windowInstance);

    /// <summary>
    /// 选择文件夹.
    /// </summary>
    /// <param name="windowInstance">窗口对象.</param>
    /// <returns>文件夹.</returns>
    Task<StorageFolder> PickFolderAsync(object windowInstance);

    /// <summary>
    /// 保存文件.
    /// </summary>
    /// <param name="extension">扩展名.</param>
    /// <param name="windowInstance">窗口实例.</param>
    /// <returns>文件.</returns>
    Task<StorageFile?> SaveFileAsync(string extension, object windowInstance);

    /// <summary>
    /// Get local data and convert.
    /// </summary>
    /// <typeparam name="T">Conversion target type.</typeparam>
    /// <param name="fileName">File name.</param>
    /// <param name="typeInfo">Json type info for deserialize.</param>
    /// <param name="defaultValue">The default value when the file does not exist or has no content.</param>
    /// <param name="folderName">The folder to which the file belongs.</param>
    /// <returns>Converted result.</returns>
    Task<T> ReadLocalDataAsync<T>(string fileName, JsonTypeInfo<T> typeInfo, string defaultValue = "{}", string folderName = "");

    /// <summary>
    /// Write data to local file.
    /// </summary>
    /// <typeparam name="T">Type of data.</typeparam>
    /// <param name="fileName">File name.</param>
    /// <param name="data">Data to be written.</param>
    /// <param name="typeInfo">Type info for serialize.</param>
    /// <param name="folderName">The folder to which the file belongs.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task WriteLocalDataAsync<T>(string fileName, T data, JsonTypeInfo<T> typeInfo, string folderName = "");

    /// <summary>
    /// Delete local data file.
    /// </summary>
    /// <param name="fileName">File name.</param>
    /// <param name="folderName">The folder to which the file belongs.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task DeleteLocalDataAsync(string fileName, string folderName = "");
}
