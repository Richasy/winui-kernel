// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Windows.Storage;

namespace Richasy.WinUIKernel.Share.Base;

public partial class ImageExBase
{
    private const string CacheFolderName = "ImageExCache";

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
    }

    /// <summary>
    /// 获取缓存文件的路径.
    /// </summary>
    /// <param name="key">源地址.</param>
    /// <returns></returns>
    public async Task<string?> GetCacheFilePathAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return null;
        }

        var cacheFolder = await GetCacheFolderAsync();
        var cacheFileName = GetCacheFileName(key);
        var cacheFilePath = Path.Combine(cacheFolder.Path, cacheFileName);
        if (File.Exists(cacheFilePath))
        {
            try
            {
                var fileInfo = new FileInfo(cacheFilePath);
                var createTime = fileInfo.CreationTimeUtc;
                if (DateTimeOffset.UtcNow - createTime > WinUIKernelShareExtensions.ImageCacheTime)
                {
                    File.Delete(cacheFilePath);
                    return null;
                }

                return cacheFilePath;
            }
            catch
            {
                // Ignore any exceptions during deletion
            }
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
    public async Task WriteCacheAsync(string key, byte[] data, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(key) || data == null || data.Length == 0)
        {
            return;
        }

        var cacheFolder = await GetCacheFolderAsync();
        var cacheFileName = GetCacheFileName(key);
        var cacheFilePath = Path.Combine(cacheFolder.Path, cacheFileName);
        // Write the data to the file
        await File.WriteAllBytesAsync(cacheFilePath, data, cancellationToken);
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

    private async Task<StorageFolder> GetCacheFolderAsync()
    {
        var cacheFolder = await Microsoft.Windows.Storage.ApplicationData.GetDefault().LocalCacheFolder.CreateFolderAsync(CacheFolderName, CreationCollisionOption.OpenIfExists);
        if (!string.IsNullOrEmpty(GetCacheSubFolder()))
        {
            cacheFolder = await cacheFolder.CreateFolderAsync(GetCacheSubFolder(), CreationCollisionOption.OpenIfExists);
        }

        return cacheFolder;
    }
}
