using Ssc.SscAttribute;
using System.Collections.Generic;

public interface ITestRequest {
    [RpcRequest("9CEF8CD0-8720-4C34-9341-545AF7693AB2")]
    void TestRequest(int num, string str);

    [RpcRequest("4AC85EE0-2616-4EB3-AD50-DA7FB588870C")]
    void TestRequest(int num, string str, int[] array);

    [RpcRequest("444E0735-DA0B-4A29-9746-E7FEFE7E2293")]
    void TestRequest(int num, string str, int[] array, ITestPacket[] testPackets);

    [RpcRequest("C00E44A2-09E0-4C94-81DC-9622AA38EFB4")]
    void TestRequest(string str, Dictionary<int, ITestPacket> testPackets);
}