namespace Interfaces;
public interface IHashService
{
    IHashResult GetHash(string plainText, byte[]? salt = null);
    byte[] GetSalt();
}