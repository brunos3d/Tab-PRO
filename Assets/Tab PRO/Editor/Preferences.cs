﻿#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TabPRO.Editor {
	public static class Preferences {

		private const string ENABLE_PREFS = "TabPRO.Enable";

		private const string INSPECTOR_FOCUS_PREFS = "TabPRO.InspectorFocus";

		private const string PROJECT_SHOW_MODE_PREFS = "TabPRO.ProjectShowMode";

		private const string TAB_WINDOW_WIDTH_PREFS = "TabPRO.TabWindowWidth";

		private const string DOCK_WINDOW_WIDTH_PREFS = "TabPRO.DockWindowWidth";


		private const string BUTTON_PADDING_PREFS = "TabPRO.ButtonPadding";

		private const string BUTTON_TAB_SIZE_PREFS = "TabPRO.ButtonTabSize";

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

		public static void DeleteAllPreferences() {
			EditorPrefs.DeleteKey(ENABLE_PREFS);
			EditorPrefs.DeleteKey(INSPECTOR_FOCUS_PREFS);
			EditorPrefs.DeleteKey(PROJECT_SHOW_MODE_PREFS);
			EditorPrefs.DeleteKey(TAB_WINDOW_WIDTH_PREFS);
			EditorPrefs.DeleteKey(DOCK_WINDOW_WIDTH_PREFS);
			EditorPrefs.DeleteKey(BUTTON_PADDING_PREFS);
			EditorPrefs.DeleteKey(BUTTON_TAB_SIZE_PREFS);
		}
	}
}
#endif