using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace FullscreenEditor {

    /// <summary>Define a source mode to get a fullscreen rect.</summary>
    public enum RectSourceMode {
        /// <summary>The bounds of the main display.</summary>
        MainDisplay,
        /// <summary>The bounds of the display where the mouse pointer is.</summary>
        AtMousePosition,
        /// <summary>A rect that spans across all the displays. (Windows only)</summary>
        VirtualSpace,
        /// <summary>A custom rect defined by <see cref="FullscreenPreferences.CustomRect"/>.</summary>
        Custom
    }

    /// <summary>Contains preferences for the Fullscreen Editor plugin.</summary>
    [InitializeOnLoad]
    public static class FullscreenPreferences {

        private const string DEVELOPER_EMAIL = "samuelschultze@gmail.com";

        /// <summary>Current version of the Fullscreen Editor plugin.</summary> 
        public static readonly Version pluginVersion = new Version(2, 1, 0);
        /// <summary>Release date of this version.</summary> 
        public static readonly DateTime pluginDate = new DateTime(2019, 04, 07);

        private static readonly GUIContent resetSettingsContent = new GUIContent("Use Defaults", "Reset all settings to default ones");
        private static readonly GUIContent mailDeveloperContent = new GUIContent("Support Email", "Request support from the developer\n\n" + DEVELOPER_EMAIL);
        private static readonly GUIContent versionContent = new GUIContent(string.Format("Version: {0} ({1:d})", pluginVersion, pluginDate));

        internal static Action onLoadDefaults = () => { };
        internal static readonly List<GUIContent> contents = new List<GUIContent>();

        private static readonly PrefItem<Vector2> scroll = new PrefItem<Vector2>("Scroll", Vector2.zero, string.Empty, string.Empty);

        /// <summary>Is the window toolbar currently visible?</summary>
        public static readonly PrefItem<bool> ToolbarVisible;

        /// <summary>Is Fullscreen on Play currently enabled?</summary>
        public static readonly PrefItem<bool> FullscreenOnPlayEnabled;

        /// <summary>Defines a source to get a fullscreen rect.</summary>
        public static readonly PrefItem<RectSourceMode> RectSource;

        /// <summary>Custom rect to be used when <see cref="RectSource"/> is set to <see cref="RectSourceMode.Custom"/>.</summary>
        public static readonly PrefItem<Rect> CustomRect;

        /// <summary>Disable notifications when opening fullscreen windows.</summary>
        public static readonly PrefItem<bool> DisableNotifications;

        static FullscreenPreferences() {
            var rectSourceTooltip = string.Empty;

            rectSourceTooltip += "Controls where Fullscreen Views opens.\n\n";
            rectSourceTooltip += "Primary Screen: Fullscreen opens on the main screen;\n\n";
            rectSourceTooltip += "At Mouse Position: Fullscreen opens on the screen where the mouse pointer is;\n\n";
            rectSourceTooltip += "Virtual Space: Fullscreen spans across all screens (Windows only);\n\n";
            rectSourceTooltip += "Custom Rect: Fullscreen opens on the given custom Rect.";

            ToolbarVisible = new PrefItem<bool>("Toolbar", false, "Toolbar Visible", "Show and hide the toolbar on the top of some windows, like the Game View and Scene View.");
            FullscreenOnPlayEnabled = new PrefItem<bool>("FullscreenOnPlay", false, "Fullscreen On Play", "Override the \"Maximize on Play\" option of the game view to \"Fullscreen on Play\"");
            RectSource = new PrefItem<RectSourceMode>("RectSource", RectSourceMode.AtMousePosition, "Rect Source", rectSourceTooltip);
            CustomRect = new PrefItem<Rect>("CustomRect", FullscreenRects.GetMainDisplayRect(), "Custom Rect", string.Empty);
            DisableNotifications = new PrefItem<bool>("DisableNotifications", false, "Disable Notifications", "Disable the notifications that shows up when opening a new fullscreen view.");

            if (FullscreenUtility.MenuItemHasShortcut(Shortcut.TOOLBAR_PATH))
                ToolbarVisible.Content.text += string.Format(" ({0})", FullscreenUtility.TextifyMenuItemShortcut(Shortcut.TOOLBAR_PATH));
            if (FullscreenUtility.MenuItemHasShortcut(Shortcut.FULLSCREEN_ON_PLAY_PATH))
                FullscreenOnPlayEnabled.Content.text += string.Format(" ({0})", FullscreenUtility.TextifyMenuItemShortcut(Shortcut.FULLSCREEN_ON_PLAY_PATH));
        }

        #if UNITY_2018_3_OR_NEWER
        [SettingsProvider]
        private static SettingsProvider RetrieveSettingsProvider() {
            var settingsProvider = new SettingsProvider("Preferences/Fullscreen Editor", SettingsScope.User, contents.Select(c => c.text));
            settingsProvider.guiHandler = new Action<string>((str) => OnPreferencesGUI(str));
            return settingsProvider;
        }

        #else
        [PreferenceItem("Fullscreen")]
        private static void OnPreferencesGUI() {
            OnPreferencesGUI(string.Empty);
        }
        #endif

        private static void OnPreferencesGUI(string search) {

            if (FullscreenUtility.IsLinux)
                EditorGUILayout.HelpBox("This plugin was not tested on Linux and its behaviour is unknown.", MessageType.Warning);

            EditorGUILayout.Separator();

            #if !UNITY_2018_3_OR_NEWER
            scroll.Value = EditorGUILayout.BeginScrollView(scroll);
            #endif

            EditorGUILayout.Separator();
            ToolbarVisible.DoGUI();
            FullscreenOnPlayEnabled.DoGUI();

            EditorGUILayout.Separator();
            RectSource.DoGUI();
            DisableNotifications.DoGUI();

            if (!IsRectModeSupported(RectSource))
                EditorGUILayout.HelpBox("The selected Rect Source mode is not supported on this platform", MessageType.Warning);

            switch (RectSource.Value) {
                case RectSourceMode.Custom:
                    EditorGUI.indentLevel++;
                    CustomRect.DoGUI();

                    var customRect = CustomRect.Value;

                    if (customRect.width < 300f)
                        customRect.width = 300f;
                    if (customRect.height < 300f)
                        customRect.height = 300f;

                    CustomRect.Value = customRect;

                    EditorGUI.indentLevel--;
                    break;
            }

            EditorGUILayout.Separator();
            Shortcut.DoShortcutsGUI();
            EditorGUILayout.Separator();
            EditorGUILayout.HelpBox("Each item has a tooltip explaining its behaviour", MessageType.Info);

            #if !UNITY_2018_3_OR_NEWER
            EditorGUILayout.EndScrollView();
            #endif

            using(new EditorGUILayout.HorizontalScope()) {
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField(versionContent, GUILayout.Width(170f));
            }

            using(new EditorGUILayout.HorizontalScope()) {
                if (GUILayout.Button(resetSettingsContent, GUILayout.Width(120f)))
                    onLoadDefaults();

                if (GUILayout.Button(mailDeveloperContent, GUILayout.Width(120f)))
                    OpenSupportEmail();
            }

            EditorGUILayout.Separator();
        }

        public static void OpenSupportEmail(Exception e = null) {
            Application.OpenURL(GetEmailURL(e));
        }

        private static string GetEmailURL(Exception e) {
            var full = new StringBuilder();
            var body = new StringBuilder();

            #if UNITY_2018_1_OR_NEWER
            Func<string, string> EscapeURL = url => UnityEngine.Networking.UnityWebRequest.EscapeURL(url).Replace("+", "%20");
            #else
            Func<string, string> EscapeURL = url => WWW.EscapeURL(url).Replace("+", "%20");
            #endif

            body.Append(EscapeURL("\r\nDescribe your problem or make your request here\r\n"));
            body.Append(EscapeURL("\r\nAdditional Information:"));
            body.Append(EscapeURL("\r\nVersion: " + pluginVersion.ToString(3)));
            body.Append(EscapeURL("\r\nUnity " + InternalEditorUtility.GetFullUnityVersion()));
            body.Append(EscapeURL("\r\n" + SystemInfo.operatingSystem));

            if (e != null)
                body.Append(EscapeURL("\r\n" + e));

            full.Append("mailto:");
            full.Append(DEVELOPER_EMAIL);
            full.Append("?subject=");
            full.Append(EscapeURL("Fullscreen Editor - Support"));
            full.Append("&body=");
            full.Append(body);

            return full.ToString();
        }

        internal static bool IsRectModeSupported(RectSourceMode mode) {
            switch (mode) {
                case RectSourceMode.VirtualSpace:
                    return FullscreenUtility.IsWindows;

                case RectSourceMode.Custom:
                case RectSourceMode.MainDisplay:
                case RectSourceMode.AtMousePosition:
                    return true;

                default:
                    return false;
            }
        }

    }

}
