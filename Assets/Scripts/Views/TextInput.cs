using UnityEngine;
using TMPro;
using EventMessages;

namespace LifeView
{
    /// <summary>
    /// A display class that handles changes for the text fields.
    /// </summary>
    [RequireComponent(typeof(TMP_InputField))]
    public class TextInput : MonoBehaviour
    {
        /// <summary>
        /// An event for receiving data from some other source.
        /// This will place the current set value on the field.
        /// This event is also used to send the new value to whomever
        /// it may concern.
        /// </summary>
        [SerializeField] private NumericAction setValue = null;

        /// <summary>
        /// A reference to the input field.
        /// </summary>
        private TMP_InputField field;

        /// <summary>
        /// Built-in function, used to register methods to events.
        /// </summary>
        private void OnEnable()
        {
            setValue.listeners += ReceiveInput;
        }

        /// <summary>
        /// Built-in function, used to unregister methods to events.
        /// </summary>
        private void OnDisable()
        {
            setValue.listeners -= ReceiveInput;
        }

        private void Awake()
        {
            field = GetComponent<TMP_InputField>();
        }

        /// <summary>
        /// Called when finalizing the edit being done on the input field,
        /// typically on press of "enter".
        /// </summary>
        /// <param name="input">The final version of the input string.</param>
        public void OnEndEdit(string input)
        {
            // Parse the input then send it through the event.
            // There is no worry of Format exception as long as the field
            // is properly set to only handle integers or floats (decimals).
            setValue?.Raise(float.Parse(input));
        }

        /// <summary>
        /// The registered method which is called on loading the values
        /// that may be edited.
        /// </summary>
        /// <param name="f">The value that is currently set/being used.</param>
        public void ReceiveInput(float f)
        {
            field.text = f.ToString();
        }
    }
}