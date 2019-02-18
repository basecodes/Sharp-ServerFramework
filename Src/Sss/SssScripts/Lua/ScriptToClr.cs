using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter;
using Ssc.Ssc;
using Ssc.SscExtension;
using Ssc.SscFactory;
using Ssc.SscSerialization;
using Ssm.SsmComponent;
using Sss.SssComponent;
using Sss.SssSerialization.Lua;

namespace Sss.SssScripts.Lua {
    internal static class ScriptToClr {        
        
        public static void RegisterTableToPeerComponent(){
            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(
                DataType.Table, typeof(IPeerComponent),
                v => {
                    var table = v.Table;
                    return table.Get(nameof(IPeerComponent)).ToObject<ClassWrapper<LuaPeerComponent>>().Value;
                }
            );
        }

        public static void RegisterTableToObject() {
            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(
                DataType.Table, typeof(object),
                v => {
                    var table = v.Table;
                    var type = table.Get("Type")?.ToObject<FieldType>();
                    switch (type) {
                        case FieldType.NullType:
                            return null;
                        case FieldType.BaseType: {
                                var conversion = Script.GlobalOptions.CustomConverters.GetScriptToClrCustomConversion(
                                    DataType.Table, typeof(IConvertible));
                                return conversion?.Invoke(v);
                            }
                        case FieldType.PacketType: {
                                var conversion = Script.GlobalOptions.CustomConverters.GetScriptToClrCustomConversion(
                                    DataType.Table, typeof(ISerializablePacket));
                                return conversion?.Invoke(v);
                            }
                        case FieldType.ArrayBase: {
                                var conversion = Script.GlobalOptions.CustomConverters.GetScriptToClrCustomConversion(
                                    DataType.Table, typeof(IConvertible[]));
                                return conversion?.Invoke(v);
                            }
                        case FieldType.ArrayPacket: {
                                var conversion = Script.GlobalOptions.CustomConverters.GetScriptToClrCustomConversion(
                                    DataType.Table, typeof(ISerializablePacket[]));
                                return conversion?.Invoke(v);
                            }
                        case FieldType.DictKBVB: {
                                var conversion = Script.GlobalOptions.CustomConverters.GetScriptToClrCustomConversion(
                                    DataType.Table, typeof(Dictionary<IConvertible, IConvertible>));
                                return conversion?.Invoke(v);
                            }
                        case FieldType.DictKBVP: {
                                var conversion = Script.GlobalOptions.CustomConverters.GetScriptToClrCustomConversion(
                                    DataType.Table, typeof(Dictionary<IConvertible, ISerializablePacket>));
                                return conversion?.Invoke(v);
                            }
                        case FieldType.DictKPVP: {
                                var conversion = Script.GlobalOptions.CustomConverters.GetScriptToClrCustomConversion(
                                    DataType.Table, typeof(Dictionary<ISerializablePacket, ISerializablePacket>));
                                return conversion?.Invoke(v);
                            }
                        case FieldType.DictKPVB: {
                                var conversion = Script.GlobalOptions.CustomConverters.GetScriptToClrCustomConversion(
                                    DataType.Table, typeof(Dictionary<ISerializablePacket, IConvertible>));
                                return conversion?.Invoke(v);
                            }
                    }

                    return null;
                }
            );
        }

        public static void RegisterTableToRpcComponent(){
            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(
                DataType.Table, typeof(IControllerComponent),
                v => {
                    var table = v.Table;
                    return table.Get(nameof(IControllerComponent)).ToObject<ClassWrapper<LuaControllerComponent>>().Value;
                }
            );
        }

        public static void RegisterFunctionToResponseCallback() {
            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(
                DataType.Function, typeof(ResponseCallback),
                v => {
                    var func = v.Function;
                    void Callback(IResponseMessage responseMessage, IDeserializable singleDeserializable) {
                        func?.Call(responseMessage, singleDeserializable);
                    }

                    return (ResponseCallback) Callback;
                }
            );
        }

        public static void RegisterFunctionToAction() {
            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(
                DataType.Function, typeof(Action),
                v => {
                    var func = v.Function;
                    void Action() {
                        func?.Call();
                    }
                    return (Action)Action;
                }
            );
        }

