using System;
using Ssc.Ssc;
using Ssc.SscLog;
using Ssc.SscSerialization;
using Ssf.Ssf;
using Ssf.SsfConfiguration;
using Ssf.SsfManager;
using Ssf.SsfSecurity;
using Sss.SssComponent;
using Sss.SssScripts;
using Sss.SssScripts.Lua;

namespace Ssf {
    public partial class Ssfi {
        public static NetworkConfig NetworkConfig { get; }
        public static CryptoConfig CryptoConfig { get; }
    }

    public partial class Ssfi {
        static Ssfi() {
            NetworkConfig = new NetworkConfig();
            CryptoConfig = new CryptoConfig();

            Security = new Security();
        }

        public static string Version { get; } = "v2.9";

        internal static ISecurity Security { get; }

        public static void Initialize() {

            Ssc.Ssci.Initialize();
            LuaRegister();
            
            var logAppenders = new SsfLogAppenders();
            LogManager.Initialize(logAppenders.ConsoleAppender);
        }

        public static void LuaRegister() {
            // System
            LuaHelper.RegisterType<Type>();
            LuaHelper.RegisterType<LuaHelper>();

            LuaHelper.RegisterType<IPeer>();
            LuaHelper.RegisterType<Serializable>();
            LuaHelper.RegisterType<EndianBinaryReader>();
            LuaHelper.RegisterType<EndianBinaryWriter>();

            // Lua
            LuaHelper.RegisterType<LuaPeerComponent>();
            LuaHelper.RegisterType<LuaControllerComponent>();
            LuaHelper.RegisterType<CacheManager>();
            LuaHelper.RegisterType<ControllerComponentManager>();
            LuaHelper.RegisterType<ResponseCallback>();
            LuaHelper.RegisterType<IResponseMessage>();
            LuaHelper.RegisterType<ISerializable>();
            LuaHelper.RegisterType<IDeserializable>();
            LuaHelper.RegisterType<ClassWrapper<LuaPeerComponent>>();
            LuaHelper.RegisterType<ClassWrapper<LuaControllerComponent>>();
        }
    }
}