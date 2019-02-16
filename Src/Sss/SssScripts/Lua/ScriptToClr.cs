using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter;
using Ssc.Ssc;
using Ssc.SscFactory;
using Ssc.SscSerialization;
using Ssm.SsmComponent;
using Sss.SssComponent;
using Sss.SssRpc;
using Sss.SssSerialization.Lua;

namespace Sss.SssScripts.Lua {
    internal static class ScriptToClr {        
        
        public static void RegisterTableToPeerComponent(){
            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(
                DataType.Table, typeof(IPeerComponent),
                v => {
                    var table = v.Table;
                    return table.Get(nameof(IPeerComponent)).ToObject<LuaWrapper<LuaPeerComponent>>().Value;
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
                    return table.Get(nameof(IControllerComponent)).ToObject<LuaWrapper<LuaControllerComponent>>().Value;
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

        private static IDictionary MakeDictionary(Type keyType, Type valueType) {
            var dictType = typeof(Dictionary<,>);
            var makeme = dictType.MakeGenericType(keyType, valueType);
            return ObjectFactory.GetActivator<IDictionary>(makeme.GetConstructors().First())();
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
                    var table = v.Table;
                    var values = MakeDictionary(typeof(K), typeof(V));

                    foreach (var item in table.Pairs) {
                        object key = null;
                        if (typeof(K) == typeof(IConvertible)) {
                            key = item.Key.ToObject<IConvertible>();
                        }

                        if (typeof(K) == typeof(ISerializablePacket)) {
                            key = item.Key.Table.Get(nameof(ISerializablePacket)).ToObject<LuaWrapper<ILuaPacket>>().Value as ISerializablePacket;
                        }

                        object value = null;
                        if (typeof(V) == typeof(IConvertible)) {
                            value = item.Value.ToObject<IConvertible>();
                        }

                        if (typeof(V) == typeof(ISerializablePacket)) {
                            value = item.Value.Table.Get(nameof(ISerializablePacket)).ToObject<LuaWrapper<ILuaPacket>>().Value as ISerializablePacket;
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
                    var table = v.Table;

                    if (typeof(T) == typeof(IConvertible)) {
                        var values = new IConvertible[table.Length];
                        for (var i = 0; i < table.Length; i++) {
                            values[i] = table.Get(i + 1).ToObject<IConvertible>();
                        }
                        return values;
                    }

                    if (typeof(T) == typeof(ISerializablePacket)) {
                        var values = new ISerializablePacket[table.Length];
                        for (var i = 0; i < table.Length; i++) {
                            values[i] = table.Get(i + 1).Table.Get(nameof(ISerializablePacket)).ToObject<LuaWrapper<ILuaPacket>>().Value;
                        }
                        return values;
                    }


                    return null;
                }
            );
        }

        public static void RegisterPacket() {
            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(
                DataType.Table, typeof(ISerializablePacket),
                v => {
                    var table = v.Table;
                    return table.Get(nameof(ISerializablePacket))?.ToObject<LuaWrapper<ILuaPacket>>().Value;
                }
            );
        }

        public static void RegisterBaseType(DataType dataType) {
            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(
                dataType, typeof(IConvertible),
                v => {
                    switch (dataType) {
                        case DataType.Boolean:
                            return v.Boolean;

                        case DataType.Number:
                            return v.Number;

                        case DataType.String:
                            return v.String;

                        case DataType.Nil:
                            return null;

                        default:
                            throw new NotSupportedException($"{dataType}");
                    }
                }
            );
        }
        
        public static void RegisterRpcServiceToTable() {
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