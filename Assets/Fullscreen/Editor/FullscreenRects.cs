using System;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FullscreenEditor {
    /// <summary>Helper for getting fullscreen rectangles.</summary>
    public static class FullscreenRects {

        /// <summary>Represents a callback for user defined fullscreen rect calculation.</summary>
        /// <param name="mode">The mode set in <see cref="FullscreenPreferences.RectSource"/></param>
        /// <param name="rect">A rect calculated based on custom logic.</param>
        /// <returns>Whether the rect calculated should be used or not.</returns>
        public delegate bool FullscreenRectCallback(RectSourceMode mode, out Rect rect);

        /// <summary>The number of monitors attached to this machine, returns -1 if the platform is not supported.</summary>
        public static int ScreenCount {
            get {
                #if UNITY_EDITOR_WIN
                const int SM_CMONITORS = 80;
                return GetSystemMetrics(SM_CMONITORS);
                #else
                return -1;
                #endif
            }
        }

        /// <summary>Custom callback to allow the user to specify their own logic to how fullscreens will be arranged.
        /// Check the documentation for usage examples.</summary>
        public static FullscreenRectCallback CustomRectCallback { get; set; }

        /// <summary>Returns a fullscreen rect</summary>
        /// <param name="mode">The mode that will be used to retrieve the rect.</param>
        public static Rect GetFullscreenRect(RectSourceMode mode) {

            if (CustomRectCallback != null) {
                var rect = new Rect();
                var shouldUse = CustomRectCallback(mode, out rect);

                if (shouldUse)
                    return rect;
            }

            switch (mode) {
                case RectSourceMode.MainDisplay:
                    return GetMainDisplayRect();

                case RectSourceMode.AtMousePosition:
                    return FullscreenUtility.IsWindows ?
                        GetDisplayBoundsAtPoint(FullscreenUtility.MousePosition) :
                        GetWorkAreaRect(true);

                case RectSourceMode.VirtualSpace:
                    return FullscreenUtility.IsWindows ?
                        GetVirtualScreenBounds() :
                        GetWorkAreaRect(true);

                case RectSourceMode.Custom:
                    return GetCustomUserRect();

                default:
                    Logger.Warning("Invalid fullscreen mode, please fix this by changing the rect source mode in preferences.");
                    return new Rect(Vector2.zero, Vector2.one * 300f);
            }
        }

        /// <summary>Returns a rect with the dimensions of the main screen.
        /// (Note that the position may not be right for multiple screen setups)</summary>
        public static Rect GetMainDisplayRect() {
            return new Rect(0f, 0f, Screen.currentResolution.width, Screen.currentResolution.height);
        }

        /// <summary>Returns a rect defined by the user in the preferences.</summary>
        public static Rect GetCustomUserRect() {
            return FullscreenPreferences.CustomRect;
        }

        /// <summary>Returns a rect covering all the screen, except for the taskbar/dock.
        /// On Windows it adds a 4px border and does not account for scaling (can cause bugs when using scales different than 100%).
        /// On macOS this returns a fullscreen rect when the main window is maximized and mouseScreen is set to true.</summary>
        /// <param name="mouseScreen">Should we get the rect on the screen where the mouse pointer is?</param>
        public static Rect GetWorkAreaRect(bool mouseScreen) {
            return Types.ContainerWindow.InvokeMethod<Rect>("FitRectToScreen", new Rect(Vector2.zero, Vector2.one * 10000f), true, mouseScreen);
        }

        /// <summary>Returns a rect covering all the screen, except for the taskbar/dock.
        /// On Windows it adds a 4px border and does not account for scaling (can cause bugs when using scales different than 100%).
        /// On macOS this returns a fullscreen rect when the main window is maximized and mouseScreen is set to true.</summary>
        /// <param name="container">The ContainerWindow that will be used as reference for calulating border error.</param>
        /// <param name="mouseScreen">Should we get the rect on the screen where the mouse pointer is?</param>
        public static Rect GetWorkAreaRect(Object container, bool mouseScreen) {
            return container.InvokeMethod<Rect>("FitWindowRectToScreen", new Rect(Vector2.zero, Vector2.one * 10000f), true, mouseScreen);
        }

        /// <summary>Returns the bounds rect of the screen that contains the given point. (Windows only)</summary>
        /// <param name="point">The point relative to <see cref="RectSourceMode.VirtualSpace"/></param>
        public static Rect GetDisplayBoundsAtPoint(Vector2 point) {
            return InternalEditorUtility.GetBoundsOfDesktopAtPoint(point);
        }

        /// <summary>Full virtual screen bounds, spanning across all monitors. (Windows only)</summary>
        public static Rect GetVirtualScreenBounds() {
            #if UNITY_EDITOR_WIN
            const int SM_XVIRTUALSCREEN = 76;
            const int SM_YVIRTUALSCREEN = 77;
            const int SM_CXVIRTUALSCREEN = 78;
            const int SM_CYVIRTUALSCREEN = 79;

            var x = GetSystemMetrics(SM_XVIRTUALSCREEN);
            var y = GetSystemMetrics(SM_YVIRTUALSCREEN);
            var width = GetSystemMetrics(SM_CXVIRTUALSCREEN);
            var height = GetSystemMetrics(SM_CYVIRTUALSCREEN);

            return new Rect {
                yMin = y,
                xMin = x,
                width = width,
                height = height,
            };
            #else
            throw new NotImplementedException();
            #endif
        }

        #if UNITY_EDITOR_WIN
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetSystemMetrics(int smIndex);
        #endif

    }
}
