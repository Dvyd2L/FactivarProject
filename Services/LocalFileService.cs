using Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Services;

/// <summary>
/// Clase que maneja las operaciones de archivos en el servidor local.
/// Implementa la interfaz IFileHandler.
/// </summary>
/// <param name="env">Una instancia de IWebHostEnvironment para acceder al entorno de alojamiento web.</param>
/// <param name="httpContextAccessor">Una instancia de IHttpContextAccessor para acceder al contexto HTTP actual.</param>
public class LocalFileService(IWebHostEnvironment env,
    IHttpContextAccessor httpContextAccessor) : IFileHandler
{
    #region PROPs
    /// <summary>
    /// Para conocer la configuración del servidor para construir la url de la imagen
    /// </summary>
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    /// <summary>
    /// Para poder localizar la carpeta de contenido estático wwwroot
    /// </summary>
    private readonly IWebHostEnvironment _env = env;
    #endregion PROPs

    #region METHODs
    /// <summary>
    /// Elimina un archivo en el servidor local.
    /// </summary>
    /// <param name="path">La ruta del archivo a eliminar.</param>
    /// <param name="folder">La carpeta donde se encuentra el archivo.</param>
    /// <returns>Una tarea que representa la operación de eliminación asíncrona.</returns>
    public Task Delete(
        string path,
        string folder
        )
    {
        if (path is not null)
        {
            string nombreArchivo = Path.GetFileName(path);
            string directorioArchivo = Path.Combine(_env.WebRootPath, folder, nombreArchivo);

            if (File.Exists(directorioArchivo))
            {
                File.Delete(directorioArchivo);
            }
        }

        return Task.FromResult(0);
    }

    /// <summary>
    /// Edita un archivo existente o crea uno nuevo si no existe.
    /// </summary>
    /// <param name="content">El contenido del archivo en bytes.</param>
    /// <param name="extension">La extensión del archivo.</param>
    /// <param name="folder">La carpeta donde se guardará el archivo.</param>
    /// <param name="path">La ruta del archivo a editar.</param>
    /// <param name="contentType">El tipo de contenido del archivo.</param>
    /// <returns>La URL del archivo editado.</returns>
    public async Task<string> Edit(
        byte[] content,
        string extension,
        string folder,
        string path,
        string contentType,
        string? name = null
        )
    {
        await Delete(path, folder);

        return await Save(content, extension, folder, contentType, name);
    }

    /// <summary>
    /// Guarda un archivo en el servidor local.
    /// </summary>
    /// <param name="content">El contenido del archivo en bytes.</param>
    /// <param name="extension">La extensión del archivo.</param>
    /// <param name="folder">La carpeta donde se guardará el archivo.</param>
    /// <param name="contentType">El tipo de contenido del archivo.</param>
    /// <returns>La URL del archivo guardado.</returns>
    public async Task<string> Save(
        byte[] content,
        string extension,
        string folder,
        string contentType,
        string? name = null
        )
    {
        // En caso de no recibir nombre cCreamos un nombre aleatorio con la extensión
        name ??= Guid.NewGuid().ToString();
        string fileName = $"{name}_{contentType}{extension}";

        // La ruta será wwwroot/folder (en este caso imagenes)
        string directory = Path.Combine(_env.WebRootPath, folder);

        // Si no existe la carpeta la creamos
        if (!Directory.Exists(directory))
        {
            _ = Directory.CreateDirectory(directory);
        }

        // La ruta donde dejaremos el archivo será la concatenación de la ruta de la carpeta y el nombre del archivo
        string path = Path.Combine(directory, fileName);

        // Guardamos el archivo
        await File.WriteAllBytesAsync(path, content);

        // La url de la ímagen será http o https://dominio/carpeta/nombreimagen
        string currentURL = $"{_httpContextAccessor.HttpContext?.Request.Scheme}://{_httpContextAccessor.HttpContext?.Request.Host}";
        string dbURL = Path.Combine(currentURL, directory, fileName).Replace("\\", "/");

        return dbURL;
    }
    #endregion METHODs
}
