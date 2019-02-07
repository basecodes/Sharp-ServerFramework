using NUnit.Framework;
using Ssc.SscAlgorithm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test {
    public class Key {
        public Key(int n) {
            num = n;
        }

        public int num { get; set; }
    }

    public class Value{
        public Value(string s) {
            str = s;
        }

        public string str { get; set; }
    }

    [TestFixture]
    public class TestAlgorithm {
        [Test]
        public void Test() {
            int Compare(Key value1, Key value2) {
                return value1.num - value2.num;
            }

            var heap = new Heap<Key, Value>(100, Compare, new Value("0"));

            heap.Push(new Key(8), new Value("8"));
            heap.Push(new Key(100), new Value("100"));
            heap.Push(new Key(108), new Value("108"));
            heap.Push(new Key(5), new Value("5"));
            heap.Push(new Key(56), new Value("56"));
            heap.Push(new Key(78), new Value("78"));
            heap.Push(new Key(1000), new Value("1000"));
            heap.Push(new Key(98), new Value("98"));


            Assert.That(heap.Pop().str, Is.EqualTo(new Value("5").str));
            Assert.That(heap.Pop().str, Is.EqualTo(new Value("8").str));
            Assert.That(heap.Pop().str, Is.EqualTo(new Value("56").str));
            Assert.That(heap.Pop().str, Is.EqualTo(new Value("78").str));
            Assert.That(heap.Pop().str, Is.EqualTo(new Value("98").str));
            Assert.That(heap.Pop().str, Is.EqualTo(new Value("100").str));
            Assert.That(heap.Pop().str, Is.EqualTo(new Value("108").str));
            Assert.That(heap.Pop().str, Is.EqualTo(new Value("1000").str));
        }
    }
}
