namespace Ssc.SscTemplate {
    public class SingletonTemplate<T> where T : new() {

        public static T Instance { get; } = new T();
        
        protected SingletonTemplate() {
        }
    }
}