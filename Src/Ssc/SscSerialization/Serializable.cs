using System;
using System.Collections;
using System.Collections.Generic;
using Ssc.SscLog;
using Ssc.SscStream;
using Ssc.SscTemplate;

namespace Ssc.SscSerialization {
    public struct Serializable: ISerializable {
        private static readonly Logger Logger = LogManager.GetLogger<Serializable>(LogType.Middle);

        public IWriteStream WriteStream { get; }
        private EndianBinaryWriter _endianBinaryWriter;

        public Serializable(object _ = null) {
            WriteStream = PoolAllocator<IWriteStream>.GetObject();
            _endianBinaryWriter = new EndianBinaryWriter(WriteStream);
            WriteStream.ShiftRight((byte)0);
        }

        public void Dispose() {
            WriteStream.Dispose();
        }

        private void Increment() {
            var byteFragment = WriteStream.ToByteFragment();
            byteFragment.Buffer[byteFragment.Offset] = (byte)(byteFragment.Buffer[byteFragment.Offset] + 1);
        }

        public ISerializable SerializeNull() {
            Increment();

            WriteStream.ShiftRight((byte)FieldType.NullType);
            return this;
        }

        #region Serialize

        public ISerializable Serialize<T>(T value) where T : IConvertible {
            Increment();

            WriteStream.ShiftRight((byte)FieldType.BaseType);
            _endianBinaryWriter.Write(value);
            return this;
        }

        public ISerializable Serialize(TypeCode typeCode, IConvertible value) {
            Increment();

            WriteStream.ShiftRight((byte)FieldType.BaseType);
            _endianBinaryWriter.Write(typeCode, value);
            return this;
        }

        #endregion Serialize

        #region SerializePacket

        public ISerializable SerializePacket<T>(T packet) where T : class ,ISerializablePacket{
            Increment();

            WriteStream.ShiftRight((byte)FieldType.PacketType);
            _endianBinaryWriter.WritePacket(packet);
            return this;
        }

        public ISerializable SerializePacket(string interfaceName, ISerializablePacket packet) {
            Increment();

            WriteStream.ShiftRight((byte)FieldType.PacketType);
            _endianBinaryWriter.WritePacket(interfaceName, packet);
            return this;
        }

        #endregion SerializePacket

        #region SerializeArray

        public ISerializable SerializeArray<T>(T[] array) where T : IConvertible {
            Increment();

            WriteStream.ShiftRight((byte)FieldType.ArrayBase);
            _endianBinaryWriter.WriteArray(array);
            return this;
        }

        public ISerializable SerializeArray(TypeCode typeCode, IConvertible[] array) {
            Increment();

            WriteStream.ShiftRight((byte)FieldType.ArrayBase);
            _endianBinaryWriter.WriteArray(typeCode, array);
            return this;
        }

        #endregion SerializeArray

        #region SerializeArray2

        public ISerializable SerializePacketArray<T>(T[] array) where T : class,ISerializablePacket {
            Increment();

            WriteStream.ShiftRight((byte)FieldType.ArrayPacket);
            _endianBinaryWriter.WritePacketArray(array);
            return this;
        }

        public ISerializable SerializePacketArray(string interfaceName, ISerializablePacket[] array) {
            Increment();

            WriteStream.ShiftRight((byte)FieldType.ArrayPacket);
            _endianBinaryWriter.WritePacketArray(interfaceName, array);
            return this;
        }

        #endregion SerializeArray2

        #region SerializeDictionary

        public ISerializable SerializeDictionaryBB<K, V>(Dictionary<K, V> dictionary)
            where K : IConvertible
            where V : IConvertible {
            Increment();

            WriteStream.ShiftRight((byte)FieldType.DictKBVB);
            _endianBinaryWriter.WriteDictionaryBB(dictionary);
            return this;
        }

        public ISerializable SerializeDictionaryBB(TypeCode keyType, TypeCode valueType,
            Dictionary<IConvertible, IConvertible> dictionary) {
            Increment();

            WriteStream.ShiftRight((byte)FieldType.DictKBVB);
            _endianBinaryWriter.WriteDictionaryBB(keyType, valueType, dictionary);
            return this;
        }

        #endregion SerializeDictionary

        #region SerializeDictionary2

        public ISerializable SerializeDictionaryBP<K, V>(Dictionary<K, V> dictionary)
            where K : IConvertible
            where V : class,ISerializablePacket {
            Increment();

            WriteStream.ShiftRight((byte)FieldType.DictKBVP);
            _endianBinaryWriter.WriteDictionaryBP(dictionary);
            return this;
        }


        public ISerializable SerializeDictionaryBP(TypeCode keyType, string interfaceName,
            Dictionary<IConvertible, ISerializablePacket> dictionary) {
            Increment();

            WriteStream.ShiftRight((byte)FieldType.DictKBVP);
            _endianBinaryWriter.WriteDictionaryBP(keyType, interfaceName, dictionary);
            return this;
        }

        #endregion SerializeDictionary2

        #region SerializeDictionary3

        public ISerializable SerializeDictionaryPP<K, V>(Dictionary<K, V> dictionary)
            where K : class,ISerializablePacket
            where V : class,ISerializablePacket{
            Increment();

            WriteStream.ShiftRight((byte)FieldType.DictKPVP);
            _endianBinaryWriter.WriteDictionaryPP(dictionary);
            return this;
        }

