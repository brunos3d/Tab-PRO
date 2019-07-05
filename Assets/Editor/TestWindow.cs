using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TabPRO.Editor;

public class TestWindow : EditorWindow {

	public static bool fullscreen;

	[MenuItem("Tools/Test Window")]
	static void Init() {
		GetWindow<TestWindow>();
	}

	[MenuItem("Tools/MainView Full %#F1")]
	private static void FullScreen() {
		fullscreen = !fullscreen;
		int width = Screen.currentResolution.width;
		int height = Screen.currentResolution.height;

		if (fullscreen) {
			var rect = new Rect(0.0f, -83.0f, width, height + 83.0f);
			MainViewSetPosition(rect);
		}
		else {
			var rect = new Rect(0.0f, 0.0f, width, height - 83.0f);
			MainViewSetPosition(rect);
		}
		Debug.Log(fullscreen);
	}

	private static void MainViewSetPosition(Rect position) {
		var mainview = typeof(Editor).Assembly.GetType("UnityEditor.MainView");
		var instance = Resources.FindObjectsOfTypeAll(mainview)[0];
		mainview.InvokeMethodFrom("SetPosition", instance, position);
	}
}
