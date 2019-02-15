using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ssc.SscConfiguration;
using Ssc.SscFactory;
using Ssc.SscFile;
using Ssc.SscLog;
using Ssc.SscTemplate;
using Ssc.SscUtil;
using Ssf.SsfComponent;
using Ssf.SsfConfiguration;
using Ssf.SsfManager;
using Ssf.SsfService;
using Ssm.SsmComponent;
using Ssm.SsmManager;
using Sss.SssManager;
using Sss.SssScripts.Lua;

namespace Ssf {
    public abstract class Startup {
        private static readonly Logger Logger = LogManager.GetLogger<Startup>(LogType.Middle);

        private readonly Dictionary<SocketConfig, Server> _services;

        public CacheManager CacheManager { get; }
        public ControllerComponentManager ControllerComponentManager { get; }
        public ModuleManager ModuleManager { get; }
        public RawMessageManager RawMessageManager { get; }

        private ProjectConfig _rojectConfig;
        protected Startup() {
            Ssfi.Initialize();

            CacheManager = new CacheManager();
            ControllerComponentManager = new ControllerComponentManager();
            ModuleManager = new ModuleManager();
            RawMessageManager = new RawMessageManager();

            Configuration(Ssfi.NetworkConfig);
            Configuration(LogManager.LogConfig);
            Configuration(Ssfi.CryptoConfig);

            _services = new Dictionary<SocketConfig, Server>();

            Initialize();

            LuaHelper.RegisterType(GetType());

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
        }

        protected virtual void RegisterModules(ModuleManager moduleManager) {
            var directory = SystemUtil.GetPorjectRootDirectory();

            _rojectConfig = JsonHelper.FromJsonFile<ProjectConfig>(directory + "/Configuration/Modules.json");
            foreach (var item in _rojectConfig.Paths) {

                if (item.Language == Language.CSharp) {
                    moduleManager.AddModuleFromDllFile(item.File, item.Entry);
                }

                if (item.Language == Language.Lua) {
                    moduleManager.AddModuleFromLuaFile(item.File, item.Entry, item.Search);
                }

                if (item.Language == Language.Python) {
                    moduleManager.AddModuleFromPythonFile(item.File, item.Entry, item.Search.Split(';'));
                }
            }
        }

        protected virtual void Initialize() {
            PoolAllocator<ISecurityComponent>.SetPool(
                args => ObjectFactory.GetActivator<ISecurityComponent>(
                    typeof(SecurityComponent).GetConstructors().First())());

            RegisterModules(ModuleManager);
        }

        protected virtual void Configuration(NetworkConfig networkConfig) {
            var directory = SystemUtil.GetPorjectRootDirectory();

            JsonHelper.FromJsonFile(directory + "/Configuration/Services.json", networkConfig);
        }

        protected virtual void Configuration(LogConfig logConfig) {
            var directory = SystemUtil.GetPorjectRootDirectory();

            JsonHelper.FromJsonFile(directory + "/Configuration/Logger.json", logConfig);
        }

        protected virtual void Configuration(CryptoConfig cryptoConfig) {
            var directory = SystemUtil.GetPorjectRootDirectory();

            JsonHelper.FromJsonFile(directory + "/Configuration/Encryptor.json", cryptoConfig);
        }

        private MasterServer SetService(SocketConfig socketConfig) {
            var masterServer = new MasterServer(socketConfig.ServiceID, RawMessageManager, ModuleManager,
                CacheManager, ControllerComponentManager);
            _services.TryAdd(socketConfig, masterServer);
            ServiceManager.AddService(socketConfig.ServiceID, masterServer);
            masterServer.InitializeModules();
            return masterServer;
        }

        public void RunServer(SocketConfig socketConfig) {
            var masterServer = SetService(socketConfig);
            masterServer.StartServer(socketConfig);
        }

        public void RunClient(SocketConfig socketConfig) {
            var masterClient = SetService(socketConfig);
            masterClient.Connect(socketConfig);
        }

        private void GlobalInitFinish() {
            foreach (var module in ModuleManager.ForeachInitializedModule()) {
                module.Value.Finish(ServiceManager.GetService(module.Value.ServiceId),
                    CacheManager, ControllerComponentManager);
            }
        }

        public void Run(string[] args) {

            foreach (var item in Ssfi.NetworkConfig.Services) {
                SetService(item);
            }

            GlobalInitFinish();

            foreach (var item in _services) {
                if (item.Key.ServiceType == ServiceType.Server) {
                    if (item.Key.Enable) {
                        item.Value.StartServer(item.Key);
                    }
                }

                if (item.Key.ServiceType == ServiceType.Client) {
                    if (item.Key.Enable) {
                        item.Value.Connect(item.Key);
                    }
                }

            }

            InteractiveManager.Show(Show);
            InteractiveManager.RunInteractive(this, args);
        }

        private void ProjectShow() {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("ProjectName: " + _rojectConfig.ProjectName);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        private void ServiceShow() {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Services:");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(Ssfi.NetworkConfig.ToString());
            Console.WriteLine();
        }

        private void LogShow() {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Log:");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(LogManager.LogConfig.ToString());
            Console.WriteLine();
        }

        private void ModuleShow() {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Modules:");
            Console.ForegroundColor = ConsoleColor.White;
            foreach (var item in ModuleManager.ForeachInitializedModule()) {
                Console.WriteLine(item.Value.ModuleName);
            }
            Console.WriteLine();
        }

        private void Show() {
            ProjectShow();
            ServiceShow();
            LogShow();
            ModuleShow();
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e) {
            Stop();
        }

        public void Stop() {
            foreach (var server in _services.Values) {
                server.StopServer();
            }
        }
    }
}