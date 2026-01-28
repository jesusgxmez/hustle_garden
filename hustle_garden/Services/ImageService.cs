namespace HuertoApp.Services;

public interface IImageService
{
    Task<ImageResult> CapturePhotoAsync();
    Task<ImageResult> PickPhotoAsync();
    Task<ImageResult> PickPhotoFromFileSystemAsync();
    Task<bool> DeleteImageAsync(string path);
    string GetImageDisplayPath(string path);
}

public class ImageService : IImageService
{
    private const long MAX_IMAGE_SIZE = 10 * 1024 * 1024; // 10MB

    public async Task<ImageResult> CapturePhotoAsync()
    {
        try
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                return ImageResult.Failure("La cámara no está disponible en este dispositivo");
            }

            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo == null)
            {
                return ImageResult.Cancelled();
            }

            return await SavePhotoAsync(photo);
        }
        catch (PermissionException)
        {
            return ImageResult.Failure("No se tienen permisos para usar la cámara");
        }
        catch (Exception ex)
        {
            return ImageResult.Failure($"Error al capturar foto: {ex.Message}");
        }
    }

    public async Task<ImageResult> PickPhotoAsync()
    {
        try
        {
            var photo = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Selecciona una foto"
            });

            if (photo == null)
            {
                return ImageResult.Cancelled();
            }

            return await SavePhotoAsync(photo);
        }
        catch (PermissionException)
        {
            return ImageResult.Failure("No se tienen permisos para acceder a la galería");
        }
        catch (Exception ex)
        {
            return ImageResult.Failure($"Error al seleccionar foto: {ex.Message}");
        }
    }

    public async Task<ImageResult> PickPhotoFromFileSystemAsync()
    {
        try
        {
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { "public.image" } },
                { DevicePlatform.Android, new[] { "image/*" } },
                { DevicePlatform.WinUI, new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" } },
                { DevicePlatform.macOS, new[] { "jpg", "jpeg", "png", "gif", "bmp" } }
            });

            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Selecciona una imagen",
                FileTypes = customFileType
            });

            if (result == null)
            {
                return ImageResult.Cancelled();
            }

            using var stream = await result.OpenReadAsync();
            if (stream.Length > MAX_IMAGE_SIZE)
            {
                return ImageResult.Failure("La imagen no puede superar 10MB");
            }

            var localPath = Path.Combine(FileSystem.AppDataDirectory, $"{Guid.NewGuid()}{Path.GetExtension(result.FileName)}");
            
            using var fileStream = File.OpenWrite(localPath);
            stream.Seek(0, SeekOrigin.Begin);
            await stream.CopyToAsync(fileStream);

            return ImageResult.Success(localPath);
        }
        catch (Exception ex)
        {
            return ImageResult.Failure($"Error al seleccionar archivo: {ex.Message}");
        }
    }

    public async Task<bool> DeleteImageAsync(string path)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return false;
            }

            await Task.Run(() => File.Delete(path));
            return true;
        }
        catch
        {
            return false;
        }
    }

    public string GetImageDisplayPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return "default_plant.png"; 
        }

        return File.Exists(path) ? path : "default_plant.png";
    }

    private async Task<ImageResult> SavePhotoAsync(FileResult photo)
    {
        try
        {
            using var stream = await photo.OpenReadAsync();
            if (stream.Length > MAX_IMAGE_SIZE)
            {
                return ImageResult.Failure("La imagen no puede superar 10MB");
            }

            var localPath = Path.Combine(FileSystem.AppDataDirectory, $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}");

            using var fileStream = File.OpenWrite(localPath);
            stream.Seek(0, SeekOrigin.Begin);
            await stream.CopyToAsync(fileStream);

            return ImageResult.Success(localPath);
        }
        catch (Exception ex)
        {
            return ImageResult.Failure($"Error al guardar imagen: {ex.Message}");
        }
    }
}

public class ImageResult
{
    public bool IsSuccess { get; private set; }
    public bool IsCancelled { get; private set; }
    public string ImagePath { get; private set; }
    public string ErrorMessage { get; private set; }

    private ImageResult(bool isSuccess, bool isCancelled, string imagePath = "", string errorMessage = "")
    {
        IsSuccess = isSuccess;
        IsCancelled = isCancelled;
        ImagePath = imagePath;
        ErrorMessage = errorMessage;
    }

    public static ImageResult Success(string imagePath) => new ImageResult(true, false, imagePath);
    public static ImageResult Failure(string errorMessage) => new ImageResult(false, false, "", errorMessage);
    public static ImageResult Cancelled() => new ImageResult(false, true);
}
