namespace Services.Interfaces;

/// <summary>
/// Como preveemos que puedan haber diferentes versiones del GestorArchivos (local, nube, azure...) hemos creado
/// la interface IFileHandler para que todos los servicios de gestión de archivos la cumplan 
/// </summary>
public interface IFileHandler
{
    Task<string> Edit(byte[] content, string extension, string folder, string path,
        string contentType, string? name = null);

    Task Delete(string path, string folder);

    Task<string> Save(byte[] content, string extension, string folder, string contentType, string? name = null);
}