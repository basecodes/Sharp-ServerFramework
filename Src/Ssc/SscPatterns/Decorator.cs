namespace Ssc.SscPatterns {
    public interface Component<T> {
        void operation();
    }

    public class Decorator<T> : Component<T> {
        private Component<T> _component;

        public Decorator(Component<T> component) {
            _component = component;
        }

        public void operation() {
        }
    }
}