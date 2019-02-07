using MoonSharp.Interpreter;
using Ssm.SsmComponent;

namespace Sss.SssComponent {
    public interface ILuaComponent:IPeerComponent {
        Table Instance { get; }
        T GetField<T>(string name);
        T Call<T>(string methodName, params object[] args);
    }
}