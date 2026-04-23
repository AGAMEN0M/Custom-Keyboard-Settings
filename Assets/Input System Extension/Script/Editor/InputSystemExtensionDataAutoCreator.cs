/*
 * ---------------------------------------------------------------------------
 * Description: Automatically creates or retrieves the InputSystemExtensionData 
 *              asset at editor startup or via menu, ensuring required 
 *              configuration for input system extension is always available.
 *              
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using UnityEditor;
using UnityEngine;
using System.IO;

namespace InputSystemExtension.Editor
{
    #region === Asset Creation System ===

    /// <summary>
    /// Responsible for creating or retrieving the InputSystemExtensionData asset.
    /// Handles folder discovery, creation, and overwrite logic.
    /// </summary>
    public static class InputSystemExtensionDataAutoCreator
    {
        #region === Public API ===

        /// <summary>
        /// Creates or ensures the InputSystemExtensionData asset exists.
        /// </summary>
        /// <param name="allowOverwritePrompt">If true, shows a dialog before replacing an existing asset.</param>
        /// <returns>The created or existing asset.</returns>
        public static InputSystemExtensionData CreateOrGetData(bool allowOverwritePrompt)
        {
            // Try to find the base folder "Input System Extension".
            string baseFolderPath = FindInputSystemExtensionFolder();

            // If not found, fallback to Assets root.
            if (string.IsNullOrEmpty(baseFolderPath))
            {
                Debug.LogWarning("Folder 'Input System Extension' not found. Using Assets/Resources instead.");
                baseFolderPath = "Assets";
            }

            // Ensure the Resources folder exists inside the base folder.
            string resourcesPath = $"{baseFolderPath}/Resources";

            if (!AssetDatabase.IsValidFolder(resourcesPath))
            {
                AssetDatabase.CreateFolder(baseFolderPath, "Resources");
            }

            // Define final asset path.
            string assetPath = $"{resourcesPath}/Input System Extension Data.asset";

            // Try to load an existing asset.
            var existingAsset = AssetDatabase.LoadAssetAtPath<InputSystemExtensionData>(assetPath);

            if (existingAsset != null)
            {
                // If overwrite is not allowed, return existing asset immediately.
                if (!allowOverwritePrompt)
                {
                    return existingAsset;
                }

                // Ask the user if they want to overwrite the existing asset.
                if (!EditorUtility.DisplayDialog(
                    "Replace File",
                    "There is already an 'Input System Extension Data' asset. Do you want to replace it?",
                    "Yes",
                    "No"))
                {
                    return existingAsset;
                }

                // Delete the existing asset before creating a new one.
                AssetDatabase.DeleteAsset(assetPath);
            }

            // Create a new ScriptableObject instance.
            var asset = ScriptableObject.CreateInstance<InputSystemExtensionData>();

            // Create the asset in the project.
            AssetDatabase.CreateAsset(asset, assetPath);

            // Mark asset as dirty to ensure it gets saved.
            EditorUtility.SetDirty(asset);

            // Save and refresh the AssetDatabase.
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return asset;
        }

        /// <summary>
        /// Creates the InputSystemExtensionData asset via Unity menu.
        /// </summary>
        [MenuItem("Assets/Create/Tools/Input System Extension/Input System Extension Data")]
        public static void CreateCustomObjectData()
        {
            // Create or get the asset, allowing overwrite prompt.
            var asset = CreateOrGetData(true);

            // Focus the Project window.
            EditorUtility.FocusProjectWindow();

            // Select the created or existing asset.
            Selection.activeObject = asset;
        }

        #endregion

        #region === Internal Utilities ===

        /// <summary>
        /// Searches the project for the "Input System Extension" folder.
        /// </summary>
        /// <returns>The folder path if found; otherwise null.</returns>
        internal static string FindInputSystemExtensionFolder()
        {
            // Search for folders matching the name.
            var guids = AssetDatabase.FindAssets("Input System Extension t:Folder");

            foreach (var guid in guids)
            {
                // Convert GUID to asset path.
                string path = AssetDatabase.GUIDToAssetPath(guid);

                // Ensure exact folder name match.
                if (Path.GetFileName(path) == "Input System Extension")
                {
                    return path;
                }
            }

            // Return null if not found.
            return null;
        }

        #endregion
    }

    #endregion

    #region === Automatic Asset Creation on Editor Startup ===

    /// <summary>
    /// Ensures the InputSystemExtensionData asset exists when the Unity Editor starts.
    /// </summary>
    [InitializeOnLoad]
    public static class InputSystemExtensionDataStartup
    {
        /// <summary>
        /// Static constructor executed on editor load.
        /// </summary>
        static InputSystemExtensionDataStartup()
        {
            // Delay execution to ensure Unity is fully initialized.
            EditorApplication.delayCall += () =>
            {
                // Ensure the asset exists without prompting the user.
                InputSystemExtensionDataAutoCreator.CreateOrGetData(false);
            };
        }
    }

    #endregion
}