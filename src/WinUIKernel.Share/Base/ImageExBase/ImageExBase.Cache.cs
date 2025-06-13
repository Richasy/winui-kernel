// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Windows.Storage;

namespace Richasy.WinUIKernel.Share.Base;

public partial class ImageExBase
{
    private const string CacheFolderName = "ImageExCache";

    private static string CacheFolderPath => Path.Combine(Microsoft.Windows.Storage.ApplicationData.GetDefault().LocalCacheFolder.Path, CacheFolderName);

    /// <summary>
    /// 获取所有缓存文件的大小.
    /// </summary>
    /// <returns>总字节数.</returns>
    public static async ValueTask<long> GetAllCacheSizeAsync()
    {
        var cacheFolder = await GetCacheFolderAsync();
        if (cacheFolder == null)
        {
            return 0L;
        }

        var totalSize = Directory.GetFiles(cacheFolder.Path, "*.*", SearchOption.TopDirectoryOnly)
            .Sum(file => new FileInfo(file).Length);

        return totalSize;
    }

    /// <summary>
    /// 清除缓存文件夹中的所有文件.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task ClearCacheAsync()
    {
        var cacheFolder = await GetCacheFolderAsync();
        if (cacheFolder != null)
        {
            await cacheFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
        }
    }

    /// <summary>
    /// 获取缓存文件的路径.
    /// </summary>
    /// <param name="key">源地址.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<string?> GetCacheFilePathAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(key))
        {
            return null;
        }

        var cacheFolder = await GetCacheFolderAsync();
        var cacheFileName = GetCacheFilePath(key);
        var cacheFilePath = Path.Combine(cacheFolder.Path, cacheFileName);
        if (File.Exists(cacheFilePath))
        {
            return cacheFilePath;
        }

        return null;
    }

    /// <summary>
    /// 写入缓存文件.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="data"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task WriteCacheAsync(string key, byte[] data, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(key) || data == null || data.Length == 0)
        {
            return;
        }

        var cacheFolder = await GetCacheFolderAsync();
        var cacheFileName = GetCacheFilePath(key);
        var cacheFilePath = Path.Combine(cacheFolder.Path, cacheFileName);
        // Ensure the directory exists
        Directory.CreateDirectory(cacheFolder.Path);
        // Write the data to the file
        await File.WriteAllBytesAsync(cacheFilePath, data, cancellationToken);
    }

    private static string GetCacheFilePath(string key)
    {
        var extension = key.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? Path.GetExtension(new Uri(key).Segments.Last()) : Path.GetExtension(key);
        var invalidCharIndex = extension.IndexOfAny(Path.GetInvalidFileNameChars());
        if (invalidCharIndex > -1)
        {
            extension = extension[..invalidCharIndex];
        }

        using var md5 = System.Security.Cryptography.MD5.Create();
        var hash = Convert.ToHexStringLower(md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(key)));
        return $"{hash}{extension}";
    }

    private static async Task<StorageFolder> GetCacheFolderAsync()
    {
        var cacheFolder = await Microsoft.Windows.Storage.ApplicationData.GetDefault().LocalCacheFolder.CreateFolderAsync(CacheFolderName, CreationCollisionOption.OpenIfExists);
        return cacheFolder;
    }
}
