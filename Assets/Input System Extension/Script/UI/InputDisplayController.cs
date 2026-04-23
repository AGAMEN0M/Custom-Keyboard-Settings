/*
 * ---------------------------------------------------------------------------
 * Description: Controls the activation and deactivation of input display elements
 *              by name using InputDisplayManager, based on predefined states.
 * 
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using UnityEngine;
using System;

namespace InputSystemExtension
{
    [RequireComponent(typeof(InputDisplayManager))]
    [AddComponentMenu("Tools/Input System Extension/UI/Display/Input Display Controller")]
    public class InputDisplayController : MonoBehaviour
    {
        #region === Serializable Structs ===

        /// <summary>
        /// Represents an input display entry with a name tag and whether it should be enabled or disabled.
        /// </summary>
        [Serializable]
        public struct InputDisplayEntry
        {
            [Tooltip("Identifier of the display element to modify.")]
            public string nameTag;

            [Tooltip("Determines if this element should be enabled.")]
            public bool enable;
        }

        #endregion

        #region === Inspector Configuration ===

        [Header("Input elements to enable when activated.")]
        [SerializeField] private InputDisplayEntry[] entriesToEnable;

        [Header("Input elements to disable when deactivated.")]
        [SerializeField] private InputDisplayEntry[] entriesToDisable;

        #endregion

        #region === Private References ===

        private InputDisplayManager displayManager;

        #endregion

        #region === Public Properties ===

        /// <summary>
        /// Gets or sets the entries to enable when activated.
        /// </summary>
        public InputDisplayEntry[] EntriesToEnable
        {
            get => entriesToEnable;
            set => entriesToEnable = value;
        }

        /// <summary>
        /// Gets or sets the entries to disable when deactivated.
        /// </summary>
        public InputDisplayEntry[] EntriesToDisable
        {
            get => entriesToDisable;
            set => entriesToDisable = value;
        }

        #endregion

        #region === Unity Methods ===

        private void Awake()
        {
            // Tries to fetch the InputDisplayManager from the same GameObject.
            displayManager = GetComponent<InputDisplayManager>();

            // If not found, disables this component and logs an error.
            if (displayManager == null)
            {
                Debug.LogError("Missing required InputDisplayManager component.", this);
                enabled = false;
            }
        }

        #endregion

        #region === Public Methods ===

        /// <summary>
        /// Applies input display entries based on the given state.
        /// </summary>
        /// <param name="enable">
        /// If true, applies the entries from 'EntriesToEnable';  
        /// otherwise, applies 'EntriesToDisable'.
        /// </param>
        public void ApplyDisplayState(bool enable)
        {
            // Selects which list of entries to use.
            var targetEntries = enable ? entriesToEnable : entriesToDisable;

            // If the list is null or empty, do nothing.
            if (targetEntries == null || targetEntries.Length == 0) return;

            // Iterates through all entries and applies them using the manager.
            foreach (var entry in targetEntries)
            {
                displayManager.EnableAndDisableViewer(entry.nameTag, entry.enable);
            }
        }

        #endregion
    }
}