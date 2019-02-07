using Ssc.SscAttribute;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITest {
    [RpcRequest("100")]
    bool Test(int num);
}