        public ISerializable SerializeDictionaryPP(string keyInterfaceName, string valueInterfaceName,
            Dictionary<ISerializablePacket, ISerializablePacket> dictionary) {
            Increment();

            WriteStream.ShiftRight((byte)FieldType.DictKPVP);
            _endianBinaryWriter.WriteDictionaryPP(keyInterfaceName, valueInterfaceName, dictionary);
            return this;
        }

        #endregion SerializeDictionary3

        #region SerializeDictionary4

        public ISerializable SerializeDictionaryPB<K, V>(Dictionary<K, V> dictionary)
            where K : class,ISerializablePacket
            where V : IConvertible {
            Increment();

            WriteStream.ShiftRight((byte)FieldType.DictKPVB);
            _endianBinaryWriter.WriteDictionaryPB(dictionary);
            return this;
        }

        public ISerializable SerializeDictionaryPB(string interfaceName, TypeCode keyType,
            Dictionary<ISerializablePacket, IConvertible> dictionary) {
            Increment();

            WriteStream.ShiftRight((byte)FieldType.DictKPVB);
            _endianBinaryWriter.WriteDictionaryPB(interfaceName, keyType, dictionary);
            return this;
        }

        #endregion SerializeDictionary4

        public void SerializableObject(Type type, object obj) {
            if (type == null || obj == null) {
                WriteStream.ShiftRight((byte)FieldType.NullType);
                return;
            }

            if (obj is IConvertible baseValue) {
                Serialize(Type.GetTypeCode(type), baseValue);
                return;
            }

            if (obj is ISerializablePacket packet) {
                SerializePacket(packet.TypeName, packet);
                return;
            }

            if (obj is Array array) {
                var elementType = type.GetElementType();
                if (typeof(IConvertible).IsAssignableFrom(elementType)) {
                    var baseArray = new IConvertible[array.Length];
                    for (var i = 0; i < baseArray.Length; i++) {
                        baseArray[i] = (IConvertible)array.GetValue(i);
                    }
                    SerializeArray(Type.GetTypeCode(elementType), baseArray);
                    return;
                }

                if(typeof(ISerializablePacket).IsAssignableFrom(elementType)){
                    var packets = new ISerializablePacket[array.Length];
                    var typeName = "";
                    for (var i = 0; i < packets.Length; i++) {
                        var pkt = array.GetValue(i);
                        if (pkt is ISerializablePacket serializablePacket) {
                            packets[i] = serializablePacket;
                            typeName = serializablePacket.TypeName;
                        }
                    }
                    SerializePacketArray(typeName, packets);
                }

                return;
            }

            if (obj is IDictionary dict) {
                var types = type.GenericTypeArguments;
                var keyType = types[0];
                var valueType = types[1];
                if (typeof(IConvertible).IsAssignableFrom(keyType) &&
                    typeof(IConvertible).IsAssignableFrom(valueType)) {
                    var dicts = new Dictionary<IConvertible, IConvertible>();

                    foreach (DictionaryEntry item in dict) {
                        if (item.Key is IConvertible key && item.Value is IConvertible value) {
                            dicts.Add(key, value);
                        }
                    }

                    SerializeDictionaryBB(Type.GetTypeCode(keyType), Type.GetTypeCode(valueType), dicts);
                    return;
                }

                if (typeof(IConvertible).IsAssignableFrom(keyType) && valueType.IsByRef) {
                    var dicts = new Dictionary<IConvertible, ISerializablePacket>();
                    var valueName = "";
                    foreach (DictionaryEntry item in dict) {
                        if (item.Key is IConvertible key && item.Value is ISerializablePacket serializablePacket) {
                            dicts.Add(key, serializablePacket);
                            valueName = serializablePacket.TypeName;
                        }
                    }
                    SerializeDictionaryBP(Type.GetTypeCode(keyType), valueName, dicts);
                    return;
                }

                if (keyType.IsByRef && valueType.IsByRef) {
                    var keyName = "";
                    var valueName = "";
                    var dicts = new Dictionary<ISerializablePacket, ISerializablePacket>();
                    foreach (DictionaryEntry item in dict) {
                        if (item.Key is ISerializablePacket spKey && item.Value is ISerializablePacket spValue) {
                            dicts.Add(spKey, spValue);
                            keyName = spKey.TypeName;
                            valueName = spValue.TypeName;
                        }
                    }
                    SerializeDictionaryPP(keyName, valueName, dicts);
                    return;
                }

                if (keyType.IsByRef && typeof(IConvertible).IsAssignableFrom(valueType)) {
                    var dicts = new Dictionary<ISerializablePacket, IConvertible>();
                    var keyName = "";
                    foreach (DictionaryEntry item in dict) {
                        if (item.Key is ISerializablePacket key && item.Value is IConvertible value) {
                            dicts.Add(key, value);
                            keyName = key.TypeName;
                        }
                    }
                    SerializeDictionaryPB(keyName, Type.GetTypeCode(valueType), dicts);
                    return;
                }
            }

            throw new NotSupportedException($"{obj.GetType().Name}类型不支持！");
        }

        public void SerializableObject(object obj) {
            SerializableObject(obj?.GetType(), obj);
        }
    }
}