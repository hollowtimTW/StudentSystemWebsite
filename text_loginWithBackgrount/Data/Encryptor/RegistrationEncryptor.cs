using System.Security.Cryptography;
using System.Text;

namespace text_loginWithBackgrount.Data.Encryptor
{
    public class RegistrationEncryptor : IRegistrationEncryptor
    {
        public (string encryptedPassword, string salt) EncryptPassword(string password)
        {
            string salt = GenerateSalt();
            string encryptedPassword = HashPassword(password, salt);
            return (encryptedPassword, salt);
        }

        //創造鹽 (亂碼)
        private string GenerateSalt()
        {
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                byte[] salt = new byte[16];
                rng.GetBytes(salt);
                return Convert.ToBase64String(salt);//轉成base64字串
            }
        }

        //丟入密碼和鹽創造最終驗證Hash
        public string HashPassword(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())// SHA-256 雜湊
            {
                byte[] combined = Encoding.UTF8.GetBytes(password + salt);
                byte[] hashedBytes = sha256.ComputeHash(combined);
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
