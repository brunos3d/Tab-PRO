#if UNITY_EDITOR
using System;
using System.Reflection;

namespace TabPRO.Editor {
	public static class ReflectionUtils {

		private const BindingFlags BINDING_FLAGS = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;

		public static T GetFieldValue<T>(this Type type, string name) {
			return (T)type.FindField(name).GetValue(null);
		}

		public static T GetFieldValue<T>(this object obj, string name) {
			return (T)obj.GetType().FindField(name).GetValue(obj);
		}

		public static object GetFieldValue(this object obj, string name) {
			return obj.GetType().FindField(name).GetValue(obj);
		}

		public static void SetFieldValue(this Type type, string name, object value) {
			type.FindField(name).SetValue(null, value);
		}

		public static void SetFieldValue(this object obj, string name, object value) {
			obj.GetType().FindField(name).SetValue(obj, value);
		}

		public static T GetPropertyValue<T>(this Type type, string name) {
			return (T)type.FindProperty(name).GetValue(null, null);
		}

		public static T GetPropertyValue<T>(this object obj, string name) {
			return (T)obj.GetType().FindProperty(name).GetValue(obj, null);
		}

		public static void SetPropertyValue(this Type type, string name, object value) {
			type.FindProperty(name).SetValue(null, value, null);
		}

		public static void SetPropertyValue(this object obj, string name, object value) {
			obj.GetType().FindProperty(name).SetValue(obj, value, null);
		}

		public static T InvokeMethod<T>(this Type type, string name, params object[] args) {
			return (T)type.FindMethod(name).Invoke(null, args);
		}

		public static T InvokeMethod<T>(this object obj, string name, params object[] args) {
			return (T)obj.GetType().FindMethod(name).Invoke(obj, args);
		}

		public static void InvokeMethod(this Type type, string name, params object[] args) {
			type.FindMethod(name).Invoke(null, args);
		}

		public static void InvokeMethod(this object obj, string name, params object[] args) {
			obj.GetType().FindMethod(name).Invoke(obj, args);
		}

		public static FieldInfo FindField(this Type type, string name) {
			return type.GetField(name, BINDING_FLAGS);
		}

		public static PropertyInfo FindProperty(this Type type, string name) {
			return type.GetProperty(name, BINDING_FLAGS);
		}

		public static MethodInfo FindMethod(this Type type, string name) {
			MethodInfo result = null;

			foreach (MethodInfo method in type.GetMethods(BINDING_FLAGS)) {
				if (method.Name == name) {
					result = method;
					break;
				}
			}

			return result;
		}
	}
}
#endif