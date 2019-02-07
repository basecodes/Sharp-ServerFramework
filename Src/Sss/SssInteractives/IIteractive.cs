namespace Sss.SssInteractives {
    public interface IIteractive {
        string Language { get; }

        void Run<T>(T startup, string exit, string[] args);
    }
}