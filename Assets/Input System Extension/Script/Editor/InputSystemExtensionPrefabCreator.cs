/*
 * ---------------------------------------------------------------------------
 * Description: Editor utility for instantiating Input System Extension UI 
 *              prefabs via the GameObject menu. Automatically handles canvas 
 *              creation, parenting, and prefab setup in the scene.
 * 
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;

namespace InputSystemExtension.Editor
{
    /// <summary>
    /// Utility class to create and instantiate predefined UI prefab objects related to Input System Extension.
    /// Adds entries to the GameObject menu for quick creation.
    /// </summary>
    public static class InputSystemExtensionPrefabCreator
    {
        #region === Utilities ===

        /// <summary>
        /// Creates a Canvas and EventSystem in the scene if none exist.
        /// </summary>
        /// <returns>The created Canvas instance.</returns>
        private static Canvas CreateUICanvas()
        {
            // Create the Canvas GameObject.
            GameObject canvasGO = new("Canvas");

            // Add required UI components.
            var canvas = canvasGO.AddComponent<Canvas>();
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            // Set canvas to render on screen overlay.
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.gameObject.layer = LayerMask.NameToLayer("UI");
            canvas.sortingOrder = 0;
            canvas.targetDisplay = 0;

            // Create the EventSystem GameObject.
            GameObject eventSystemGO = new("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();

            // Register the creation with Undo system.
            Undo.RegisterCreatedObjectUndo(canvasGO, "Create Canvas");
            Undo.RegisterCreatedObjectUndo(eventSystemGO, "Create EventSystem");

            return canvas;
        }

        /// <summary>
        /// Instantiates a prefab by name and parents it to the selected GameObject or the canvas if UI.
        /// </summary>
        /// <param name="fileName">Name of the prefab (without extension).</param>
        /// <param name="selectedGameObject">Currently selected GameObject in hierarchy.</param>
        /// <param name="isUI">If true, will ensure the object is parented under a UI canvas.</param>
        private static void CreateAndConfigurePrefab(string fileName, GameObject selectedGameObject, bool isUI = false)
        {
            // Try to find or create a Canvas if this is a UI prefab.
            Canvas canvas = null;

            if (isUI)
            {
                // Try to find an existing Canvas in the scene.
                canvas = UnityEngine.Object.FindAnyObjectByType<Canvas>();

                // If none is found, create a new one.
                if (canvas == null) canvas = CreateUICanvas();
            }

            // Find the prefab asset in the project.
            var prefab = FindPrefabByName(fileName);
            if (prefab == null)
            {
                Debug.LogError($"Prefab not found: {fileName}.prefab. Ensure it exists in the project.");
                return;
            }

            // Determine parent transform for the new prefab instance.
            var parent = selectedGameObject != null ? selectedGameObject.transform : (isUI ? canvas.transform : null);

            // Instantiate the prefab.
            var instance = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;

            // Final setup for renaming and unpacking.
            FinalizePrefabSetup(fileName, instance);
        }

        /// <summary>
        /// Searches the project for a prefab by name, restricting the search
        /// to the "Input System Extension/Prefab" directory for safety.
        /// </summary>
        /// <param name="prefabName">Name of the prefab (without extension).</param>
        /// <returns>The prefab GameObject asset, or null if not found.</returns>
        public static GameObject FindPrefabByName(string prefabName)
        {
            // Find all prefab assets matching the name.
            var guids = AssetDatabase.FindAssets($"{prefabName} t:Prefab");

            foreach (var guid in guids)
            {
                // Convert GUID to asset path.
                string path = AssetDatabase.GUIDToAssetPath(guid);

                // Normalize path to use forward slashes.
                string normalizedPath = path.Replace("\\", "/");

                // Ensure the prefab is inside the allowed directory.
                if (!normalizedPath.Contains("Input System Extension/Prefab"))
                {
                    continue; // Skip anything outside the safe folder.
                }

                // Check if the file name matches exactly (case-insensitive).
                if (Path.GetFileNameWithoutExtension(normalizedPath).Equals(prefabName, StringComparison.OrdinalIgnoreCase))
                {
                    return AssetDatabase.LoadAssetAtPath<GameObject>(normalizedPath);
                }
            }

            Debug.LogError($"Prefab with the name '{prefabName}' not found in 'Input System Extension/Prefab'.");
            return null;
        }

        /// <summary>
        /// Unpacks the prefab instance, registers Undo, and enables renaming.
        /// </summary>
        /// <param name="fileName">Name of the prefab being instantiated.</param>
        /// <param name="newGameObject">The new instance created from the prefab.</param>
        private static void FinalizePrefabSetup(string fileName, GameObject newGameObject)
        {
            if (newGameObject == null) return;

            // Register creation with Undo system.
            Undo.RegisterCreatedObjectUndo(newGameObject, $"Create {fileName}");

            // Unpack prefab so it's fully editable in scene.
            PrefabUtility.UnpackPrefabInstance(newGameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

            // Select the new object.
            Selection.activeGameObject = newGameObject;

            // Trigger renaming via F2 key on the selected object.
            EditorApplication.delayCall += () =>
            {
                if (Selection.activeGameObject == newGameObject)
                {
                    EditorWindow.focusedWindow.SendEvent(new()
                    {
                        keyCode = KeyCode.F2,
                        type = EventType.KeyDown
                    });
                }
            };
        }

        #endregion

        #region === Menu Items ===

        /// <summary>
        /// Adds a menu option to instantiate the "Rebind Control Manager (Legacy)" UI prefab.
        /// </summary>
        [MenuItem("GameObject/Tools/Input System Extension/UI/Legacy/Rebind Control Manager (Legacy)")]
        public static void CreateRebindControlManagerPrefab() => CreateAndConfigurePrefab("Rebind Control Manager (Legacy)", Selection.activeGameObject, true);

        /// <summary>
        /// Adds a menu option to instantiate the "Button (Reset All) [Legacy]" UI prefab.
        /// </summary>
        [MenuItem("GameObject/Tools/Input System Extension/UI/Legacy/Button (Reset All) [Legacy]")]
        public static void CreateButtonResetAllPrefab() => CreateAndConfigurePrefab("Button (Reset All) [Legacy]", Selection.activeGameObject, true);

        /// <summary>
        /// Adds a menu option to instantiate the "Rebind Control Manager (TMP)" UI prefab.
        /// </summary>
        [MenuItem("GameObject/Tools/Input System Extension/UI/Rebind Control Manager (TMP)")]
        public static void CreateRebindControlManagerTMPPrefab() => CreateAndConfigurePrefab("Rebind Control Manager (TMP)", Selection.activeGameObject, true);

        /// <summary>
        /// Adds a menu option to instantiate the "Button (Reset All) [TMP]" UI prefab.
        /// </summary>
        [MenuItem("GameObject/Tools/Input System Extension/UI/Button (Reset All) [TMP]")]
        public static void CreateButtonResetAllTMPPrefab() => CreateAndConfigurePrefab("Button (Reset All) [TMP]", Selection.activeGameObject, true);

        /// <summary>
        /// Adds a menu option to instantiate the "Input Display Manager" UI prefab.
        /// </summary>
        [MenuItem("GameObject/Tools/Input System Extension/UI/Input Display Manager")]
        public static void CreateInputDisplayManagerPrefab() => CreateAndConfigurePrefab("Input Display Manager", Selection.activeGameObject, true);

        #endregion
    }
}