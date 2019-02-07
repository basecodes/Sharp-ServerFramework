using System;
using Ssc.SscLog;
using Ssc.SscStream;

namespace Ssc.SscSerialization {
    public struct Deserializable : IDeserializable {
        private static readonly Logger Logger = LogManager.GetLogger<Deserializable>(LogType.Middle);

        private readonly IReadStream _readStream;
        private readonly EndianBinaryReader _endianBinaryReader;

        public Deserializable(IReadStream readStream) {
            _readStream = readStream;
            _endianBinaryReader = new EndianBinaryReader(readStream);
        }

        public object[] Deserialize(object[] objects, int length) {
            for (var i = 0; i < length; ++i) {
                var type = (FieldType) _readStream.ShiftRight<byte>();
                switch (type) {
                    case FieldType.ArrayBase: {
                        objects[i] = _endianBinaryReader.ReadArray();
                    }
                        break;

                    case FieldType.ArrayPacket: {
                        objects[i] = _endianBinaryReader.ReadPacketArray();
                    }
                        break;

                    case FieldType.DictKBVB: {
                        objects[i] = _endianBinaryReader.ReadDictionaryBB();
                    }
                        break;

                    case FieldType.PacketType: {
                        objects[i] = _endianBinaryReader.ReadPacket();
                    }
                        break;

                    case FieldType.DictKBVP: {
                        objects[i] = _endianBinaryReader.ReadDictionaryBP();
                    }
                        break;

                    case FieldType.DictKPVP: {
                        objects[i] = _endianBinaryReader.ReadDictionaryPP();
                    }
                        break;

                    case FieldType.DictKPVB: {
                        objects[i] = _endianBinaryReader.ReadDictionaryPB();
                    }
                        break;

                    case FieldType.BaseType: {
                        objects[i] = _endianBinaryReader.Read();
                    }
                        break;

                    case FieldType.NullType: {
                        objects[i] = null;
                    }
                        break;

                    default: {
                        Logger.Warn($"{type}类型不支持！");
                        throw new NotSupportedException(type.ToString());
                    }
                }
            }

            return objects;
        }
    }
}