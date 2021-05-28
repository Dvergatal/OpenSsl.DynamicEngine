
namespace System.Security.Cryptography
{  
    /// <summary>  
    /// The Engine Factory Class  
    /// </summary>  
    public static class EngineFactory
    {
        public struct OpenSSLConfig
        {
            public string EngineId { get; set; }
            public string EnginePath { get; set; }
            public string ModulePath { get; set; }
        }
        public static IEngine GetEngine(OpenSSLConfig config)
        {
            Engine engine = config switch
            {
                {EnginePath: string enginePath} when !string.IsNullOrEmpty(enginePath) => new DynamicEngine(config.EngineId,
                    enginePath, config.ModulePath),
                {EngineId: "pkcs11"}  => new TokenEngine(config.ModulePath),
                _ => throw new InvalidOperationException($"Unable to load engine")
            };

            engine.Initialize();
            engine.SetDefaults(EngineDefaults.All); // <- Select the engine features you intend to use
            System.Console.WriteLine("Engine {0} initialized !!!!!!!!!!!!", (string)engine.GetEngineId());

            return engine;
        }
    }  
}  
