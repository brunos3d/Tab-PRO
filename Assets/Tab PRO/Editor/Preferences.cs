#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TabPRO.Editor {
	public class Preferences : EditorWindow {

		private const string DEVELOPER_EMAIL = "bruno3dcontato@gmail.com";

		private const string ENABLE_PREFS = "TabPRO.Enable";

		private const string INSPECTOR_FOCUS_PREFS = "TabPRO.InspectorFocus";

		private const string PROJECT_SHOW_MODE_PREFS = "TabPRO.ProjectShowMode";

		private const string TAB_WINDOW_WIDTH_PREFS = "TabPRO.TabWindowWidth";

		private const string DOCK_WINDOW_WIDTH_PREFS = "TabPRO.DockWindowWidth";

		private const string BUTTON_PADDING_PREFS = "TabPRO.ButtonPadding";

		private const string BUTTON_TAB_SIZE_PREFS = "TabPRO.ButtonTabSize";

		private static readonly string[] PROJECT_SHOW_MODE_OPTIONS = { "One Column", "Two Columns" };

		public static bool enableTab {
			get {
				return EditorPrefs.GetBool(ENABLE_PREFS, true);
			}
			set {
				EditorPrefs.SetBool(ENABLE_PREFS, value);
			}
		}

		public static bool inspectorFocus {
			get {
				return EditorPrefs.GetBool(INSPECTOR_FOCUS_PREFS, true);
			}
			set {
				EditorPrefs.SetBool(INSPECTOR_FOCUS_PREFS, value);
			}
		}

		public static int projectShowMode {
			get {
				return EditorPrefs.GetInt(PROJECT_SHOW_MODE_PREFS, 0);
			}
			set {
				EditorPrefs.SetInt(PROJECT_SHOW_MODE_PREFS, value);
			}
		}

		public static int tabWindowWidth {
			get {
				return EditorPrefs.GetInt(TAB_WINDOW_WIDTH_PREFS, 50);
			}
			set {
				EditorPrefs.SetInt(TAB_WINDOW_WIDTH_PREFS, value);
			}
		}

		public static int dockWindowWidth {
			get {
				return EditorPrefs.GetInt(DOCK_WINDOW_WIDTH_PREFS, 300);
			}
			set {
				EditorPrefs.SetInt(DOCK_WINDOW_WIDTH_PREFS, value);
			}
		}

		public static float buttonPadding {
			get {
				return EditorPrefs.GetFloat(BUTTON_PADDING_PREFS, 10.0f);
			}
			set {
				EditorPrefs.SetFloat(BUTTON_PADDING_PREFS, value);
			}
		}

		public static float buttonTabSize {
			get {
				return EditorPrefs.GetFloat(BUTTON_TAB_SIZE_PREFS, 40.0f);
			}
			set {
				EditorPrefs.SetFloat(BUTTON_TAB_SIZE_PREFS, value);
			}
		}

		[InitializeOnLoadMethod]
		private static void InitPreferences() {
			LoadPreferences();
		}

		private static void DeleteAllPreferences() {
			EditorPrefs.DeleteKey(ENABLE_PREFS);
			EditorPrefs.DeleteKey(INSPECTOR_FOCUS_PREFS);
			EditorPrefs.DeleteKey(PROJECT_SHOW_MODE_PREFS);
			EditorPrefs.DeleteKey(TAB_WINDOW_WIDTH_PREFS);
			EditorPrefs.DeleteKey(DOCK_WINDOW_WIDTH_PREFS);
			EditorPrefs.DeleteKey(BUTTON_PADDING_PREFS);
			EditorPrefs.DeleteKey(BUTTON_TAB_SIZE_PREFS);
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