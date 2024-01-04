using Helpers.Interfaces;

namespace Services.Interfaces;

public interface IHashService
{
    IHashResult GetHash(string plainText, byte[]? salt = null);
    byte[] GetSalt();
}