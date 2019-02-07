
namespace Ssc.Ssc {
    public interface IResponseMessage : IRpc {
        string Exception { get; }
    }
}