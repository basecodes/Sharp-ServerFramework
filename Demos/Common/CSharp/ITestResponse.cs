using System;
using System.Collections.Generic;
using Ssc.Ssc;
using Ssc.SscAttribute;

public interface ITestResponse {
    [RpcResponse("5F674579-4D3F-42DE-A72C-A8B46AE94908")]
    bool TestResponse(int num, string str, IPeer peer, Action<Action> callback);

    [RpcResponse("1ECE00D8-614A-481F-861E-D20EEA55247C")]
    bool TestResponse(int num, string str, int[] array, IPeer peer, Action<Action> callback);

    [RpcResponse("26D0A8C7-3D9B-4AC9-B6AF-700A61E23BFB")]
    bool TestResponse(int num, string str, int[] array, ITestPacket[] testPackets, IPeer peer, Action<Action> callback);

    [RpcResponse("4844F488-2169-4AAE-A93B-56E45E10495B")]
    void TestResponse(string str, Dictionary<int, ITestPacket> testPackets, IPeer peer, Action<Action> action);
}