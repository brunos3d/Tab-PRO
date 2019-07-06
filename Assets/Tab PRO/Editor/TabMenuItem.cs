#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace TabPRO.Editor {
	public enum ItemAction {
		OpenTabWindow,
		OpenWindow,
		ExecuteMenuItem,
		SelectAsset
	}

	[System.Serializable]
	public class TabMenuItem {

		public string dynamic_icon;

		public GUIContent content;

		public ItemAction action;

		public string data;

		public TabMenuItem() { }

		public TabMenuItem(GUIContent content, ItemAction action, string data) {
			this.data = data;
			this.action = action;
			this.content = content;
		}
	}
}
#endif