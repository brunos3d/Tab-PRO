#if UNITY_EDITOR
using System;
using System.Reflection;

namespace TabPRO.Editor {
	public static class ReflectionUtils {

		private const BindingFlags METHOD_BIND_FLAGS = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;

		public static object InvokeMethodFrom(this Type type, string name, object target, params object[] args) {
#if NET_2_0
			var method = GetMethodFrom(type, name);
			return method == null ? null : method.Invoke(target, args);
#else
			return GetMethodFrom(type, name)?.Invoke(target, args);
#endif
		}

		public static MethodInfo GetMethodFrom(this Type type, string name) {
			MethodInfo result = type.GetMethod(name, METHOD_BIND_FLAGS);

			if (result == null) {
				foreach (MethodInfo method in type.GetMethods(METHOD_BIND_FLAGS)) {
					if (method.Name == name) {
						return method;
					}
				}
			}
			return result;
		}
	}
}
#endif