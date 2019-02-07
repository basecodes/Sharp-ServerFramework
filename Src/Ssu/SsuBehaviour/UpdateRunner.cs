using System;

namespace Ssu.SsuBehaviour {
    public class UpdateRunner {
        public event Action ApplicationQuit;
        private static readonly Ssc.SscLog.Logger Logger = Ssc.SscLog.LogManager.GetLogger<UpdateRunner>(Ssc.SscLog.LogType.Middle);
        private event Action _runnables;

        public void Update() {
            try {
                _runnables?.Invoke();
            } catch (Exception e) {
                Logger.Error(e);
            }
        }

        public void Add(IUpdatable updatable) {
            Remove(updatable);
            _runnables += updatable.Update;
        }

        public void Remove(IUpdatable updatable) {
            _runnables -= updatable.Update;
        }

        private void OnApplicationQuit() {
            ApplicationQuit?.Invoke();
        }
    }
}