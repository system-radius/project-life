using UnityEngine;
using UnityEngine.UI;
using EventMessages;

namespace LifeView
{
    /// <summary>
    /// A display class responsible for check boxes.
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public class CheckBoxToggle : MonoBehaviour
    {
        /// <summary>
        /// An event for receiving the state of the check box.
        /// Also used to broacast the changed state.
        /// </summary>
        [SerializeField] private BooleanAction booleanAction = null;

        /// <summary>
        /// A reference to the toggle component.
        /// </summary>
        private Toggle toggle;

        /// <summary>
        /// Built-in function, used to register methods to events.
        /// </summary>
        private void OnEnable()
        {
            booleanAction.listeners += ReceiveInput;
        }

        /// <summary>
        /// Built-in function, used to unregister methods to events.
        /// </summary>
        private void OnDisable()
        {
            booleanAction.listeners -= ReceiveInput;
        }

        private void Awake()
        {
            toggle = GetComponent<Toggle>();
        }

        /// <summary>
        /// Called when changing the value of a check box. To be registered
        /// using Unity events.
        /// </summary>
        public void OnValueChanged(bool value)
        {
            booleanAction?.Raise(value);
        }

        /// <summary>
        /// The registered method which is called on loading the values
        /// that may be edited.
        /// </summary>
        /// <param name="state">The toggle state of the check box.</param>
        private void ReceiveInput(bool state)
        {
            toggle.isOn = state;
        }
    }
}