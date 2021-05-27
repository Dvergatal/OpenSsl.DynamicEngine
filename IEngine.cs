namespace System.Security.Cryptography.OpenSsl
{
    public interface IEngine : IDisposable
    {
        void SetDefaults(EngineDefaults defaults);
        void Finish();
    }
}