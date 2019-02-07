using System;

namespace Ssc.SscPool {
    public struct Element<T> {
        public int NextIndex { get; set; }
        public T Object { get; set; }
    }

    public class MemoryPool<T> {
        private readonly Element<T>[] _elements;
        private int _offset;

        public MemoryPool(int length) {
            Length = length;
            _offset = 0;
            _elements = new Element<T>[Length];
            for (var i = 0; i < Length; ++i) {
                _elements[i] = new Element<T>();
                if (Length - 1 == i){
                    _elements[i].NextIndex = -1;
                } else {
                    _elements[i].NextIndex = i + 1;
                }
            }
        }

        public int Length { get; }

        public int Pop() {
            lock (_elements) {
                if (_offset == -1) {
                    throw new ArgumentOutOfRangeException(nameof(_offset));
                }

                var offset = _offset;
                _offset = _elements[_offset].NextIndex;
                return offset;
            }
        }

        public void Push(int index) {
            lock (_elements) {
                if (index >= Length || index < 0) {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                _elements[index].NextIndex = _offset;
                _offset = index;
            }
        }

        public void Set(int index, T element) {
            lock (_elements) {
                if (index > Length || index < 0) {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                _elements[index].Object = element;
            }
        }

        public T Get(int index) {
            lock (_elements) {
                if (index > Length || index < 0) {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                return _elements[index].Object;
            }
        }
    }
}