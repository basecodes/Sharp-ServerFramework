using System;
using Ssc.Ssc;

namespace Ssc.SscStream {
    public interface IStream:IDisposable,IMemoryable {
        ByteFragment ToByteFragment();
        void Reset();
    }
}