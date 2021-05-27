using System.Runtime.InteropServices;

namespace System.Security.Cryptography.OpenSsl
{
    public abstract class Engine : IEngine
    {
        protected DynamicEngineHandle engine { get; set; }

        protected string Id { get; set; }

        protected string ModulePath { get; set; }

        protected bool initialized { get; set; } = false;

        internal abstract void Initialize();

        public virtual void SetDefaults(EngineDefaults defaults)
        {
            if (defaults == EngineDefaults.All ||
                defaults == (defaults & (
                    EngineDefaults.RSA |
                    EngineDefaults.DSA |
                    EngineDefaults.DH |
                    EngineDefaults.RandomNumberGeneration |
                    EngineDefaults.ECDH |
                    EngineDefaults.ECDSA |
                    EngineDefaults.Ciphers |
                    EngineDefaults.Digests |
                    EngineDefaults.Store |
                    EngineDefaults.PKEY_METHS |
                    EngineDefaults.PKEY_ASN1_METHS)))
            {
                var result = SafeNativeMethods.ENGINE_set_default(engine, defaults);
                if (0 == result)
                {
                    SafeNativeMethods.ENGINE_free(engine);
                    throw new InvalidOperationException($"Unable to set engine as default '{defaults}'. ENGINE_set_default returned {result}");
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(defaults));
            }
        }

        internal string GetEngineId()
        {
            if (initialized)
            {
                if (engine.IsClosed || engine.IsInvalid)
                    throw new InvalidOperationException();
                var id = SafeNativeMethods.ENGINE_get_id(engine);
                return Marshal.PtrToStringAuto(id);
            }

            return Id;
        }

        public abstract void Finish();

        public abstract void Dispose();
    }
}