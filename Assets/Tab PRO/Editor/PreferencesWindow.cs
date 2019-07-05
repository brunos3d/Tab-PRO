
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TabPRO.Editor {
	using static Preferences;

	public class PreferencesWindow : EditorWindow {

		private const string DEVELOPER_EMAIL = "bruno3dcontato@gmail.com";

		private static readonly string[] PROJECT_SHOW_MODE_OPTIONS = { "One Column", "Two Columns" };

		[InitializeOnLoadMethod]
		private static void InitPreferences() {
			LoadPreferences();
		}

		private static void LoadPreferences() {
			EditorApplication.delayCall += () => {
				if (enableTab) {
					TabWindow.TabEnable();
				}
				else {
					TabWindow.TabDisable();
				}
			};
		}

		private void OnDisable() {
			LoadPreferences();
		}

		private void OnGUI() {
			// Preferences GUI
			bool bkp_rt = GUI.skin.label.richText;
			GUI.skin.label.richText = true;

			var layout_rect = new Rect(10.0f, 10.0f, position.width - 20.0f, position.height - 20.0f);

			// Canvas to draw layout elements
			GUILayout.BeginArea(layout_rect);
			{
				// Title
				GUILayout.Label("<size=18>Preferences</size>");

				EditorGUILayout.Separator();

				enableTab = EditorGUILayout.Toggle(new GUIContent("Enable Tab PRO"), enableTab);

				if (!enableTab) {
					EditorGUILayout.HelpBox(@"When closing this window the tab will be closed, because the ""enableTab"" option was disabled!", MessageType.Warning);
					GUILayout.Space(10.0f);
				}

				EditorGUILayout.Separator();

				inspectorFocus = EditorGUILayout.Toggle(new GUIContent("Inspector Focus"), inspectorFocus);
				projectShowMode = EditorGUILayout.Popup(new GUIContent("Project Show Mode:"), projectShowMode, PROJECT_SHOW_MODE_OPTIONS);

				EditorGUILayout.Separator();

				tabWindowWidth = EditorGUILayout.IntSlider(new GUIContent("Tab Window Width:"), tabWindowWidth, 20, 200);
				dockWindowWidth = EditorGUILayout.IntSlider(new GUIContent("Dock Window Width:"), dockWindowWidth, 200, 600);

				EditorGUILayout.Separator();

				buttonPadding = EditorGUILayout.Slider(new GUIContent("Button Padding:"), buttonPadding, 0.0f, 50.0f);
				buttonTabSize = EditorGUILayout.Slider(new GUIContent("Button Tab Size:"), buttonTabSize, 15.0f, 190.0f);

				EditorGUILayout.Separator();

				if (GUILayout.Button(new GUIContent("Use Defaults"), GUILayout.ExpandWidth(false))) {
					DeleteAllPreferences();
				}
			}
			GUILayout.EndArea();

			GUI.skin.label.richText = bkp_rt;
		}
	}
}
#endif