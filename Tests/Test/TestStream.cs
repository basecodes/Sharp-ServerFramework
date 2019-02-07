using NUnit.Framework;
using Ssc.SscStream;
using Ssc.SscTemplate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test {
    [TestFixture]
    public class TestStream {
        [Test]
        public void Test() {
            PoolAllocator<IWriteStream>.SetPool((arguments => new WriteStream()));
            var writeStream = PoolAllocator<IWriteStream>.GetObject();
            writeStream.ShiftRight("Test");
            writeStream.ShiftRight(123);
            writeStream.ShiftRight(5.0f);
            writeStream.ShiftLeft("front");

            var byteFragment = writeStream.ToByteFragment();

            PoolAllocator<IReadStream>.SetPool((arguments => new ReadStream()));
            var readStream = PoolAllocator<IReadStream>.GetObject();
            readStream.CopyBuffer(byteFragment.Buffer, byteFragment.Offset, byteFragment.Count);
            Assert.That(readStream.ShiftRight<string>(), Is.EqualTo("front"));
            Assert.That(readStream.ShiftRight<string>(), Is.EqualTo("Test"));
            Assert.That(readStream.ShiftRight<int>(), Is.EqualTo(123));
            Assert.That(readStream.ShiftRight<float>(), Is.EqualTo(5.0f));
        }
    }
}
