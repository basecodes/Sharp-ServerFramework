using Ssm.SsmComponent;

namespace Ssm.SsmManager {
    public interface IControllerComponentManager {
        
        IControllerComponent AddControllerComponent(string interfaceName, IControllerComponent controllerComponent);
        Implement AddControllerComponent<Implement>(string interfaceName, Implement implement) 
            where Implement : class, IControllerComponent;
        Implement AddControllerComponent<Interface, Implement>(Implement implement) 
            where Interface : IControllerComponent
            where Implement : class, Interface;
        IControllerComponent GetControllerComponent(string interfaceName);
        Implement GetControllerComponent<Implement>(string interfaceName) 
            where Implement : class, IControllerComponent;
        Interface GetControllerComponent<Interface>() where Interface : class, IControllerComponent;
        void RemoveControllerComponent(string interfaceName);
        void RemoveControllerComponent<Interface>() where Interface : class, IControllerComponent;
        
    }
}