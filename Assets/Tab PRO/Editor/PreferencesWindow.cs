
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace TabPRO.Editor {
	// import Preferences members
	using static Preferences;
	public class PreferencesWindow : EditorWindow {

		private const string DEVELOPER_EMAIL = "bruno3dcontato@gmail.com";

		private static readonly string[] PROJECT_SHOW_MODE_OPTIONS = { "One Column", "Two Columns" };

		private static TabMenu tab_menu;

		private static int selected_index;

		public static ReorderableList tab_reorder_list;


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

		private void OnEnable() {
			tab_menu = Resources.Load<TabMenu>("DefaultTabMenu");
			CreateReorderableList();
		}

		private void CreateReorderableList() {
			tab_reorder_list = new ReorderableList(tab_menu.items, typeof(TabMenuItem), true, true, true, true);

			tab_reorder_list.index = 0;

			tab_reorder_list.drawHeaderCallback = (Rect rect) => {
				EditorGUI.LabelField(rect, "Tab Items");
			};

			tab_reorder_list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
				var menu_item = tab_menu.items[index];
				if (isActive) {
					selected_index = index;
				}
				GUI.Label(rect, menu_item.content.image);
				GUI.Label(new Rect(rect.x + 30, rect.y, rect.width - 60, EditorGUIUtility.singleLineHeight), menu_item.content.tooltip);
			};

			tab_reorder_list.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {

				var path_pattern = "Icons/{0}/{1}";
				var dynamic_folder = EditorGUIUtility.isProSkin ? "Light" : "Dark";
				var path = string.Format(path_pattern, dynamic_folder, "TabPRO_NewTab_Icon");
				var icon = Resources.Load<Texture>(path);

				var item = new TabMenuItem(new GUIContent(icon, "Open PackageManager Window"), ItemAction.ExecuteMenuItem, "Window/Package Manager");

				tab_menu.items.Add(item);
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

				if (tab_menu.items.Count > 0) {
					tab_reorder_list.DoLayoutList();

					GUILayout.BeginVertical(GUI.skin.box);
					{
						selected_index = Mathf.Clamp(selected_index, 0, tab_menu.items.Count);

						// Second pass to avoid "ArgumentOutOfRangeException" error
						if (tab_menu.items.Count > 0) {
							tab_menu.items[selected_index].content.image = (Texture)EditorGUILayout.ObjectField(new GUIContent("Item Icon"), tab_menu.items[selected_index].content.image, typeof(Texture), false);
							tab_menu.items[selected_index].content.text = EditorGUILayout.TextField(new GUIContent("Item Label"), tab_menu.items[selected_index].content.text);
							tab_menu.items[selected_index].content.tooltip = EditorGUILayout.TextField(new GUIContent("Item Tooltip"), tab_menu.items[selected_index].content.tooltip);
							tab_menu.items[selected_index].action = (ItemAction)EditorGUILayout.EnumPopup(new GUIContent("Item Action"), tab_menu.items[selected_index].action);
							GUILayout.Label(new GUIContent("Item Action Data:"));
							tab_menu.items[selected_index].data = EditorGUILayout.TextArea(tab_menu.items[selected_index].data);
						}
					}
					GUILayout.EndVertical();

					EditorGUILayout.Separator();
				}

				if (GUILayout.Button(new GUIContent("Use Defaults"), GUILayout.ExpandWidth(false))) {
					DeleteAllPreferences();
					tab_menu.ResetDefaults();
				}
			}
			GUILayout.EndArea();

			GUI.skin.label.richText = bkp_rt;
		}
	}
}
#endif