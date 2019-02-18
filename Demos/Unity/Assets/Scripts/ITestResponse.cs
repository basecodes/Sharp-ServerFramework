using Ssc.Ssc;
using Ssc.SscAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface ITestResponse {
    [RpcResponse("9CEF8CD0-8720-4C34-9341-545AF7693AB2")]
    void TestResponse(int num, string str,IPeer peer,Action<Action> action);

    [RpcResponse("4AC85EE0-2616-4EB3-AD50-DA7FB588870C")]
    void TestResponse(int num, string str, int[] array,IPeer peer,Action<Action> action);

    [RpcResponse("444E0735-DA0B-4A29-9746-E7FEFE7E2293")]
    void TestResponse(int num, string str, int[] array, ITestPacket[] testPackets,IPeer peer,Action<Action> action);

    [RpcResponse("C00E44A2-09E0-4C94-81DC-9622AA38EFB4")]
    void TestResponse(string str, Dictionary<int, ITestPacket> testPackets,IPeer peer,Action<Action> action);
}