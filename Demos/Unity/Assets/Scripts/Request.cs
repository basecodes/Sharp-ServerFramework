using Ssu;
using Ssu.SsuBehaviour;
using UnityEngine.UI;

public class Request : SsuMonoBehaviour {

    public Button btn;
    void Start() {
        btn.onClick.AddListener(() => {
            Ssui.Invoke<ITest>(() => (t) => t.Test(100));
        });
    }

    void Update() {

    }
}