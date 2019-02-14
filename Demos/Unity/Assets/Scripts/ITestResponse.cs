using Ssc.Ssc;
using Ssc.SscAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface ITestResponse {
    [RpcResponse("9CEF8CD0-8720-4C34-9341-545AF7693AB2")]
    void TestResponse(string request, IPeer peer, Action<Action> action);

    [RpcResponse("4AC85EE0-2616-4EB3-AD50-DA7FB588870C")]
    void TestResponse(string request, ITestPacket testPacket, IPeer peer, Action<Action> action);
}