        public static void RegisterTableToDictionary<K, V>() {
            if (typeof(K) != typeof(IConvertible) && typeof(K) != typeof(ISerializablePacket)){
                throw new NotSupportedException($"{typeof(K).Name}类型必须是({nameof(IConvertible)}或{nameof(ISerializablePacket)})");
            }

            if (typeof(V) != typeof(IConvertible) && typeof(V) != typeof(ISerializablePacket)) {
                throw new NotSupportedException($"{typeof(V).Name}类型必须是({nameof(IConvertible)}或{nameof(ISerializablePacket)})");
            }

            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(
                DataType.Table, typeof(Dictionary<K, V>),
                v => {
                    var table = v.Table.Get("Value").Table;
                    var keyType = v.Table.Get("KeyType").ToObject<Type>();
                    var valueType = v.Table.Get("ValueType").ToObject<Type>();

                    var values = DictionaryExtension.MakeDictionary(keyType, valueType);

                    foreach (var item in table.Pairs) {
                        object key = null;
                        if (typeof(IConvertible).IsAssignableFrom(keyType)) {
                            key = item.Key.ToObject(keyType);
                        }

                        if (typeof(ISerializablePacket).IsAssignableFrom(keyType)) {
                            key = item.Key.Table.Get(nameof(ISerializablePacket)).ToObject<ClassWrapper<ILuaPacket>>().Value as ISerializablePacket;
                        }

                        object value = null;
                        if (typeof(IConvertible).IsAssignableFrom(valueType)) {
                            value = item.Value.ToObject(valueType);
                        }

                        if (typeof(ISerializablePacket).IsAssignableFrom(valueType)) {
                            value = item.Value.Table.Get(nameof(ISerializablePacket)).ToObject<ClassWrapper<ILuaPacket>>().Value as ISerializablePacket;
                        }

                        if (key == null || value == null) {
                            return null;
                        }

                        values.Add(key, value);
                    }

                    return values;
                }
            );
        }

        public static void RegisterTableToArray<T>() {
            if (typeof(T) != typeof(IConvertible) && typeof(T) != typeof(ISerializablePacket)) {
                throw new NotSupportedException($"{typeof(T).Name}类型必须是({nameof(IConvertible)}或{nameof(ISerializablePacket)})");
            }

            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(
                DataType.Table, typeof(T[]),
                v => {
                    var table = v.Table.Get("Value").Table;
                    var subType = v.Table.Get("ElementTypeCode");
                    var typeCode = subType.ToObject<TypeCode>();
                    if (typeCode != TypeCode.Object) {
                        var values = typeCode.GetBaseType().MakeArray(table.Length);
                        for (var i = 0; i < table.Length; i++) {
                            values.SetValue(GetValue(table.Get(i + 1), typeCode),i);
                        }
                        return values;
                    } else {
                        var values = typeof(ISerializablePacket).MakeArray(table.Length);
                        for (var i = 0; i < table.Length; i++) {
                            var luaPacket = table.Get(i + 1).Table.Get(nameof(ISerializablePacket)).ToObject<ClassWrapper<ILuaPacket>>().Value;
                            values.SetValue(luaPacket, i);
                        }
                        return values;
                    }
                }
            );
        }

        public static void RegisterPacket() {
            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(
                DataType.Table, typeof(ISerializablePacket),
                v => {
                    var table = v.Table.Get("Value").Table;
                    return table.Get(nameof(ISerializablePacket))?.ToObject<ClassWrapper<ILuaPacket>>().Value;
                }
            );
        }

        private static object GetValue(DynValue dynValue,TypeCode? typeCode) {
            switch (typeCode) {
                case TypeCode.Boolean:
                    return dynValue.ToObject<bool>();
                case TypeCode.Byte:
                    return dynValue.ToObject<byte>();
                case TypeCode.Char:
                    return dynValue.ToObject<char>();
                case TypeCode.Double:
                    return dynValue.ToObject<double>();
                case TypeCode.Int16:
                    return dynValue.ToObject<short>();
                case TypeCode.Int32:
                    return dynValue.ToObject<int>();
                case TypeCode.Int64:
                    return dynValue.ToObject<long>();
                case TypeCode.SByte:
                    return dynValue.ToObject<sbyte>();
                case TypeCode.Single:
                    return dynValue.ToObject<float>();
                case TypeCode.String:
                    return dynValue.ToObject<string>();
                case TypeCode.UInt16:
                    return dynValue.ToObject<ushort>();
                case TypeCode.UInt32:
                    return dynValue.ToObject<uint>();
                case TypeCode.UInt64:
                    return dynValue.ToObject<ulong>();
            }
            return null;
        }
        public static void RegisterTableToBaseType() {
            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(
                DataType.Table, typeof(IConvertible),
                v => {
                    var table = v.Table;
                    var typeCode = table.Get("TypeCode")?.ToObject<TypeCode>();
                    return GetValue(table.Get("Value"),typeCode);
                }
            );
        }
        
        public static void RegisterControllerToTable() {
            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(
                DataType.UserData, typeof(Table),
                v => {
                    var userData = v.UserData;
                    if (userData.Object is LuaController luaRpcService) {
                        return luaRpcService.Instance;
                    }
                    return null;
                }
            );
        }

        public static void RegisterUserData<T>() {
            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(
                DataType.UserData, typeof(T),
                v => {
                    var userData = v.UserData;
                    return (T) userData.Object;
                }
            );
        }
    }
}