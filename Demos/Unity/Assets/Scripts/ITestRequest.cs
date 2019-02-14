using Ssc.Ssc;
using Ssc.SscAttribute;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITestRequest {
    [RpcRequest("5F674579-4D3F-42DE-A72C-A8B46AE94908")]
    bool Test(int num, string str);

    [RpcRequest("1ECE00D8-614A-481F-861E-D20EEA55247C")]
    bool Test(int num, string str, int[] array);

    [RpcRequest("26D0A8C7-3D9B-4AC9-B6AF-700A61E23BFB")]
    bool Test(int num, string str, int[] array, ITestPacket[] testPackets);
}
