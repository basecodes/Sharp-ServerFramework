using System;
using System.Collections;
using System.Collections.Generic;
using Ssc.SscExtension;
using Ssc.SscLog;
using Ssc.SscRpc;
using Ssc.SscStream;

namespace Ssc.SscSerialization {
    public struct EndianBinaryReader : IEndianBinaryReader {
        private static readonly Logger Logger = LogManager.GetLogger<EndianBinaryReader>(LogType.Middle);

        private readonly IReadStream _readStream;
        
        public EndianBinaryReader(IReadStream readStream) {
            _readStream = readStream;
        }

        #region Read

        public T Read<T>() where T : IConvertible {
            var value = Read();
            return value == null ? default : (T) value;
        }

        public IConvertible Read() {
            if (_readStream.ShiftRight<bool>()) {
                var typeCode = (TypeCode) _readStream.ShiftRight<byte>();
                var value = _readStream.ShiftRight(typeCode);
                return value;
            }

            return null;
        }

        #endregion Read

        #region ReadPacket

        public T ReadPacket<T>() where T : class,ISerializablePacket {
            return ReadPacket() as T;
        }

        public ISerializablePacket ReadPacket() {
            if (_readStream.ShiftRight<bool>()) {
                var interfaceName = _readStream.ShiftRight<string>();
                var packet = PacketManager.CreatePacket(interfaceName);
                var endianBinaryReader = new EndianBinaryReader(_readStream);
                packet.FromBinaryReader(endianBinaryReader);
                return packet;
            }

            return null;
        }

        #endregion ReadPacket

        #region ReadArray

        public T[] ReadArray<T>() where T : IConvertible {
            var packets = ReadArray();
            if (packets is T[] value) {
                return value;
            }
            return null;
        }

        public Array ReadArray() {
            if (_readStream.ShiftRight<bool>()) {
                var length = _readStream.ShiftRight<ushort>();
                var typeCode = (TypeCode) _readStream.ShiftRight<byte>();
                var array = typeCode.GetBaseType().MakeArray(length);
                for (var i = 0; i < length; ++i) {
                    array.SetValue(Read(), i);
                }
                return array;
            }

            return null;
        }

        #endregion ReadArray

        #region ReadPacketArray

        public T[] ReadPacketArray<T>() where T : class,ISerializablePacket {
            var packets = ReadPacketArray();
            if (packets is T[] value) {
                return value;
            }
            return null;
        }

        public Array ReadPacketArray() {
            if (_readStream.ShiftRight<bool>()) {
                var length = _readStream.ShiftRight<ushort>();
                var interfaceName = _readStream.ShiftRight<string>();
                var packets = PacketManager.GetPacketType(interfaceName).MakeArray(length);
                for (var i = 0; i < length; ++i) {
                    packets.SetValue(ReadPacket(), i);
                }
                return packets;
            }

            return null;
        }

        #endregion ReadPacketArray

        #region ReadDictionaryBB

        public Dictionary<K, V> ReadDictionaryBB<K, V>()
            where K : IConvertible
            where V : IConvertible {
            return ReadDictionaryBB() as Dictionary<K, V>;
        }

        public IDictionary ReadDictionaryBB() {
            if (_readStream.ShiftRight<bool>()) {
                var length = _readStream.ShiftRight<ushort>();

                var keyTypeCode = (TypeCode) _readStream.ShiftRight<byte>();
                var valueTypeCode = (TypeCode) _readStream.ShiftRight<byte>();

                var dicts = DictionaryExtension.MakeDictionary(
                    keyTypeCode.GetBaseType(),
                    valueTypeCode.GetBaseType());

                for (var i = 0; i < length; i++) {
                    dicts.Add(Read(), Read());
                }

                return dicts;
            }

            return null;
        }

        #endregion ReadDictionaryBB

        #region ReadDictionaryBP

        public Dictionary<K, V> ReadDictionaryBP<K, V>()
            where K : IConvertible
            where V : class,ISerializablePacket {
            return ReadDictionaryBP() as Dictionary<K, V>;
        }

        public IDictionary ReadDictionaryBP() {
            if (_readStream.ShiftRight<bool>()) {
                var length = _readStream.ShiftRight<ushort>();

                var keyTypeCode = (TypeCode) _readStream.ShiftRight<byte>();
                var valueInterfaceName = _readStream.ShiftRight<string>();

                var dicts = DictionaryExtension.MakeDictionary(
                    keyTypeCode.GetBaseType(),
                    PacketManager.GetPacketType(valueInterfaceName));

                for (var i = 0; i < length; i++) {
                    dicts.Add(Read(), ReadPacket());
                }
                return dicts;
            }

            return null;
        }

        #endregion ReadDictionaryBP

        #region ReadDictionaryPP

        public Dictionary<K, V> ReadDictionaryPP<K, V>()
            where K : class,ISerializablePacket
            where V : class,ISerializablePacket{
            return ReadDictionaryPP() as Dictionary<K, V>;
        }

        public IDictionary ReadDictionaryPP() {
            if (_readStream.ShiftRight<bool>()) {
                var length = _readStream.ShiftRight<ushort>();
                var keyInterfaceName = _readStream.ShiftRight<string>();
                var valueInterfaceName = _readStream.ShiftRight<string>();

                var dicts = DictionaryExtension.MakeDictionary(
                    PacketManager.GetPacketType(keyInterfaceName),
                    PacketManager.GetPacketType(valueInterfaceName));

                for (var i = 0; i < length; i++) {
                    dicts.Add(ReadPacket(), ReadPacket());
                }
                return dicts;
            }

            return null;
        }

        #endregion ReadDictionaryPP

        #region ReadDictionaryPB

        public Dictionary<K, V> ReadDictionaryPB<K, V>()
            where K : class,ISerializablePacket
            where V : IConvertible {
            return ReadDictionaryPB() as Dictionary<K, V>;
        }

        public IDictionary ReadDictionaryPB() {
            if (_readStream.ShiftRight<bool>()) {
                var length = _readStream.ShiftRight<ushort>();
                var keyInterfaceName = _readStream.ShiftRight<string>();
                var valueTypeCode = (TypeCode) _readStream.ShiftRight<byte>();

                var dicts = DictionaryExtension.MakeDictionary(
                    PacketManager.GetPacketType(keyInterfaceName),
                    valueTypeCode.GetBaseType());

                for (var i = 0; i < length; i++) {
                    dicts.Add(ReadPacket(), Read());
                }

                return dicts;
            }

            return null;
        }

        #endregion ReadDictionaryPB
    }
}