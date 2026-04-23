/*
 * ---------------------------------------------------------------------------
 * Description: Custom attribute and drawer for selecting an InputAction from 
 *              a default InputActionAsset in the inspector using [GetAction].
 * 
 * Using: [GetAction]
 * 
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.InputSystem;
using System.Linq;
using UnityEditor;
#endif

namespace InputSystemExtension
{
    #region === Attribute Definition ===

    /// <summary>
    /// Attribute to show a dropdown in the inspector listing InputActionReference
    /// options from the default InputActionAsset configured in InputSystemExtensionData.
    /// </summary>
    public class GetActionAttribute : PropertyAttribute { }

    #endregion

#if UNITY_EDITOR

    #region === Property Drawer ===

    /// <summary>
    /// Custom PropertyDrawer for the [GetAction] attribute.
    /// Allows selecting an InputActionReference from actions in the configured InputActionAsset.
    /// </summary>
    [CustomPropertyDrawer(typeof(GetActionAttribute))]
    public class GetActionDrawer : PropertyDrawer
    {
        private InputSystemExtensionData extensionData; // Cached reference to the ScriptableObject that holds the default InputActionAsset.

        /// <summary>
        /// Main method that draws the custom field in the inspector.
        /// </summary>
        /// <param name="position">Rect area where the field will be drawn.</param>
        /// <param name="property">The serialized property being drawn.</param>
        /// <param name="label">Label for the field in the inspector.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Validate that the property is an ObjectReference (required for InputActionReference).
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.HelpBox(position, "Use [GetAction] with InputActionReference.", MessageType.Error);
                return;
            }

            // Load the ScriptableObject if not yet loaded.
            if (extensionData == null)
            {
                extensionData = InputSystemExtensionHelper.GetInputSystemExtensionData();
            }

            // Check if the ScriptableObject or its default InputActionAsset is missing.
            if (extensionData == null || extensionData.defaultInputAction == null)
            {
                EditorGUI.HelpBox(position, "Missing InputSystemExtensionData or defaultInputAction.", MessageType.Error);
                return;
            }

            // Gather all valid actions from all action maps in the default InputActionAsset.
            var actions = extensionData.defaultInputAction.actionMaps
                .Where(map => map != null)
                .SelectMany(map => map.actions)
                .Where(action => action != null)
                .ToList();

            // Display an error if no actions are found.
            if (actions.Count == 0)
            {
                EditorGUI.HelpBox(position, "No actions found in asset.", MessageType.Warning);
                return;
            }

            // Get the currently assigned InputActionReference from the property.
            var currentRef = property.objectReferenceValue as InputActionReference;

            // Check if current reference is valid and belongs to the asset.
            bool isInvalidReference = false;
            string invalidLabel = "";

            if (currentRef != null && currentRef.action != null)
            {
                // Check if the current action exists in the asset list.
                if (!actions.Contains(currentRef.action))
                {
                    isInvalidReference = true;

                    // Build a readable label for the invalid action.
                    invalidLabel = currentRef.action.actionMap != null
                        ? $"[⚠ INVALID] {currentRef.action.actionMap.name}/{currentRef.action.name}"
                        : $"[⚠ INVALID] {currentRef.action.name}";
                }
            }

            // Build display list.
            var displayList = actions.Select(a => a.actionMap != null ? $"{a.actionMap.name}/{a.name}" : a.name).ToList();

            int currentIndex = -1;

            if (isInvalidReference)
            {
                // Insert invalid item at the top.
                displayList.Insert(0, invalidLabel);
                currentIndex = 0;
            }
            else if (currentRef != null && currentRef.action != null)
            {
                currentIndex = actions.FindIndex(a => a == currentRef.action);
            }

            string[] displayNames = displayList.ToArray();

            EditorGUI.LabelField(position, new GUIContent("", label.tooltip));

            // Calculate rects (leave space for warning if needed).
            Rect popupRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            // Draw popup.
            int selectedIndex = EditorGUI.Popup(popupRect, label.text, currentIndex, displayNames);

            // Draw warning box if invalid.
            if (isInvalidReference)
            {
                Rect helpBoxRect = new(
                    position.x,
                    position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                    position.width,
                    EditorGUIUtility.singleLineHeight * 2
                    );

                EditorGUI.HelpBox(helpBoxRect, "InputActionReference does not belong to the configured InputActionAsset.", MessageType.Warning);
            }

            // Handle selection change.
            if (selectedIndex != currentIndex && selectedIndex >= 0)
            {
                // If invalid item is selected again, do nothing.
                if (isInvalidReference && selectedIndex == 0) return;

                // Adjust index if invalid item exists.
                int adjustedIndex = isInvalidReference ? selectedIndex - 1 : selectedIndex;

                if (adjustedIndex >= 0 && adjustedIndex < actions.Count)
                {
                    var selectedAction = actions[adjustedIndex];

                    // Instead of creating a new reference, find an existing InputActionReference sub-asset.
                    var assetPath = AssetDatabase.GetAssetPath(extensionData.defaultInputAction);
                    var references = AssetDatabase.LoadAllAssetsAtPath(assetPath).OfType<InputActionReference>().ToList();

                    // Try to find a reference that points to the selected action.
                    var reference = references.FirstOrDefault(r => r.action == selectedAction);

                    property.objectReferenceValue = reference;

                    // Mark the object as modified to save the change.
                    property.serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                }
            }
        }

        /// <summary>
        /// Returns the height of the property in the Inspector.
        /// Adds extra height if the current reference is invalid to accommodate the help box.
        /// </summary>
        /// <param name="property">The property being drawn.</param>
        /// <param name="label">The GUI label of the property.</param>
        /// <returns>Height of the property field.</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Default single line height.
            float height = EditorGUIUtility.singleLineHeight;

            // Validate property type.
            if (property.propertyType != SerializedPropertyType.ObjectReference) return height * 2; // Space for error HelpBox.

            // Load data if needed.
            if (extensionData == null) extensionData = InputSystemExtensionHelper.GetInputSystemExtensionData();

            // If missing config, reserve space for error.
            if (extensionData == null || extensionData.defaultInputAction == null) return height * 2;

            var currentRef = property.objectReferenceValue as InputActionReference;

            bool isInvalidReference = false;

            if (currentRef != null && currentRef.action != null)
            {
                var actions = extensionData.defaultInputAction.actionMaps.Where(map => map != null).SelectMany(map => map.actions).Where(action => action != null);
                if (!actions.Contains(currentRef.action)) isInvalidReference = true;
            }

            // If invalid, add space for HelpBox.
            if (isInvalidReference)
            {
                height += EditorGUIUtility.standardVerticalSpacing;
                height += EditorGUIUtility.singleLineHeight * 2; // HelpBox height.
            }

            return height;
        }
    }

    #endregion

#endif
}