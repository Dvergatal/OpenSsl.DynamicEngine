using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography.OpenSsl
{
    internal static class SafeNativeMethods
    {
#if WINDOWS
        private const string OpenSslLibrary = "libssleay32.dll";
#else
        private const string OpenSslLibrary = "libcrypto.so";
#endif

        [DllImport(OpenSslLibrary)]
        internal static extern void ERR_load_crypto_strings();

        [DllImport(OpenSslLibrary)]
        internal static extern void OPENSSL_add_all_algorithms_noconf();

        [DllImport(OpenSslLibrary)]
        internal static extern void ENGINE_load_builtin_engines();

        [DllImport(OpenSslLibrary)]
        internal static extern void ENGINE_register_all_complete();


        [DllImport(OpenSslLibrary)]
        internal static extern DynamicEngineHandle ENGINE_by_id(string name);

        [DllImport(OpenSslLibrary, CharSet = CharSet.Ansi)]
        internal static extern IntPtr ENGINE_get_name(DynamicEngineHandle engine);

        [DllImport(OpenSslLibrary)]
        internal static extern int ENGINE_get_flags(DynamicEngineHandle engine);

        [DllImport(OpenSslLibrary, CharSet = CharSet.Ansi)]
        internal static extern IntPtr ENGINE_get_id(DynamicEngineHandle engine);

        [DllImport(OpenSslLibrary)]
        internal static extern int ENGINE_init(DynamicEngineHandle engine);

        [DllImport(OpenSslLibrary)]
        internal static extern int ENGINE_finish(DynamicEngineHandle engine);

        [DllImport(OpenSslLibrary)]
        internal static extern void ENGINE_register_complete(DynamicEngineHandle engine);

        [DllImport(OpenSslLibrary)]
        internal static extern int ENGINE_set_default(DynamicEngineHandle engine, EngineDefaults defaults);

        [DllImport(OpenSslLibrary)]
        internal static extern void ENGINE_free(DynamicEngineHandle engine);

        static SafeNativeMethods()
        {
            ERR_load_crypto_strings();
            OPENSSL_add_all_algorithms_noconf();
            // prepare for dynamic engines
            ENGINE_load_builtin_engines();
            ENGINE_register_all_complete();
        }
    }
}
