using System.Security.Cryptography;
using System.Text;

namespace c1Soft_Projesi.Services;

public class PasswordService
{
    public string Hash(string password)
    {
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        byte[] hashBytes = SHA256.HashData(passwordBytes);

        return Convert.ToHexString(hashBytes);
    }

    public bool Verify(string password, string passwordHash)
    {
        string newHash = Hash(password);
        return newHash == passwordHash;
    }
}
