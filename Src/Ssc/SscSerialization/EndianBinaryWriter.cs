using System;
using System.Collections.Generic;
using Ssc.SscLog;
using Ssc.SscStream;

namespace Ssc.SscSerialization {
    public struct EndianBinaryWriter : IEndianBinaryWriter {
        private IWriteStream _writeStream;
        private static readonly Logger Logger = LogManager.GetLogger<EndianBinaryWriter>(LogType.Middle);

        public EndianBinaryWriter(IWriteStream writeStream) {
            _writeStream = writeStream;
        }

        #region Write

        public void Write<T>(T value) where T : IConvertible {
            Write(Type.GetTypeCode(typeof(T)), value);
        }

        public void Write(TypeCode typeCode, IConvertible value) {
            if (value != null) {
                _writeStream.ShiftRight(true);
                _writeStream.ShiftRight((byte) typeCode);
                _writeStream.ShiftRight(typeCode, value);
            } else {
                _writeStream.ShiftRight(false);
            }
        }

        #endregion Write

        #region WritePacket

        public void WritePacket<T>(T packet) where T : class,ISerializablePacket {
            if (packet is ISerializablePacket serializablePacket)
                WritePacket(typeof(T).Name, serializablePacket);
            else
                _writeStream.ShiftRight(false);
        }

        public void WritePacket(string interfaceName, ISerializablePacket packet) {
            if (packet != null) {
                _writeStream.ShiftRight(true);
                _writeStream.ShiftRight(interfaceName);
                var endianBinaryWriter = new EndianBinaryWriter(_writeStream);
                packet.ToBinaryWriter(endianBinaryWriter);
            } else {
                _writeStream.ShiftRight(false);
            }
        }

        #endregion WritePacket

        #region WriteArray

        public void WriteArray<T>(T[] array) where T : IConvertible {
            if (array != null && array.Length != 0) {
                _writeStream.ShiftRight(true);
                _writeStream.ShiftRight((ushort) array.Length);
                _writeStream.ShiftRight((byte) Type.GetTypeCode(typeof(T)));
                for (var i = 0; i < array.Length; i++) Write(array[i]);
            } else {
                _writeStream.ShiftRight(false);
            }
        }

        public void WriteArray(TypeCode typeCode, IConvertible[] array) {
            if (array != null && array.Length != 0) {
                _writeStream.ShiftRight(true);
                _writeStream.ShiftRight((ushort) array.Length);
                _writeStream.ShiftRight((byte) typeCode);
                foreach (var value in array) Write(typeCode, value);
            } else {
                _writeStream.ShiftRight(false);
            }
        }

        #endregion WriteArray

        #region WritePacketArray

        public void WritePacketArray<T>(T[] packets) where T : class,ISerializablePacket {
            if (packets != null && packets.Length != 0) {
                _writeStream.ShiftRight(true);
                _writeStream.ShiftRight((ushort) packets.Length);
                _writeStream.ShiftRight(typeof(T).Name);

                for (var i = 0; i < packets.Length; i++) WritePacket(packets[i]);
            } else {
                _writeStream.ShiftRight(false);
            }
        }

        public void WritePacketArray(string interfaceName, ISerializablePacket[] packets) {
            if (packets != null && packets.Length != 0) {
                _writeStream.ShiftRight(true);
                _writeStream.ShiftRight((ushort) packets.Length);
                _writeStream.ShiftRight(interfaceName);

                for (var i = 0; i < packets.Length; i++) WritePacket(interfaceName, packets[i]);
            } else {
                _writeStream.ShiftRight(false);
            }
        }

        #endregion WritePacketArray

        #region WriteDictionaryBB

        public void WriteDictionaryBB<K, V>(Dictionary<K, V> dictionary)
            where K : IConvertible
            where V : IConvertible {
            if (dictionary != null && dictionary.Count != 0) {
                _writeStream.ShiftRight(true);
                _writeStream.ShiftRight((ushort) dictionary.Count);

                _writeStream.ShiftRight((byte) Type.GetTypeCode(typeof(K)));
                _writeStream.ShiftRight((byte) Type.GetTypeCode(typeof(V)));

                foreach (var item in dictionary) {
                    Write(item.Key);
                    Write(item.Value);
                }
            } else {
                _writeStream.ShiftRight(false);
            }
        }

        public void WriteDictionaryBB(TypeCode keyTypeCode, TypeCode valueTypeCode,
            Dictionary<IConvertible, IConvertible> dictionary) {
            if (dictionary != null && dictionary.Count != 0) {

                _writeStream.ShiftRight(true);
                _writeStream.ShiftRight((ushort) dictionary.Count);

                _writeStream.ShiftRight((byte) keyTypeCode);
                _writeStream.ShiftRight((byte) valueTypeCode);

                foreach (var item in dictionary) {
                    Write(keyTypeCode, item.Key);
                    Write(valueTypeCode, item.Value);
                }
            } else {
                _writeStream.ShiftRight(false);
            }
        }

