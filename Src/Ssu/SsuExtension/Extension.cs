using UnityEngine;

namespace Ssu.SsuExtension {
    public static class Extension {
        public static T GetChildByName<T>(this GameObject gameObject, string name) where T : class {
            var transform = gameObject.transform.Find(name);
            return transform.GetComponent<T>();
        }
    }
}