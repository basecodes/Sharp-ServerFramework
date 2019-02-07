using UnityEngine;
using UnityEngine.Networking;

namespace Ssu.SsuAttribute {
    public class PreviewTextureAttribute : PropertyAttribute {
        public Rect lastPosition = new Rect(0, 0, 0, 0);
        public long expire = 6000000000; // 10min
        public UnityWebRequest webRequest;
        public Texture2D texture;

        public PreviewTextureAttribute() {

        }

        public PreviewTextureAttribute(int expire) {
            this.expire = expire * 1000 * 10000;
        }
    }
}
