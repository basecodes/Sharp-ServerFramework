using Ssc.SscAttribute;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.CSharp {
    public interface ITestRequest {
        [RpcRequest("9CEF8CD0-8720-4C34-9341-545AF7693AB2")]
        void TestRequest(string request);

        [RpcRequest("4AC85EE0-2616-4EB3-AD50-DA7FB588870C")]
        void TestRequest(string request,ITestPacket testPacket);
    }
}
