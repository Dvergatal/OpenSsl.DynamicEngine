using System.IO;

namespace System.Security.Cryptography
{
    public class TokenEngine : Engine
    {
        public TokenEngine(string modulePath)
        {
            ModulePath = modulePath;
        }

        internal override void Initialize()
        {
            if (engine == null)
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

                if(SafeNativeMethods.ENGINE_ctrl_cmd_string(engine, "MODULE_PATH", ModulePath, 0) != 1)
                {
                    throw new InvalidOperationException("token: setting module_path <= '{ModulePath}'");
                }

                var result = SafeNativeMethods.ENGINE_init(engine);
                if (result == 0)
                {
                    SafeNativeMethods.ENGINE_free(engine);
                    throw new InvalidOperationException($"Unable to load pkcs11 engine '{ModulePath}'. ENGINE_init returned {result}");
                }

                initialized = true;
            }
        }

        public override void Finish()
        {
            SafeNativeMethods.ENGINE_finish(engine);
        }

        public override void Dispose()
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
