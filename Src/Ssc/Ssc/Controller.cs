using Ssc.SscLog;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Ssc.Ssc {
    public class Controller : IController {

        private static readonly Logger Logger = LogManager.GetLogger<Controller>(LogType.Middle);

        public List<string> MethodIds { get; internal set; }

        public Controller() {
            MethodIds = new List<string>();
        }

        public void Register<TInterface>(
            Expression<Action<TInterface>> interfaceExpression, 
            Expression<Action> implementExpression) 
            where TInterface : class {

            var method = Ssci.Register(interfaceExpression, implementExpression);
            MethodIds.Add(method.Id);
        }

        public void Register<TDelegate>(
            string methodId,
            Expression<TDelegate> implementExpression)
            where TDelegate : Delegate{
            var method = Ssci.Register(methodId, implementExpression);
            MethodIds.Add(method.Id);
        }

        public void Register<TInterface, TDelegate>(
            Expression<Action<TInterface>> interfaceExpression, 
            Expression<TDelegate> implementExpression)
            where TDelegate : Delegate {
            var method = Ssci.Register(interfaceExpression, implementExpression);
            MethodIds.Add(method.Id);
        }

        public void Register(
            string methodId, 
            Expression<Action> implementExpression) {
            var method = Ssci.Register(methodId, implementExpression);
            MethodIds.Add(method.Id);
        }

        public void Register<T>(
            T value,
            Action<T, string> callback) where T : struct, Enum {
            var method = Ssci.Register(value, callback);
            MethodIds.Add(method.Id);
        }

        public void Register(
            string methodId,
            Action<string, string> callback) {
            var method = Ssci.Register(methodId, callback);
            MethodIds.Add(method.Id);
        }
    }
}