using Ssc;
using Ssc.Ssc;
using Ssu;
using Ssu.SsuBehaviour;
using System;
using UnityEngine.UI;

public class Request : SsuMonoBehaviour {

    public Button btn1;
    public Button btn2;
    public Button btn3;
    public Text Text1;
    public Text Text2;
    public Text Text3;
    void Start() {
        btn1.onClick.AddListener(() => {
            Ssui.Invoke<ITestRequest>(() => (t) => t.Test(100, "string"));
        });

        btn2.onClick.AddListener(() => {
            var array = new int[] { 1, 2, 3, 4 };
            Ssui.Invoke<ITestRequest>(() => (t) => t.Test(100, "string", array));
        });

        btn3.onClick.AddListener(() => {
            var array = new int[] { 1, 2, 3, 4 };
            var packets = new ITestPacket[] {
                new TestPacket(){Name = "abc",Password="123"},
                new TestPacket(){Name = "cde",Password="456"},
            };
            Ssui.Invoke<ITestRequest>(() => (t) => t.Test(100, "string", array, packets));
        });

        Register<ITestResponse>(
            (resp) => resp.TestResponse(null, null, null),
            () => this.TestResponse(null, null, null));

        Register<ITestResponse>(
            (resp) => resp.TestResponse(null, null, null, null),
            () => this.TestResponse(null, null, null, null));

        Ssui.RegisterPacket<ITestPacket, TestPacket>();
    }

    private void TestResponse(string request, ITestPacket testPacket,IPeer peer, Action<Action> action) {
        Text3.text = request+" "+testPacket.Name+" "+testPacket.Password;
    }
    private void TestResponse(string request, IPeer peer, Action<Action> action) {
        Text1.text = request;
    }

    void Update() {

    }
}