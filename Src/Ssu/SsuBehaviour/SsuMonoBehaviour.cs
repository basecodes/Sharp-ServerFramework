using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Ssc;
using Ssc.SscLog;
using Ssc.SscPatterns;
using Ssc.SscRpc;
using Ssu.SsuManager;
using UnityEngine;

namespace Ssu.SsuBehaviour {
    public class SsuMonoBehaviour : MonoBehaviour {

        private static readonly Ssc.SscLog.Logger Logger = LogManager.GetLogger<SsuMonoBehaviour>(Ssc.SscLog.LogType.Middle);
        private readonly List<string> _events = new List<string>();
        private readonly List<string> _delegates = new List<string>();

        /// <summary>
        /// 所有事件监听
        /// </summary>
        private static readonly EventManager<string, Component> _eventManager = new EventManager<string, Component>();

        /// <summary>
        /// 共享数据
        /// </summary>
        private static readonly Dictionary<string, object> _fields = new Dictionary<string, object>();

        /// <summary>
        /// 添加共享字段
        /// </summary>
        protected void AddSharedObject<T>(T value) {
            var key = typeof(T);
            AddSharedObject(key.Name, value);
        }

        protected void AddSharedObject<T>(string name, T value) {
            if (string.IsNullOrEmpty(name)) {
                throw new ArgumentNullException(nameof(name));
            }

            if (value == null) {
                throw new ArgumentNullException(nameof(value));
            }

            if (value is MonoBehaviour) {
                throw new NotSupportedException($"不支持{nameof(MonoBehaviour)}类型！");
            }

            if (_fields.ContainsKey(name)) {
                Logger.Warn($"当前Key:{name}字段已存在！");
                return;
            }

            _fields.Add(name, value);
        }

        /// <summary>
        /// 获取本场景共享字段
        /// </summary>
        protected T GetSharedObject<T>() {
            var key = typeof(T);
            return GetSharedObject<T>(key.Name); ;
        }

        protected T GetSharedObject<T>(string name) {
            if (string.IsNullOrEmpty(name)) {
                throw new ArgumentNullException(nameof(name));
            }

            if (!_fields.ContainsKey(name)) {
                Logger.Warn($"当前Key:{name}字段不存在！");
                return default;
            }

            var obj = _fields[name];
            if (obj is T value) {
                return value;
            }
            Logger.Warn($"字段值类型不是指定的{typeof(T).Name}类型而是{obj.GetType().Name}");
            return default;
        }

        /// <summary>
        /// 添加事件监听
        /// </summary>
        protected void AddListener(
            string eventType, 
            EventManager<string, Component>.Handle listener) {
            if (string.IsNullOrEmpty(eventType)) {
                throw new ArgumentNullException(nameof(eventType));
            }
            _events.Add(eventType);
            _eventManager.AddListener(eventType, listener);
        }

        /// <summary>
        /// 投递事件
        /// </summary>
        protected void PostNotification(
            string eventType, 
            Component sender, 
            params object[] args) {
            
            if (string.IsNullOrEmpty(eventType)) {
                throw new ArgumentNullException(nameof(eventType));
            }
            if (!_eventManager.PostNotification(eventType, sender, args)) {
                Logger.Warn($"事件：{eventType}投递失败！事件已移除");
            }

        }

        /// <summary>
        /// 添加为注册函数接口
        /// </summary>
        public void AddNotHandler(NotHandler notHandler) {
            if (notHandler == null) {
                throw new ArgumentNullException(nameof(notHandler));
            }

            RpcManager.AddNotHandlerMethod(notHandler);
        }

        /// <summary>
        /// 检查对象是否为空
        /// </summary>
        protected bool CheckObjectEmpty(object obj) {
            if (obj == null) {
                Logger.Warn($"{obj.GetType().Name}为空！");
                return true;
            }
            return false;
        }

        /// <summary>
        /// 销毁
        /// </summary>
        protected virtual void OnDestory() {
            foreach (var dge in _delegates) {
                RpcRegister.RemoveMethod(dge);
            }

            foreach (var evt in _events) {
                _eventManager.RemoveEvent(evt);
            }
        }


        protected void Register<TDelegate>(
            string methodId,
            Expression<TDelegate> implementExpression,
            bool clearUp = true) where TDelegate : Delegate {

            var method = Ssci.Register(methodId, implementExpression);
            if (clearUp) {
                _delegates.Add(method.Id);
            }
        }

        protected void Register<TInterface, TDelegate>(
            Expression<Action<TInterface>> interfaceExpression,
            Expression<TDelegate> implementExpression,
            bool clearUp = true)
            where TDelegate : Delegate {

            var method = Ssci.Register(interfaceExpression, implementExpression);
            if (clearUp) {
                _delegates.Add(method.Id);
            }
        }

        protected void Register<TInterface>(
            Expression<Action<TInterface>> interfaceExpression,
            Expression<Action> implementExpression,
            bool clearUp = true)
            where TInterface : class {

            var method = Ssci.Register(interfaceExpression, implementExpression);
            if (clearUp) {
                _delegates.Add(method.Id);
            }
        }

        protected void Register(
            string methodId,
            Expression<Action> implementExpression,
            bool clearUp = true) {

            var method = Ssci.Register(methodId, implementExpression);
            if (clearUp) {
                _delegates.Add(method.Id);
            }
        }

        protected void Register<T>(
            T value,
            Action<T, string> callback,
            bool clearUp = true) where T : struct, Enum {

            var method = Ssci.Register(value, callback);
            if (clearUp) {
                _delegates.Add(method.Id);
            }
        }

        protected void Register(
            string methodId,
            Action<string, string> callback,
            bool clearUp = true) {

            var method = Ssci.Register(methodId, callback);
            if (clearUp) {
                _delegates.Add(method.Id);
            }
        }
    }
}