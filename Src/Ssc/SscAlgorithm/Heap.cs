using System;
using System.Collections.Generic;

namespace Ssc.SscAlgorithm {
    public class Heap<TPriority, TValue> {
        private readonly TValue _defaultValue;

        private readonly HeapItem[] _collection;
        private readonly Func<TPriority,TPriority,int> _comparer;
        private int _lastIndex;

        public Heap(int maxSice,Func<TPriority, TPriority, int> comparer, TValue defaultValue) {
            _comparer = comparer;
            _collection = new HeapItem[maxSice];
            _lastIndex = -1;
            _defaultValue = defaultValue;
        }

        public int MaxSize => _collection.Length;

        private int GetParentIndex(int index) {
            return index <= 0 ? -1 : (int) Math.Floor(((double) index - 1) / 2);
        }

        private int GetLeftChildIndex(int index) {
            var leftIndex = 2 * index + 1;
            return leftIndex > _lastIndex ? -1 : leftIndex;
        }

        private int GetRigtChildIndex(int index) {
            var rightIndex = 2 * index + 2;
            return rightIndex > _lastIndex ? -1 : rightIndex;
        }

        private void Exchange(int index1, int index2) {
            var temp = _collection[index1];
            _collection[index1] = _collection[index2];
            _collection[index2] = temp;
        }

        private void Heapify(int index) {
            var leftIndex = GetLeftChildIndex(index);

            if (leftIndex == -1) {
                return;
            }

            var rightIndex = GetRigtChildIndex(index);

            if (rightIndex == -1) {
                if (_comparer(_collection[leftIndex].Priority,_collection[index].Priority) < 0){
                    Exchange(leftIndex, index);
                }

                return;
            }

            HeapItem minOrMaxItem;

            if (_comparer(_collection[rightIndex].Priority,_collection[leftIndex].Priority) < 0){
                minOrMaxItem = _collection[rightIndex];
            }else{
                minOrMaxItem = _collection[leftIndex];
            }

            if (_comparer(minOrMaxItem.Priority,_collection[index].Priority) < 0) {
                if (minOrMaxItem == _collection[rightIndex]) {
                    Exchange(rightIndex, index);
                    Heapify(rightIndex);
                } else {
                    Exchange(leftIndex, index);
                    Heapify(leftIndex);
                }
            }
        }

        public TValue Peek() => _lastIndex == -1 ? _defaultValue : _collection[0].Item;

        public void Push(TPriority priority, TValue item) {
            if (_lastIndex >= MaxSize - 1)
                throw new IndexOutOfRangeException($"Heap reached its maximum capacity {MaxSize}");

            _lastIndex++;
            _collection[_lastIndex] = new HeapItem {
                Priority = priority,
                Item = item
            };

            var index = _lastIndex;
            var parentIndex = GetParentIndex(index);
            while (parentIndex != -1 && _comparer(_collection[index].Priority, _collection[parentIndex].Priority) < 0) {
                Exchange(index, parentIndex);
                index = parentIndex;
                parentIndex = GetParentIndex(index);
            }
        }

        public TValue Pop() {
            if (_lastIndex == -1) {
                return _defaultValue;
            }

            var minOrMaxItem = _collection[0];

            _lastIndex--;
            if (_lastIndex != -1) {
                _collection[0] = _collection[_lastIndex + 1];
                Heapify(0);
            }

            return minOrMaxItem.Item;
        }

        public void Clear() {
            Array.Clear(_collection, 0, _collection.Length);
        }

        private class HeapItem {
            public TPriority Priority { get; set; }
            public TValue Item { get; set; }
        }
    }
}