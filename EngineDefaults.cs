namespace System.Security.Cryptography
{
    [Flags]
    public enum EngineDefaults
    {
        None = 0,
        RSA = 1,
        DSA = 2,
        DH = 4,
        RandomNumberGeneration = 8,
        ECDH = 0x10,
        ECDSA = 0x20,
        Ciphers = 0x40,
        Digests = 0x80,
        Store = 0x0100,
        PKEY_METHS = 0x0200,
        PKEY_ASN1_METHS = 0x0400,
        All = 0xFFFF
    }
}
