namespace text_loginWithBackgrount.Data.Encryptor
{
    public interface IRegistrationEncryptor
    {
        //註冊時加密
        (string encryptedPassword, string salt) EncryptPassword(string password);

        //放鹽和密碼去組hash
        string HashPassword(string password, string salt);
    }
}
