using UnityEngine;
using EventMessages;

namespace LifeView
{
    /// <summary>
    /// A component that fires an event on button press.
    /// </summary>
    public class ButtonPress : MonoBehaviour
    {
        /// <summary>
        /// The event to be fired on button press.
        /// </summary>
        [SerializeField] private ActionEvent buttonPress;

        /// <summary>
        /// Exposed method to be set on editor.
        /// </summary>
        public void OnButtonPress()
        {
            buttonPress?.Raise();
        }
    }
}