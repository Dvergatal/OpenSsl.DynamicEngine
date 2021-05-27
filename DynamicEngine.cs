using System.IO;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography.OpenSsl
{
    public class DynamicEngine : Engine
    {
        public DynamicEngine(string id, string enginePath, string modulePath = null)
        {
            Id = id;
            EnginePath = enginePath;
            ModulePath = modulePath;
        }
        
        private string EnginePath { get; set; }

        internal override void Initialize()
        {
            if (engine == null)
            {
                engine = SafeNativeMethods.ENGINE_by_id("dynamic");
                if (engine.IsInvalid)
                {
                    throw new InvalidOperationException($"Unable to load dynamic engine");
                }

                if (!File.Exists(EnginePath))
                {
                    throw new InvalidOperationException($"Unable to find engine library path");
                }

                if (SafeNativeMethods.ENGINE_ctrl_cmd_string(engine, "SO_PATH", EnginePath, 0) != 1)
                {
                    throw new InvalidOperationException("dynamic: setting so_path <= '{EnginePath}'");
                }

                if (SafeNativeMethods.ENGINE_ctrl_cmd_string(engine, "ID", Id, 0) != 1)
                {
                    throw new InvalidOperationException("dynamic: setting engine id <= '{id}'");
                }

                if (SafeNativeMethods.ENGINE_ctrl_cmd(engine, "LIST_ADD", 1, IntPtr.Zero, null, 0) != 1)
                {
                    throw new InvalidOperationException("dynamic: setting list_add <= 1");
                }

                if (SafeNativeMethods.ENGINE_ctrl_cmd(engine, "LOAD", 1, IntPtr.Zero, null, 0) != 1)
                {
                    throw new InvalidOperationException("dynamic: setting load <= 1");
                }

                if(Id == "pkcs11")
                {
                    if(!File.Exists(ModulePath))
                    {
                        throw new InvalidOperationException($"Unable to load pkcs11 module path");
                    }

                    if(SafeNativeMethods.ENGINE_ctrl_cmd_string(engine, "MODULE_PATH", ModulePath, 0) != 1)
                    {
                        throw new InvalidOperationException("dynamic: setting module_path <= '{ModulePath}'");
                    }
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
