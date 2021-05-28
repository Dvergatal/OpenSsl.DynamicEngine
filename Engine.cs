using System.Text;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace System.Security.Cryptography
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
                if (result == 0) 
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

        public virtual void Login(string pin)
        {
            if (SafeNativeMethods.ENGINE_ctrl_cmd_string(engine, "PIN", pin, 0) != 1)
                throw new InvalidOperationException("engine: unable to log in with PIN='{pin}'");
            
            System.Console.WriteLine("Logged into token!!!!!!!!!!!!");
        }

        public virtual SafeEvpPKeyHandle GetPrivKey(string label)
        {
            string keyId = "label_" + label;
            SafeEvpPKeyHandle pkey = SafeNativeMethods.ENGINE_load_private_key(engine, keyId, IntPtr.Zero, IntPtr.Zero);
            if(pkey.IsInvalid)
            {
                 throw new InvalidOperationException("engine: unable to find private key with label='{label}'");
            }

            return pkey;
        }
        public virtual string GetCSR(SafeEvpPKeyHandle pkey, string ext, HashAlgorithmName name)
        {
            // FIXME: determine key type
            RSA rsa = new RSAOpenSsl(pkey);
            CertificateRequest req = new CertificateRequest("CN=potato", rsa, name, RSASignaturePadding.Pkcs1); // this method is only for RSA key different is for EC, DSA etc.
            byte[] requestDer = req.CreateSigningRequest();
            string requestPem = new string(PemEncoding.Write("CERTIFICATE REQUEST", requestDer));
            return requestPem;
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