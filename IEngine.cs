namespace System.Security.Cryptography
{
    public interface IEngine : IDisposable
    {
        void SetDefaults(EngineDefaults defaults);

        void Login(string pin);

        SafeEvpPKeyHandle GetPrivKey(string label);

        string GetCSR(SafeEvpPKeyHandle pkey, string ext, HashAlgorithmName name);

        void Finish();
    }
}