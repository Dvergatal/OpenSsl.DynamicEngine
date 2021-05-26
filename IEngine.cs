namespace System.Security.Cryptography.OpenSsl
{
    public interface IEngine
    {
        DynamicEngineHandle engine { get; }
        void Initialize();
        void Finish();
        void SetDefaults(EngineDefaults defaults);
    }
}