namespace Sss.SssScripts.Lua {
    public class LuaWrapper<T> {
        public T Value { get;}

        public LuaWrapper(T value) {
            Value = value;
        }
    }
}
