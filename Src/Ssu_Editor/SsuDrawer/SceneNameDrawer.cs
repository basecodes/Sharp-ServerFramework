using System.Collections.Generic;
using System.Linq;
using Ssu.SsuAttribute;
using UnityEditor;
using UnityEngine;

namespace Ssu_Editor.SsuDrawer {
    [CustomPropertyDrawer(typeof(SceneNameAttribute))]
    public class SceneNameDrawer : PropertyDrawer {
        private SceneNameAttribute sceneNameAttribute {
            get {
                return (SceneNameAttribute)attribute;
            }
        }


        public override void OnGUI(Rect position,
            SerializedProperty property, GUIContent label) {
            var sceneNames = GetEnabledSceneNames();

            if (sceneNames.Length == 0) {
                EditorGUI.LabelField(position,
                    ObjectNames.NicifyVariableName(property.name), "Scene is Empty");
                return;
            }

            var sceneNumbers = new int[sceneNames.Length];

            SetSceneNambers(sceneNumbers, sceneNames);

            if (!string.IsNullOrEmpty(property.stringValue)) {
                sceneNameAttribute.selectedValue = GetIndex(sceneNames, property.stringValue);
            }

            sceneNameAttribute.selectedValue = EditorGUI.IntPopup(position,
                label.text, sceneNameAttribute.selectedValue, sceneNames, sceneNumbers);

            property.stringValue = sceneNames[sceneNameAttribute.selectedValue];
        }

        string[] GetEnabledSceneNames() {
            List<EditorBuildSettingsScene> scenes = null;
            if (sceneNameAttribute.enableOnly) {
                scenes = (from scene in EditorBuildSettings.scenes
                          where scene.enabled
                          select scene).ToList();
            } else {
                scenes = EditorBuildSettings.scenes.ToList();
            }
            var sceneNames = new HashSet<string>();
            scenes.ForEach(scene => {
                sceneNames.Add(scene.path.Substring(scene.path.LastIndexOf("/") + 1).
                    Replace(".unity", string.Empty));
            });
            return sceneNames.ToArray();
        }

        void SetSceneNambers(int[] sceneNumbers, string[] sceneNames) {
            for (var i = 0; i < sceneNames.Length; i++) {
                sceneNumbers[i] = i;
            }
        }

        int GetIndex(string[] sceneNames, string sceneName) {
            var result = 0;
            for (var i = 0; i < sceneNames.Length; i++) {
                if (sceneName == sceneNames[i]) {
                    result = i;
                    break;
                }
            }
            return result;
        }
    }
}
