namespace Ssc.SscPatterns {
    public class Originator<T> {
        public T state_ { get; set; }

        public Memento<T> createMemento() {
            return new Memento<T>(state_);
        }

        public void setMemento(Memento<T> memento) {
            state_ = memento.state_;
        }
    }

    public class Memento<T> {
        public Memento(T state) {
            state_ = state;
        }

        public T state_ { get; }
    }

    public class Caretaker<T> {
        public Memento<T> memento_ { set; get; }
    }
}