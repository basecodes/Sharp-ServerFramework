using Ssc;
using Ssc.Ssc;
using Ssu;
using Ssu.SsuBehaviour;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class Request : SsuMonoBehaviour {

    public Button btn1;
    public Button btn2;
    public Button btn3;
    public Button btn4;
    public Text Text1;
    public Text Text2;
    public Text Text3;
    public Text Text4;

    void Start() {
        btn1.onClick.AddListener(() => {
            Ssui.Invoke<ITestRequest>(() => (t) => t.TestRequest(100, "string"));
        });

        btn2.onClick.AddListener(() => {
            var array = new int[] { 1, 2, 3, 4 };
            Ssui.Invoke<ITestRequest>(() => (t) => t.TestRequest(100, "string", array));
        });

        btn3.onClick.AddListener(() => {
            var array = new int[] { 1, 2, 3, 4 };
            var packets = new ITestPacket[] {
                new TestPacket(){Name = "abc",Password="123"},
                new TestPacket(){Name = "cde",Password="456"},
            };
            Ssui.Invoke<ITestRequest>(() => (t) => t.TestRequest(100, "string", array, packets));
        });

        btn4.onClick.AddListener(() => {
            var dict = new Dictionary<int, ITestPacket>() {
                {1, new TestPacket(){Name = "abc",Password="123"}},
                {2, new TestPacket(){Name = "cde",Password="456"}}
            };

            Ssui.Invoke<ITestRequest>(() => (t) => t.TestRequest("Test", dict));
        });

        Register<ITestResponse>(
            (resp) => resp.TestResponse(null, null, null,null),
            () => this.TestResponse(null, null, null,null));

        Register<ITestResponse>(
            (resp) => resp.TestResponse(0, null, null,null),
            () => this.TestResponse(0, null, null,null));

        Register<ITestResponse>(
            (resp) => resp.TestResponse(0, null, null, null,null),
            () => this.TestResponse(0, null, null, null,null));

        Register<ITestResponse>(
           (resp) => resp.TestResponse(0, null, null, null, null,null),
           () => this.TestResponse(0, null, null, null, null,null));

        Ssui.RegisterPacket<ITestPacket, TestPacket>();
    }

    void TestResponse(int num, string str, IPeer peer, Action<Action> action) {
        Text1.text = num + " " + str;
    }

    void TestResponse(int num, string str, int[] array, IPeer peer, Action<Action> action) {
        var tmp = num + " " + str;
        foreach (var item in array) {
            tmp += " " + item;
        }
        Text2.text = tmp;
    }

    void TestResponse(int num, string str, int[] array, ITestPacket[] testPackets, IPeer peer, Action<Action> action) {
        var tmp = num + " " + str;
        foreach (var item in array) {
            tmp += " " + item;
        }

        foreach (var item in testPackets) {
            tmp += " " + item.Name;
        }
        Text3.text = tmp;
    }

    void TestResponse(string request, Dictionary<int, ITestPacket> testPackets, IPeer peer, Action<Action> action) {
        foreach (var item in testPackets) {
            request += " " + item.Key;
        }

        Text4.text = request;
    }

    void Update() {

    }
}