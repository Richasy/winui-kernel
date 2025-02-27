using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Richasy.WinUIKernel.Share.Toolkits;

/// <summary>
/// Shared file toolkit.
/// </summary>
public class SharedFileToolkit : IFileToolkit
{
    /// <inheritdoc/>
    public async Task<StorageFile> PickFileAsync(string extension, object windowInstance)
    {
        var picker = new FileOpenPicker();
        InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(windowInstance));
        foreach (var ext in extension.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            picker.FileTypeFilter.Add(ext);
        }

        picker.SuggestedStartLocation = PickerLocationId.Desktop;
        return await picker.PickSingleFileAsync().AsTask();
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<StorageFile>?> PickMultipleFileAsync(string extension, object windowInstance)
    {
        try
        {
            var picker = new FileOpenPicker();
            InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(windowInstance));
            foreach (var ext in extension.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                picker.FileTypeFilter.Add(ext);
            }

            picker.SuggestedStartLocation = PickerLocationId.Desktop;
            return await picker.PickMultipleFilesAsync().AsTask();
        }
        catch (Exception)
        {
            return default;
        }
    }

    /// <inheritdoc/>
    public async Task<StorageFile?> SaveFileAsync(string extension, object windowInstance)
    {
        try
        {
            var picker = new FileSavePicker();
            InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(windowInstance));
            var exts = extension.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var ext in exts)
            {
                picker.FileTypeChoices.Add(ext, [ext]);
            }

            picker.SuggestedStartLocation = PickerLocationId.Desktop;
            return await picker.PickSaveFileAsync().AsTask();
        }
        catch (Exception)
        {
            return default;
        }
    }

    /// <inheritdoc/>
    public async Task<StorageFolder> PickFolderAsync(object windowInstance)
    {
        var picker = new FolderPicker();
        InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(windowInstance));
        picker.SuggestedStartLocation = PickerLocationId.Desktop;
        picker.FileTypeFilter.Add("*");
        return await picker.PickSingleFolderAsync().AsTask();
    }

    /// <inheritdoc/>
    public async Task<T> ReadLocalDataAsync<T>(string fileName, JsonTypeInfo<T> typeInfo, string defaultValue = "{}", string folderName = "")
    {
        var path = string.IsNullOrEmpty(folderName) ?
                        $"ms-appdata:///local/{fileName}" :
                        $"ms-appdata:///local/{folderName}/{fileName}";
        var content = defaultValue;
        try
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(path)).AsTask();
            var fileContent = await FileIO.ReadTextAsync(file).AsTask();

            if (!string.IsNullOrEmpty(fileContent))
            {
                content = fileContent;
            }
        }
        catch (FileNotFoundException)
        {
        }

        return (typeof(T) == typeof(string) ? (T)content.Clone() : JsonSerializer.Deserialize<T>(content, typeInfo))!;
    }

    /// <inheritdoc/>
    public async Task WriteLocalDataAsync<T>(string fileName, T data, JsonTypeInfo<T> typeInfo, string folderName = "")
    {
        var folder = ApplicationData.Current.LocalFolder;

        if (!string.IsNullOrEmpty(folderName))
        {
            folder = await folder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists).AsTask();
        }

        var writeContent = data is string ? data.ToString() : JsonSerializer.Serialize(data, typeInfo);
        var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists).AsTask();
        await FileIO.WriteTextAsync(file, writeContent).AsTask();
    }

    /// <inheritdoc/>
    public async Task DeleteLocalDataAsync(string fileName, string folderName = "")
    {
        var folder = ApplicationData.Current.LocalFolder;

        if (!string.IsNullOrEmpty(folderName))
        {
            folder = await folder.CreateFolderAsync(folderName).AsTask();
        }

        var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists).AsTask();
        await file.DeleteAsync().AsTask();
    }
}
