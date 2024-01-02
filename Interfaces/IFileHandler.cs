namespace Interfaces;

public interface IFileHandler
{
    Task<string> Edit(byte[] content, string extension, string folder, string path,
        string contentType);

    Task Delete(string path, string folder);

    Task<string> Save(byte[] content, string extension, string folder, string contentType);
}