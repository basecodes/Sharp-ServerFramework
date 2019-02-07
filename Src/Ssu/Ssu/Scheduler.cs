using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ssu.Ssu {
    public class Scheduler : MonoBehaviour {
        public delegate void DoneHandler(bool isSuccessful);

        private static Scheduler _instance;

        private readonly object _mainThreadLock = new object();

        private List<Action> _mainThreadActions;
        public static long CurrentTick { get; protected set; }

        public static Scheduler Instance {
            get {
                if (_instance == null) {
                    var go = new GameObject("Timer");
                    _instance = go.AddComponent<Scheduler>();
                }

                return _instance;
            }
        }

        public event Action<long> OnTick;

        public event Action ApplicationQuit;

        private void Awake() {
            Application.runInBackground = true;

            _mainThreadActions = new List<Action>();
            _instance = this;
            DontDestroyOnLoad(this);

            StartCoroutine(StartTicker());
        }

        private void Update() {
            if (_mainThreadActions.Count > 0)
                lock (_mainThreadLock) {
                    foreach (var actions in _mainThreadActions) actions.Invoke();

                    _mainThreadActions.Clear();
                }
        }

        public static void WaitUntil(Func<bool> condiction, DoneHandler doneCallback, float timeoutSeconds) {
            _instance.StartCoroutine(WaitWhileTrueCoroutine(condiction, doneCallback, timeoutSeconds, true));
        }

        public static void WaitWhile(Func<bool> condiction, DoneHandler doneCallback, float timeoutSeconds) {
            _instance.StartCoroutine(WaitWhileTrueCoroutine(condiction, doneCallback, timeoutSeconds));
        }

        private static IEnumerator WaitWhileTrueCoroutine(Func<bool> condition, DoneHandler callback,
            float timeoutSeconds, bool reverseCondition = false) {
            while (timeoutSeconds > 0 && condition.Invoke() == !reverseCondition) {
                timeoutSeconds -= Time.deltaTime;
                yield return null;
            }

            callback.Invoke(timeoutSeconds > 0);
        }

        public static void AfterSeconds(float time, Action callback) {
            _instance.StartCoroutine(_instance.StartWaitingSeconds(time, callback));
        }

        public static void ExecuteOnMainThread(Action action) {
            _instance.OnMainThread(action);
        }

        public void OnMainThread(Action action) {
            lock (_mainThreadLock) {
                _mainThreadActions.Add(action);
            }
        }

        private IEnumerator StartWaitingSeconds(float time, Action callback) {
            yield return new WaitForSeconds(time);
            callback.Invoke();
        }

        private IEnumerator StartTicker() {
            CurrentTick = 0;
            while (true) {
                yield return new WaitForSeconds(1);
                CurrentTick++;
                try {
                    OnTick?.Invoke(CurrentTick);
                } catch (Exception e) {
                    Ssc.SscLog.Logs.Error(e);
                }
            }
        }

        private void OnDestroy() {
        }

        private void OnApplicationQuit() {
            ApplicationQuit?.Invoke();
        }
    }
}