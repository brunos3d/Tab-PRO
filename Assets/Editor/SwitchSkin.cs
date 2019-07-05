using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;

public class SwitchSkin {

	[MenuItem("Tools/Change Skin")]
	public static void Change() {
		var type = typeof(AssetStore).Assembly.GetType("UnityEditorInternal.InternalEditorUtility");
		if (type != null) {
			var method = type.GetMethod("SwitchSkinAndRepaintAllViews", BindingFlags.Public | BindingFlags.Static);
			method.Invoke(null, null);
			Debug.Log("Skin Changed");
		}
	}
}
