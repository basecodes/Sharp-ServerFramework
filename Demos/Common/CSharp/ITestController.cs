using System;
using System.Collections.Generic;
using System.Text;
using Ssc.Ssc;
using Ssc.SscAttribute;

namespace Common.CSharp {
    public interface ITestController {
        [RpcResponse("5F674579-4D3F-42DE-A72C-A8B46AE94908")]
        bool Test(int num, string str,IPeer peer,Action<Action> callback);

        [RpcResponse("1ECE00D8-614A-481F-861E-D20EEA55247C")]
        bool Test(int num, string str, int[] array,IPeer peer,Action<Action> callback);

        [RpcResponse("26D0A8C7-3D9B-4AC9-B6AF-700A61E23BFB")]
        bool Test(int num, string str, int[] array, ITestPacket[] testPackets,IPeer peer,Action<Action> callback);
    }
}
