using Ssc.Ssc;

namespace Ssm.SsmComponent {
    public interface IComponentable:IPeer {
        void AddComponent<T>(T component) where T : class, IPeerComponent;

        void AddComponent(string componentName, IPeerComponent component);

        void RemoveComponent<T>() where T : class, IPeerComponent;

        void RemoveComponent(string componentName);

        T GetComponent<T>() where T : class, IPeerComponent;
        T GetComponent<T>(string componentName) where T : class, IPeerComponent;

        IPeerComponent GetComponent(string componentName);

        bool HasComponent<T>() where T : class, IPeerComponent;

        bool HasComponent(string componentName);
    }
}
