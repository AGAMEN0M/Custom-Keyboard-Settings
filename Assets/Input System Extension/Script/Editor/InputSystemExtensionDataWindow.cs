/*
 * ---------------------------------------------------------------------------
 * Description: Provides a Project Settings UI for editing the
 *              InputSystemExtensionData ScriptableObject.
 *              Allows centralized configuration of input bindings
 *              and icon mappings directly from Project Settings.
 * 
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InputSystemExtension.Editor
{
    #region === Settings Provider ===

    /// <summary>
    /// Provides a Project Settings UI for editing the InputSystemExtensionData ScriptableObject.
    /// </summary>
    public static class InputSystemExtensionDataWindow
    {
        /// <summary>
        /// Creates the SettingsProvider used by Unity Project Settings.
        /// </summary>
        /// <returns>Configured SettingsProvider instance.</returns>
        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            return new SettingsProvider("Project/Input System Package/Extension", SettingsScope.Project)
            {
                label = "Extension",

                guiHandler = (searchContext) =>
                {
                    // Attempts to load the data asset.
                    var data = InputSystemExtensionHelper.GetInputSystemExtensionData();

                    EditorGUILayout.Space(5);

                    // Displays general information about the system.
                    EditorGUILayout.HelpBox("Configure Input System Extension settings including bindings and icon mappings.", MessageType.Info);

                    EditorGUILayout.Space(5);

                    #region === Missing Asset ===

                    // If asset does not exist, show creation UI.
                    if (data == null)
                    {
                        EditorGUILayout.HelpBox("InputSystemExtensionData asset not found.", MessageType.Warning);

                        // Button to create or locate the settings asset.
                        if (GUILayout.Button(new GUIContent("Create Settings Asset", "Creates or locates the InputSystemExtensionData asset."), GUILayout.Height(30)))
                        {
                            // Creates or gets the asset.
                            data = InputSystemExtensionDataAutoCreator.CreateOrGetData(false);

                            // Focus newly created asset if valid.
                            if (data != null)
                            {
                                EditorGUIUtility.PingObject(data); // Highlights in Project window.
                                Selection.activeObject = data;     // Selects the asset.
                            }

                            // Forces UI refresh to avoid layout issues after creation.
                            GUIUtility.ExitGUI();
                        }

                        return; // Exit GUI early since no asset exists.
                    }

                    #endregion

                    #region === Asset Reference ===

                    EditorGUILayout.LabelField("Data Asset", EditorStyles.boldLabel);

                    EditorGUILayout.BeginHorizontal();

                    // Displays the ScriptableObject reference in read-only mode.
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField(data, typeof(InputSystemExtensionData), false);
                    EditorGUI.EndDisabledGroup();

                    // Button to ping/select the asset in Project window.
                    if (GUILayout.Button(new GUIContent("Ping", "Highlight asset in Project window."), GUILayout.MaxWidth(50)))
                    {
                        EditorGUIUtility.PingObject(data);
                        Selection.activeObject = data;
                    }

                    // Button to open the asset in Inspector window.
                    if (GUILayout.Button(new GUIContent("Open", "Open asset in Inspector."), GUILayout.MaxWidth(50)))
                    {
                        EditorUtility.OpenPropertyEditor(data);
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space(10);

                    #endregion

                    #region === Tools ===

                    EditorGUILayout.LabelField("Tools", EditorStyles.boldLabel);

                    // Button to auto-generate default icon mappings.
                    if (GUILayout.Button(new GUIContent("Get Default Icons", "Automatically generates default keyboard and gamepad icon mappings."), GUILayout.Height(30)))
                    {
                        // Registers undo before modifying the asset.
                        Undo.RecordObject(data, "Generate Default Icons");

                        // Executes generation logic.
                        InputSystemExtensionDataInspector.GenerateKeyCodeList(data);

                        // Marks asset as dirty so Unity knows it changed.
                        EditorUtility.SetDirty(data);

                        // Forces asset save to disk.
                        AssetDatabase.SaveAssets();
                    }

                    EditorGUILayout.Space(10);

                    #endregion

                    #region === Draw Inspector ===

                    // Creates a serialized representation of the asset.
                    SerializedObject so = new(data);

                    // Synchronizes serialized data with the current object state.
                    so.Update();

                    // Iterator to draw all serialized fields.
                    SerializedProperty prop = so.GetIterator();

                    bool enterChildren = true;

                    // Loops through all visible properties.
                    while (prop.NextVisible(enterChildren))
                    {
                        // Skip script reference field (readonly by Unity).
                        if (prop.name == "m_Script") continue;

                        // Draws the property with children support.
                        EditorGUILayout.PropertyField(prop, true);

                        enterChildren = false;
                    }

                    // Registers undo before applying changes made via inspector.
                    Undo.RecordObject(data, "Modify Input System Extension Data");

                    // Applies changes from SerializedObject back to the real object.
                    if (so.ApplyModifiedProperties())
                    {
                        // Marks asset as dirty for saving.
                        EditorUtility.SetDirty(data);

                        // Saves asset to disk immediately (optional but consistent).
                        AssetDatabase.SaveAssets();
                    }

                    #endregion
                },

                keywords = new HashSet<string> { "Input", "Bindings", "Gamepad", "Icons", "Controls" }
            };
        }
    }

    #endregion
}