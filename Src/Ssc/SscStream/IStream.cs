using System;
using Ssc.Ssc;

namespace Ssc.SscStream {
    public interface IStream:IDisposable,IRecyclable,IAssignable {
        ByteFragment ToByteFragment();
        void Reset();
    }
}