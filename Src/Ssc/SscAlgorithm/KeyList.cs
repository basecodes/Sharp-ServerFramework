using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ssc.SscAlgorithm {
    public class KeyList<TK,TV> :IEnumerable<KeyValuePair<TK,TV>>{
        private Dictionary<TK, int> _dictionary;
        private List<TV> _list;
        public KeyList() {
            _dictionary = new Dictionary<TK, int>();
            _list = new List<TV>();
        }

        public bool ContainsKey(TK key) {
            return _dictionary.ContainsKey(key);
        }

        public TV[] ToValueArray() {
            return _list.ToArray();
        }

        public TV GetValue(TK key) {
            if (_dictionary.TryGetValue(key, out var value)) {
                return _list[value];
            }
            return default;
        }

        public void AddItem(TK key,TV value) {
            _list.Add(value);
            _dictionary.Add(key, _list.Count - 1);
        }

        public TV RemoveItem(TK key) {
            if (_dictionary.TryGetValue(key,out var index)) {

                var value= _list[index];
                _list[index] = default;
                _dictionary.Remove(key);

                return value;
            }
            return default;
        }

        public void CopyTo(KeyList<TK,TV> keyList) {
            foreach (var item in _dictionary) {
                keyList._dictionary.Add(item.Key,item.Value);
            }

            foreach (var item in _list) {
                keyList._list.Add(item);
            }
        }

        public IEnumerator<KeyValuePair<TK,TV>> GetEnumerator() {
            for (var i = 0; i < _list.Count; i++) {
                if (_list[i] == null) {
                    continue;
                }

                var key =  _dictionary.Where((k,v)=> k.Value == i).Select((k,v)=> k.Key).FirstOrDefault();
                yield return new KeyValuePair<TK, TV>(key,_list[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _list.GetEnumerator();
        }

        public void Clear() {
            _dictionary.Clear();
            _list.Clear();
        }
    }
}
