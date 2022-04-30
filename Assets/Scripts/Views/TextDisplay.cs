using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EventMessages;

namespace LifeView
{
    /// <summary>
    /// A display class that handles events for changes with regards to text labels.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextDisplay : MonoBehaviour
    {
        /// <summary>
        /// The event that contains the data to be displayed.
        /// </summary>
        [SerializeField] private NumericAction displayChange;

        /// <summary>
        /// A reference to the label component.
        /// </summary>
        private TextMeshProUGUI gui;

        /// <summary>
        /// Built-in function, used to register methods to events.
        /// </summary>
        private void OnEnable()
        {
            displayChange.listeners += ChangeDisplayedText;
        }

        /// <summary>
        /// Built-in function, used to unregister methods to events.
        /// </summary>
        private void OnDisable()
        {
            displayChange.listeners -= ChangeDisplayedText;
        }

        private void Awake()
        {
            gui = GetComponent<TextMeshProUGUI>();
        }

        /// <summary>
        /// The registered method called when the value changes. This
        /// method changes the text of the label to reflect the given
        /// parameter.
        /// </summary>
        /// <param name="f">The new numeric parameter to be displayed.</param>
        private void ChangeDisplayedText(float f)
        {
            gui.text = f.ToString();
        }
    }
}
