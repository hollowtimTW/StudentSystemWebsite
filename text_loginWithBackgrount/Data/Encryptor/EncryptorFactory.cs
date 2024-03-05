namespace text_loginWithBackgrount.Data.Encryptor
{
    public static class EncryptorFactory
    {
        public static IRegistrationEncryptor CreateEncryptor()
        {
            return new RegistrationEncryptor();
        }
    }
}