        #endregion WriteDictionaryBB

        #region WriteDictionaryBP
        
        public void WriteDictionaryBP<K, V>(Dictionary<K, V> dictionary)
            where K : IConvertible
            where V : class,ISerializablePacket {
            if (dictionary != null && dictionary.Count != 0) {
                _writeStream.ShiftRight(true);
                _writeStream.ShiftRight((ushort) dictionary.Count);

                _writeStream.ShiftRight((byte)Type.GetTypeCode(typeof(K)));
                _writeStream.ShiftRight(typeof(V).Name);

                foreach (var item in dictionary) {
                    Write(item.Key);
                    WritePacket(item.Value);
                }
            } else {
                _writeStream.ShiftRight(false);
            }
        }

        public void WriteDictionaryBP(TypeCode keyTypeCode, string interfaceName,
            Dictionary<IConvertible, ISerializablePacket> dictionary) {
            if (dictionary != null && dictionary.Count != 0) {

                _writeStream.ShiftRight(true);
                _writeStream.ShiftRight((ushort) dictionary.Count);

                _writeStream.ShiftRight((byte) keyTypeCode);
                _writeStream.ShiftRight(interfaceName);

                foreach (var item in dictionary) {
                    Write(keyTypeCode, item.Key);
                    WritePacket(interfaceName, item.Value);
                }
            } else {
                _writeStream.ShiftRight(false);
            }
        }

        #endregion WriteDictionaryBP

        #region WriteDictionaryPP

        public void WriteDictionaryPP<K, V>(Dictionary<K, V> dictionary)
            where K : class,ISerializablePacket
            where V : class,ISerializablePacket {
            if (dictionary != null && dictionary.Count != 0) {
                _writeStream.ShiftRight(true);
                _writeStream.ShiftRight((ushort) dictionary.Count);

                _writeStream.ShiftRight(typeof(K).Name);
                _writeStream.ShiftRight(typeof(V).Name);

                foreach (var item in dictionary) {
                    WritePacket(item.Key);
                    WritePacket(item.Value);
                }
            } else {
                _writeStream.ShiftRight(false);
            }
        }

        public void WriteDictionaryPP(string keyInterfaceName, string valueInterfaceName,
            Dictionary<ISerializablePacket, ISerializablePacket> dictionary) {
            if (dictionary != null && dictionary.Count != 0) {

                _writeStream.ShiftRight(true);
                _writeStream.ShiftRight((ushort) dictionary.Count);

                _writeStream.ShiftRight(keyInterfaceName);
                _writeStream.ShiftRight(valueInterfaceName);

                foreach (var item in dictionary) {
                    WritePacket(keyInterfaceName, item.Key);
                    WritePacket(valueInterfaceName, item.Value);
                }
            } else {
                _writeStream.ShiftRight(false);
            }
        }

        #endregion WriteDictionaryPP

        #region WriteDictionaryPB

        public void WriteDictionaryPB<K, V>(Dictionary<K, V> dictionary)
            where K : class,ISerializablePacket
            where V : IConvertible {
            if (dictionary != null && dictionary.Count != 0) {
                _writeStream.ShiftRight(true);
                _writeStream.ShiftRight((ushort) dictionary.Count);

                _writeStream.ShiftRight(typeof(K).Name);
                _writeStream.ShiftRight((byte) Type.GetTypeCode(typeof(V)));

                foreach (var item in dictionary) {
                    WritePacket(item.Key);
                    Write(item.Value);
                }
            } else {
                _writeStream.ShiftRight(false);
            }
        }

        public void WriteDictionaryPB(string interfaceName, TypeCode valueTypeCode,
            Dictionary<ISerializablePacket, IConvertible> dictionary) {
            if (dictionary != null && dictionary.Count != 0) {

                _writeStream.ShiftRight(true);
                _writeStream.ShiftRight((ushort) dictionary.Count);

                _writeStream.ShiftRight(interfaceName);
                _writeStream.ShiftRight((byte) valueTypeCode);

                foreach (var item in dictionary) {
                    WritePacket(interfaceName, item.Key);
                    Write(valueTypeCode, item.Value);
                }
            } else {
                _writeStream.ShiftRight(false);
            }
        }

        #endregion WriteDictionaryPB

        public ByteFragment ToBytes() {
            var byteFragment = _writeStream.ToByteFragment();
            return byteFragment;
        }
    }
}