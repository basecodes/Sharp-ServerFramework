using CSharpModule.Modules;
using Ssm.SsmModule;

namespace CSharpModule {
    public class Startup : Entry {
        public override IModule Main() {
            return new TestModule();
        }
    }
}