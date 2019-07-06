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
		private float scroll;

		private bool drag_scroll;

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

			var mainview_pos = MainViewGetPosition();

			window.position = new Rect(mainview_pos.x, mainview_pos.y, Preferences.tabWindowWidth, mainview_pos.height);

			var rect = new Rect(Preferences.tabWindowWidth, 0.0f, width - Preferences.tabWindowWidth, mainview_pos.height);
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
				window = this;

				var path_pattern = "Icons/{0}/{1}";
				var dynamic_folder = EditorGUIUtility.isProSkin ? "Light" : "Dark";
				var path = string.Format(path_pattern, dynamic_folder, "TabPRO_Settings_Icon");
				var icon = Resources.Load<Texture>(path);

				settings_content = new GUIContent(icon, "Open Preferences > Tab PRO > Settings");

				tab_menu = Resources.Load<TabMenu>("DefaultTabMenu");

				resources_loaded = true;
			}
		}

		private void OnSelectionChange() {
			if (Preferences.inspectorAutoFocus) {
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

			OnInputGUI();

			var button_tab_size = Preferences.buttonTabSize;
			var button_padding_size = (button_tab_size + Preferences.buttonPadding);

			var tab_items_view = new Rect(0.0f, 0.0f, position.width, position.height - button_tab_size - 10.0f);
			var scroll_rect = new Rect(position.width - 10.0f, 0.0f, 10.0f, tab_items_view.height);

			var scroll_max = tab_menu.items.Count * button_padding_size;
			var scroll_unit = (scroll_max / scroll_rect.height) * button_padding_size;

			if (scroll_max / scroll_rect.height >= 1.0f) {
				scroll = GUI.VerticalScrollbar(scroll_rect, scroll, scroll_unit, 0.0f, scroll_max);
			}
			else {
				scroll = 0.0f;
			}

			GUI.BeginClip(tab_items_view);

			var bkp_color = GUI.color;
			var current_event = Event.current;

			for (int id = 0; id < tab_menu.items.Count; id++) {
				var rect = new Rect(position.width / 2.0f - button_tab_size / 2.0f, id * button_padding_size - scroll, button_tab_size, button_tab_size);

				// Active/Deactive tab button 
				if (current_dock != id && !rect.Contains(current_event.mousePosition)) {
					GUI.color = new Color(bkp_color.r, bkp_color.g, bkp_color.b, 0.5f);
				}

				// Draw Tab Buttons
				if (GUI.Button(rect, tab_menu.items[id].content, GUI.skin.label) && !drag_scroll) {
					ExecuteMenuItem(id);
					//OpenTabWindow(dock_id);
				}

				GUI.color = bkp_color;
			}

			// In case of loop break
			GUI.color = bkp_color;

			GUI.EndClip();

			{
				var rect = new Rect(position.width / 2.0f - button_tab_size / 2.0f, position.height - button_tab_size - 10.0f, button_tab_size, button_tab_size);
				if (!rect.Contains(current_event.mousePosition)) {
					GUI.color = new Color(bkp_color.r, bkp_color.g, bkp_color.b, 0.5f);
				}

				// Settings Button
				if (GUI.Button(rect, settings_content, GUI.skin.label) && !drag_scroll) {
					OpenTabWindow(-1, typeof(PreferencesWindow));
				}
				GUI.color = bkp_color;
			}

			Repaint();
		}

		private void OnInputGUI() {
			Event current_event = Event.current;
			if (current_event.type == EventType.MouseDown) {
				drag_scroll = false;
			}
			if (current_event.type == EventType.MouseDrag) {
				scroll -= current_event.delta.y;
				drag_scroll = true;
			}
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
				if (instance.position.width == Preferences.dockedWindowWidth + DOCK_ID || instance.titleContent.text == "Tab Dock") {
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

			var tab_window_width = Preferences.tabWindowWidth;
			var dock_window_width = Preferences.dockedWindowWidth;

			// Close Current
			if (current_dock == index) {
				current_dock = -2;
				var rect = new Rect(tab_window_width, 0.0f, width - tab_window_width, height - 83.0f);
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
				container.position = new Rect(window.position.xMax + 1.0f, window.position.y, dock_window_width + DOCK_ID, window.position.height);

				if (index > -2) {
					switch (type.FullName) {
						case "UnityEditor.InspectorWindow":
							if (Selection.objects.Length == 0) {
								container.ShowNotification(new GUIContent("Inspector is empty!\nSelect an Object to see more details!"));
							}
							break;
						case "UnityEditor.ProjectBrowser":
							if (Preferences.projectViewMode == 0) {
								EditorApplication.delayCall += () => {
									container.InvokeMethod("SetOneColumn");
								};
							}
							break;

					}
				}

				// 1 = window border
				var rect = new Rect(dock_window_width + DOCK_ID + 1 + tab_window_width, 0.0f, width - (dock_window_width + DOCK_ID + tab_window_width), height - 1 - 83.0f);
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
			instance.InvokeMethod("SetPosition", position);
		}

		private static Rect MainViewGetPosition() {
			var mainview = typeof(Editor).Assembly.GetType("UnityEditor.MainView");
			var instance = Resources.FindObjectsOfTypeAll(mainview)[0];
			return instance.GetPropertyValue<Rect>("screenPosition");
		}
	}
}
#endif
