namespace HuertoApp.Services;

/// <summary>
/// Interfaz para servicios de manejo de imágenes (captura, selección y gestión).
/// </summary>
public interface IImageService
{
    /// <summary>
    /// Captura una foto usando la cámara del dispositivo.
    /// </summary>
    /// <returns>Resultado de la operación con la ruta de la imagen o error.</returns>
    Task<ImageResult> CapturePhotoAsync();
    /// <summary>
    /// Permite al usuario seleccionar una foto de la galería.
    /// </summary>
    /// <returns>Resultado de la operación con la ruta de la imagen o error.</returns>
    Task<ImageResult> PickPhotoAsync();
    /// <summary>
    /// Permite al usuario seleccionar una imagen del sistema de archivos.
    /// </summary>
    /// <returns>Resultado de la operación con la ruta de la imagen o error.</returns>
    Task<ImageResult> PickPhotoFromFileSystemAsync();
    /// <summary>
    /// Elimina una imagen del sistema de archivos.
    /// </summary>
    /// <param name="path">Ruta de la imagen a eliminar.</param>
    /// <returns>True si se eliminó correctamente, false en caso contrario.</returns>
    Task<bool> DeleteImageAsync(string path);
    /// <summary>
    /// Obtiene la ruta de visualización de una imagen, retornando una imagen por defecto si no existe.
    /// </summary>
    /// <param name="path">Ruta de la imagen.</param>
    /// <returns>Ruta de la imagen o ruta de imagen por defecto.</returns>
    string GetImageDisplayPath(string path);
}

/// <summary>
/// Implementación del servicio de manejo de imágenes.
/// </summary>
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

/// <summary>
/// Representa el resultado de una operación de manejo de imágenes.
/// </summary>
public class ImageResult
{
    /// <summary>
    /// Indica si la operación fue exitosa.
    /// </summary>
    public bool IsSuccess { get; private set; }
    /// <summary>
    /// Indica si el usuario canceló la operación.
    /// </summary>
    public bool IsCancelled { get; private set; }
    /// <summary>
    /// Ruta de la imagen resultante.
    /// </summary>
    public string ImagePath { get; private set; }
    /// <summary>
    /// Mensaje de error en caso de fallo.
    /// </summary>
    public string ErrorMessage { get; private set; }

    private ImageResult(bool isSuccess, bool isCancelled, string imagePath = "", string errorMessage = "")
    {
        IsSuccess = isSuccess;
        IsCancelled = isCancelled;
        ImagePath = imagePath;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Crea un resultado exitoso con la ruta de la imagen.
    /// </summary>
    /// <param name="imagePath">Ruta de la imagen.</param>
    /// <returns>Resultado exitoso.</returns>
    public static ImageResult Success(string imagePath) => new ImageResult(true, false, imagePath);
    /// <summary>
    /// Crea un resultado de fallo con un mensaje de error.
    /// </summary>
    /// <param name="errorMessage">Mensaje de error.</param>
    /// <returns>Resultado de fallo.</returns>
    public static ImageResult Failure(string errorMessage) => new ImageResult(false, false, "", errorMessage);
    /// <summary>
    /// Crea un resultado indicando que el usuario canceló la operación.
    /// </summary>
    /// <returns>Resultado de cancelación.</returns>
    public static ImageResult Cancelled() => new ImageResult(false, true);
}
