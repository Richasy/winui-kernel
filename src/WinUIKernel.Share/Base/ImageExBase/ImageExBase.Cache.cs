// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Windows.Storage;

namespace Richasy.WinUIKernel.Share.Base;

public partial class ImageExBase
{
    private const string CacheFolderName = "ImageExCache";

    // 🚀 内存缓存：避免每次都检查文件系统
    private static readonly Dictionary<string, (string? FilePath, DateTime CheckTime)> _memoryCacheIndex = new();
    private static readonly object _memoryCacheLock = new();
    private static readonly TimeSpan _memoryCacheValidity = TimeSpan.FromMinutes(5); // 内存缓存有效期

    /// <summary>
    /// 获取所有缓存文件的大小.
    /// </summary>
    /// <returns>总字节数.</returns>
    public async ValueTask<long> GetAllCacheSizeAsync()
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
    public async Task ClearCacheAsync()
    {
        var cacheFolder = await GetCacheFolderAsync();
        if (cacheFolder != null)
        {
            await cacheFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
        }

        // 🚀 清除内存缓存
        lock (_memoryCacheLock)
        {
            _memoryCacheIndex.Clear();
        }
    }

    /// <summary>
    /// 获取缓存文件的路径.
    /// </summary>
    /// <param name="key">源地址.</param>
    /// <param name="cacheSubFolder">缓存子文件夹名称（可选，用于线程安全传递）.</param>
    /// <returns></returns>
    public async Task<string?> GetCacheFilePathAsync(string key, string? cacheSubFolder = null)
    {
        if (string.IsNullOrEmpty(key))
        {
            return null;
        }

        // 🚀 优化：先检查内存缓存
        var cacheKey = GetCacheFileName(key);
        lock (_memoryCacheLock)
        {
            if (_memoryCacheIndex.TryGetValue(cacheKey, out var cached))
            {
                // 如果内存缓存有效且未过期
                if (DateTime.UtcNow - cached.CheckTime < _memoryCacheValidity)
                {
                    return cached.FilePath;
                }
                else
                {
                    // 过期则移除
                    _memoryCacheIndex.Remove(cacheKey);
                }
            }
        }

        var cacheFolder = await GetCacheFolderAsync(cacheSubFolder);
        var cacheFileName = GetCacheFileName(key);
        var cacheFilePath = Path.Combine(cacheFolder.Path, cacheFileName);
        string? result = null;

        if (File.Exists(cacheFilePath))
        {
            try
            {
                var fileInfo = new FileInfo(cacheFilePath);
                var createTime = fileInfo.CreationTimeUtc;
                if (DateTimeOffset.UtcNow - createTime > WinUIKernelShareExtensions.ImageCacheTime)
                {
                    File.Delete(cacheFilePath);
                    result = null;
                }
                else
                {
                    result = cacheFilePath;
                }
            }
            catch
            {
                // Ignore any exceptions during deletion
                result = null;
            }
        }

        // 🚀 更新内存缓存
        lock (_memoryCacheLock)
        {
            _memoryCacheIndex[cacheKey] = (result, DateTime.UtcNow);
        }

        return result;
    }

    /// <summary>
    /// 写入缓存文件.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="data"></param>
    /// <param name="cacheSubFolder">缓存子文件夹名称（可选，用于线程安全传递）.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task WriteCacheAsync(string key, byte[] data, string? cacheSubFolder = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(key) || data == null || data.Length == 0)
        {
            return;
        }

        var cacheFolder = await GetCacheFolderAsync(cacheSubFolder);
        var cacheFileName = GetCacheFileName(key);
        var cacheFilePath = Path.Combine(cacheFolder.Path, cacheFileName);
        
        // Write the data to the file
        await File.WriteAllBytesAsync(cacheFilePath, data, cancellationToken);

        // 🚀 更新内存缓存
        lock (_memoryCacheLock)
        {
            _memoryCacheIndex[cacheFileName] = (cacheFilePath, DateTime.UtcNow);
        }
    }

    private static string GetCacheFileName(string key)
    {
        key = NormalizeKey(key);
        using var md5 = System.Security.Cryptography.MD5.Create();
        var hash = Convert.ToHexStringLower(md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(key)));
        return hash;
    }

    private static string NormalizeKey(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return string.Empty;
        }

        if (key.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            var uri = new Uri(key);
            // 统一转为小写（根据需求决定是否保留大小写敏感性）
            var normalizedUrl = uri.AbsoluteUri.ToLowerInvariant();
            // 可选：对查询参数排序（如果参数顺序不影响内容）
            if (!string.IsNullOrEmpty(uri.Query))
            {
                var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
                var sortedQuery = string.Join("&", queryParams.AllKeys.OrderBy(k => k)
                    .Select(k => $"{k}={queryParams[k]}"));
                normalizedUrl = $"{uri.GetLeftPart(UriPartial.Path)}?{sortedQuery}";
            }

            return normalizedUrl;
        }

        return key.ToLowerInvariant();
    }

    private async Task<StorageFolder> GetCacheFolderAsync(string? cacheSubFolder = null)
    {
        var cacheFolder = await Microsoft.Windows.Storage.ApplicationData.GetDefault().LocalCacheFolder.CreateFolderAsync(CacheFolderName, CreationCollisionOption.OpenIfExists);

        // 优先使用传入的参数，如果没有则调用虚方法（仅在 UI 线程安全）
        var subFolder = cacheSubFolder ?? GetCacheSubFolder();
        if (!string.IsNullOrEmpty(subFolder))
        {
            cacheFolder = await cacheFolder.CreateFolderAsync(subFolder, CreationCollisionOption.OpenIfExists);
        }

        return cacheFolder;
    }
}
