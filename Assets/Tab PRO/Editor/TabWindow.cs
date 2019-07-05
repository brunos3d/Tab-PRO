#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TabPRO.Editor {
	using Editor = UnityEditor.Editor;
	public class TabWindow : EditorWindow {

		private const float DOCK_ID = 3.14f; // 3.14 = DOCK ID

		private static int current_dock;

		private static TabWindow window;

		private static TabMenu tab_menu;

		[System.NonSerialized]
		private bool resources_loaded;

		private GUIContent settings_content;

		[SerializeField]
		private EditorWindow m_container;

		private static EditorWindow container {
			get {
				if (window) {
					return window.m_container;
				}
				else {
					return null;
				}
			}
			set {
				if (window) {
					window.m_container = value;
				}
			}
		}

		private static int width {
			get {
				return Screen.currentResolution.width;
			}
		}

		private static int height {
			get {
				return Screen.currentResolution.height;
			}
		}

		[MenuItem("Window/Tab PRO/Toggle Tab %t", false, 0)]
		private static void ToggleTabEnableMenu() {
			if (CheckEnableMenu()) {
				DoMenuItemTabEnable();
			}
			else {
				DoMenuItemTabDisable();
			}
		}

		[MenuItem("Window/Tab PRO/Enable Tab", true)]
		private static bool CheckEnableMenu() {
			return Resources.FindObjectsOfTypeAll<TabWindow>().Length == 0;
		}

		[MenuItem("Window/Tab PRO/Disable Tab", true)]
		private static bool CheckDisableMenu() {
			return Resources.FindObjectsOfTypeAll<TabWindow>().Length > 0;
		}

		[MenuItem("Window/Tab PRO/Enable Tab")]
		private static void DoMenuItemTabEnable() {
			Preferences.enableTab = true;
			TabEnable();
		}

		[MenuItem("Window/Tab PRO/Disable Tab")]
		private static void DoMenuItemTabDisable() {
			Preferences.enableTab = false;
			TabDisable();
		}

		internal static void TabEnable() {
			if (!Preferences.enableTab || window != null) return;
			// Prevent duplicate window and overlap
			var instances = Resources.FindObjectsOfTypeAll<TabWindow>();
			if (instances.Length > 0) return;


			window = CreateInstance<TabWindow>();
			window.minSize = Vector2.zero;
			window.ShowPopup();

			window.position = new Rect(0.0f, 44.0f, Preferences.tabWindowWidth, height - 83.0f);

			var rect = new Rect(Preferences.tabWindowWidth, 0.0f, width - Preferences.tabWindowWidth, height - 83.0f);
			MainViewSetPosition(rect);
		}

		internal static void TabDisable() {
			if (Preferences.enableTab) return;

			var instances = Resources.FindObjectsOfTypeAll<TabWindow>();
			foreach (var instance in instances) {
				if (instance != null) {
					instance.Close();
				}
			}
			CloseAllTabs();
			RestoreMainViewPosition();
		}

		private void LoadResources() {
			if (!resources_loaded) {
				string path_pattern = "Icons/{0}/{1}";
				string dynamic_folder = EditorGUIUtility.isProSkin ? "Light" : "Dark";

				tab_menu = Resources.Load<TabMenu>("DefaultTabMenu");

				window = this;

				var path = string.Format(path_pattern, dynamic_folder, "TabPRO_Settings_Icon");
				var icon = Resources.Load<Texture>(path);
				settings_content = new GUIContent(icon, "Open Preferences > Tab PRO > Settings");

				resources_loaded = true;
			}
		}

		private void OnSelectionChange() {
			if (Preferences.inspectorFocus) {
				for (int id = 0; id < tab_menu.items.Count; id++) {
					var item = tab_menu.items[id];
					if (current_dock != id && item.action == ItemAction.OpenTabWindow && item.data == "UnityEditor.InspectorWindow") {
						OpenTabWindow(id, GetWindowType(item.data));
					}
				}
			}
		}

		private void OnGUI() {
			// Debug.Log(GetInstanceID());
			if (!resources_loaded) {
				LoadResources();
			}

			var bkp_color = GUI.color;
			var current_event = Event.current;

			for (int id = 0; id < tab_menu.items.Count; id++) {
				var rect = new Rect(position.width / 2.0f - Preferences.buttonTabSize / 2.0f, id * (Preferences.buttonTabSize + Preferences.buttonPadding), Preferences.buttonTabSize, Preferences.buttonTabSize);

				if (current_dock != id && !rect.Contains(current_event.mousePosition)) {
					GUI.color = new Color(bkp_color.r, bkp_color.g, bkp_color.b, 0.5f);
				}

				// Draw Tab Buttons
				if (GUI.Button(rect, tab_menu.items[id].content, GUI.skin.label)) {
					ExecuteMenuItem(id);
					//OpenTabWindow(dock_id);
				}

				GUI.color = bkp_color;
			}

			// In case of loop break
			GUI.color = bkp_color;

			{
				var rect = new Rect(position.width / 2.0f - Preferences.buttonTabSize / 2.0f, position.height - Preferences.buttonTabSize - 10.0f, Preferences.buttonTabSize, Preferences.buttonTabSize);
				if (!rect.Contains(current_event.mousePosition)) {
					GUI.color = new Color(bkp_color.r, bkp_color.g, bkp_color.b, 0.5f);
				}

				// Settings Button
				if (GUI.Button(rect, settings_content, GUI.skin.label)) {
					OpenTabWindow(-1, typeof(PreferencesWindow));
				}
				GUI.color = bkp_color;
			}

			Repaint();
		}

		private static void CloseAllTabs() {
			CloseCurrentTab();
			CloseLostDocks();
		}

		private static void CloseCurrentTab() {
			if (container) {
				container.Close();
			}
		}

		private static void CloseLostDocks() {
			var instances = Resources.FindObjectsOfTypeAll<EditorWindow>();
			foreach (var instance in instances) {
				if (instance == null) continue;
				// Window width is used to identify DOCK instance
				// Window titleContent is used to identify DOCK instance
				if (instance.position.width == Preferences.dockWindowWidth + DOCK_ID || instance.titleContent.text == "Tab Dock") {
					instance.Close();
				}
			}
		}

		private static void ExecuteMenuItem(int index) {
			var item = tab_menu.items[index];

			switch (item.action) {
				case ItemAction.OpenTabWindow:
					OpenTabWindow(index, GetWindowType(item.data));
					break;
				case ItemAction.ExecuteMenuItem:
					EditorApplication.ExecuteMenuItem(item.data);
					break;
				case ItemAction.SelectAsset:
					Selection.activeInstanceID = int.Parse(item.data);
					break;
			}
		}

		private static void OpenTabWindow(int index, Type type) {
			CloseAllTabs();

			// Close Current
			if (current_dock == index) {
				current_dock = -2;
				var rect = new Rect(Preferences.tabWindowWidth, 0.0f, width - Preferences.tabWindowWidth, height - 83.0f);
				MainViewSetPosition(rect);
			}
			// Open new/other DOCK
			else {
				current_dock = index;
				container = (EditorWindow)CreateInstance(type);
				// Window titleContent is used to identify DOCK instance
				container.ShowPopup();
				container.titleContent = new GUIContent("Tab Dock");
				// Window width is used to identify DOCK instance
				container.position = new Rect(window.position.xMax + 1.0f, window.position.y, Preferences.dockWindowWidth + DOCK_ID, window.position.height);

				if (index > -2) {
					switch (type.FullName) {
						case "UnityEditor.InspectorWindow":
							if (Selection.objects.Length == 0) {
								container.ShowNotification(new GUIContent("Inspector is empty!\nSelect an Object to see more details!"));
							}
							break;
						case "UnityEditor.ProjectBrowser":
							if (Preferences.projectShowMode == 0) {
								EditorApplication.delayCall += () => {
									type.InvokeMethodFrom("SetOneColumn", container);
								};
							}
							break;

					}
				}

				// 1 = window border
				var rect = new Rect(Preferences.dockWindowWidth + DOCK_ID + 1 + Preferences.tabWindowWidth, 0.0f, width - (Preferences.dockWindowWidth + DOCK_ID + Preferences.tabWindowWidth), height - 1 - 83.0f);
				MainViewSetPosition(rect);
			}
		}

		private static Type GetWindowType(string type) {
			var result = typeof(Editor).Assembly.GetType(type);

			if (result.IsSubclassOf(typeof(EditorWindow))) {
				return result;
			}
			else {
				return null;
			}
		}

		private static void RestoreMainViewPosition() {
			var rect = new Rect(0.0f, 0.0f, width, height - 83.0f);
			MainViewSetPosition(rect);
		}

		private static void MainViewSetPosition(Rect position) {
			var mainview = typeof(Editor).Assembly.GetType("UnityEditor.MainView");
			var instance = Resources.FindObjectsOfTypeAll(mainview)[0];
			mainview.InvokeMethodFrom("SetPosition", instance, position);
		}
	}
}
#endif
