using System.IO;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography.OpenSsl
{
    public class TokenEngine : IEngine, IDisposable
    {
        // private DynamicEngineHandle engine;
        public DynamicEngineHandle engine { get; private set; }
        public TokenEngine(string modulePath)
        {
            ModulePath = modulePath;
        }
        public string Id
        {
            get
            {
                if (engine.IsClosed || engine.IsInvalid) throw new InvalidOperationException();
                var id = SafeNativeMethods.ENGINE_get_id(engine);
                return Marshal.PtrToStringAuto(id);
            }
        }
        public string ModulePath { get; private set; }

        public void Initialize()
        {
            if (null == engine)
            {
                engine = SafeNativeMethods.ENGINE_by_id("pkcs11");
                if (engine.IsInvalid)
                {
                    throw new InvalidOperationException($"Unable to load pkcs11 engine");
                }

                if(!File.Exists(ModulePath))
                {
                    throw new InvalidOperationException($"Unable to load pkcs11 module path");
                }

                //    DEBUG( "token: ctor: module_path=" << QS( modulePath ) );
                if ( 1 != SafeNativeMethods.ENGINE_ctrl_cmd_string(engine, "MODULE_PATH", ModulePath, 0))
                {
                    throw new InvalidOperationException("token: setting module_path <= '{ModulePath}'");
                }

                //    DEBUG( "token: ctor: initializing " << m_pEngine );
                var result = SafeNativeMethods.ENGINE_init(engine);
                if (0 == result)
                {
                    SafeNativeMethods.ENGINE_free(engine);
                    throw new InvalidOperationException($"Unable to load pkcs11 engine '{ModulePath}'. ENGINE_init returned {result}");
                }
            }
        }

        public void Finish()
        {
            SafeNativeMethods.ENGINE_finish(engine);
        }

        public void SetDefaults(EngineDefaults defaults)
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                engine.Dispose();
            }
        }
    }
}
