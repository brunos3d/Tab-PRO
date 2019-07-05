
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TabPRO.Editor {

	[CreateAssetMenu(menuName = "Tab PRO/Tab Menu")]
	public class TabMenu : ScriptableObject {

		private static readonly string[] DEFAULT_WINDOW_TYPES = new string[] {
			"UnityEditor.ProjectBrowser",
			"UnityEditor.SceneHierarchyWindow",
			"UnityEditor.InspectorWindow",
			"UnityEditor.LightingWindow",
			"UnityEditor.NavMeshEditorWindow",
		};

		public List<TabMenuItem> items;

		private void OnEnable() {
			if (items == null || items.Count == 0) {
				ResetDefaults();
			}
		}

		private void Reset() {
			ResetDefaults();
		}

		public void ResetDefaults() {
			if (items == null) {
				items = new List<TabMenuItem>();
			}
			else {
				items.Clear();
			}

			var path_pattern = "Icons/{0}/{1}";
			var dynamic_folder = EditorGUIUtility.isProSkin ? "Light" : "Dark";

			foreach (string name in DEFAULT_WINDOW_TYPES) {
				var path = string.Format(path_pattern, dynamic_folder, name.Replace('.', '_'));
				var icon = Resources.Load<Texture>(path);
				var tooltip = name.Replace("UnityEditor.", "Open ");
				var content = new GUIContent(icon, tooltip);

				items.Add(new TabMenuItem(content, ItemAction.OpenTabWindow, name));
			}
		}
	}
}
#endif