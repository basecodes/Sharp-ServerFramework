using System;
using System.Collections.Generic;
using Ssc.SscException;
using Ssc.SscLog;
using Ssc.SscSerialization;

namespace Ssc.SscRpc {
    public class PacketManager {
        private static readonly Dictionary<string, ObjectActivator<ISerializablePacket>> _objectGenerators;
        private static readonly Dictionary<string, Type> _packetTypes;
        private static readonly Logger Logger = LogManager.GetLogger<PacketManager>(LogType.Middle);

        static PacketManager() {
            _objectGenerators = new Dictionary<string, ObjectActivator<ISerializablePacket>>();
            _packetTypes = new Dictionary<string, Type>();
        }

        public static void Register(string interfaceName, Type interfaceType,
            ObjectActivator<ISerializablePacket> objectGenerator) {
            if (string.IsNullOrEmpty(interfaceName)) {
                throw new ArgumentException(nameof(interfaceName));
            }

            if (_objectGenerators.ContainsKey(interfaceName)) {
                Logger.Warn($"已经注册过{interfaceName}类！");
                return;
            }

            _objectGenerators[interfaceName] = objectGenerator;
            _packetTypes[interfaceName] = interfaceType ?? throw new ArgumentNullException(nameof(interfaceType));
        }

        public static void Register<Interface>(ObjectActivator<ISerializablePacket> objectGenerator)
            where Interface : class {
            Register(typeof(Interface).Name, typeof(Interface), objectGenerator);
        }
        
        public static void Register(Type type,ObjectActivator<ISerializablePacket> objectGenerator) {
            if (type == null) {
                throw new ArgumentNullException(nameof(type));
            }
            if (objectGenerator == null) {
                throw new ArgumentNullException(nameof(objectGenerator));
            }
            Register(type.Name, type, objectGenerator);
        }

        public static ISerializablePacket CreatePacket(string interfaceName) {
            if (string.IsNullOrEmpty(interfaceName)) {
                throw new ArgumentNullException(nameof(interfaceName));
            }

            if (!_objectGenerators.TryGetValue(interfaceName, out var objectGenerator)){
                throw new UnregisteredException(interfaceName);
            }

            return objectGenerator();
        }

        public static ISerializablePacket CreatePacket(string interfaceName, params object[] args) {
            if (string.IsNullOrEmpty(interfaceName)) {
                throw new ArgumentNullException(nameof(interfaceName));
            }

            if (!_objectGenerators.TryGetValue(interfaceName, out var objectGenerator)) {
                throw new UnregisteredException(interfaceName);
            }

            return objectGenerator(args);
        }

        public static ISerializablePacket CreatePacket(Type interfaceType) {
            return CreatePacket(interfaceType.Name);
        }

        public static ISerializablePacket CreatePacket(Type interfaceType, params object[] args) {
            return CreatePacket(interfaceType.Name, args);
        }

        public static Implement CreatePacket<Implement>(string interfaceName)
            where Implement : class,ISerializablePacket {
            return CreatePacket(interfaceName) as Implement;
        }

        public static Implement CreatePacket<Implement>(string interfaceName, params object[] args)
            where Implement : class,ISerializablePacket {
            return CreatePacket(interfaceName, args) as Implement;
        }

        public static Interface CreatePacket<Interface>()
            where Interface : class {
            return CreatePacket(typeof(Interface)) as Interface;
        }

        public static Interface CreatePacket<Interface>(params object[] args)
            where Interface : class {
            return CreatePacket(typeof(Interface), args) as Interface;
        }

        public static Implement CreatePacket<Interface, Implement>(params object[] args)
            where Interface : class
            where Implement : class,ISerializablePacket, Interface {
            return CreatePacket<Interface>(args) as Implement;
        }

        public static bool RemovePacket(string interfaceName) {
            if (string.IsNullOrEmpty(interfaceName)) {
                throw new ArgumentNullException(nameof(interfaceName));
            }
            return _packetTypes.Remove(interfaceName) && _objectGenerators.Remove(interfaceName);
        }

        public static Type GetPacketType(string interfaceName) {
            if (!_packetTypes.TryGetValue(interfaceName, out var value)) {
                Logger.Warn($"{interfaceName}接口类型未注册！");
            }
            return value;
        }

        public static bool Cantains(string interfaceName) {
            return _objectGenerators.ContainsKey(interfaceName);
        }
    }
}