using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using View = UnityEngine.ScriptableObject;
using MainView = UnityEngine.ScriptableObject;
using ContainerWindow = UnityEngine.ScriptableObject;
using TabPRO.Editor;

public class TestWindow : EditorWindow {

	public static bool fullscreen;

	[MenuItem("Tools/Test Window")]
	static void Init() {
		GetWindow<TestWindow>();
	}

	[MenuItem("Tools/MainView Full %g")]
	private static void DoAction() {
		var window = GetWindow<TestWindow>();
		var cur_view = window.GetFieldValue("m_Parent");

		var mainview = typeof(Editor).Assembly.GetType("UnityEditor.MainView");
		var instance = Resources.FindObjectsOfTypeAll(mainview)[0];

		var new_mainview = CreateInstance(mainview);

		SwapViews((View)cur_view, (View)instance);
	}

	protected static void SwapViews(View a, View b) {
		var containerA = a.GetPropertyValue<ContainerWindow>("window");
		var containerB = b.GetPropertyValue<ContainerWindow>("window");

		SetFreezeContainer(containerA, true);
		SetFreezeContainer(containerB, true);

		containerA.SetPropertyValue("rootView", b);
		containerB.SetPropertyValue("rootView", a);

		SetFreezeContainer(containerA, true);
		SetFreezeContainer(containerB, true);
	}

	protected static void SetFreezeContainer(ContainerWindow containerWindow, bool freeze) {
		containerWindow.InvokeMethod("SetFreezeDisplay", freeze);
	}

	private void OnGUI() {
	}

	private void OnFocus() {
	}

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
		mainview.InvokeMethod("SetPosition", instance, position);
	}
}
