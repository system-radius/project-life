using UnityEngine;
using UnityEngine.InputSystem;
using EventMessages;

namespace LifeController
{
    /// <summary>
    /// Handles the input from user. This allows for manual setting
    /// of cell values on the grid. Left-click revives the cell,
    /// right-click kills it.
    /// </summary>
    public class GridInput : MonoBehaviour
    {
        /// <summary>
        /// An event fired to actually set the values in the model.
        /// </summary>
        [SerializeField] private Vector3Action setGridValue = null;

        /// <summary>
        /// Allows for setting values while the left mouse button is held down.
        /// </summary>
        private bool leftMouseActive;

        /// <summary>
        /// Allows for setting values while the right mouse button is held down.
        /// </summary>
        private bool rightMouseActive;

        /// <summary>
        /// During the update, continuously read the mouse position and convert
        /// it into the world position. That world position may be transfered as
        /// it is to the model, and the model is able to convert it to board position.
        /// </summary>
        private void Update()
        {
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            if (leftMouseActive)
            {
                worldPosition.z = 1;
                setGridValue?.Raise(worldPosition);
            }
            else if (rightMouseActive)
            {
                worldPosition.z = 0;
                setGridValue?.Raise(worldPosition);
            }
        }

        /// <summary>
        /// Called from Unity's new input system. Sets the status of the left mouse button,
        /// whether it is being held down or otherwise.
        /// </summary>
        /// <param name="context">The context of the input event.</param>
        public void OnLeftMouseButtonDown(InputAction.CallbackContext context)
        {
            leftMouseActive = context.ReadValueAsButton();
        }

        /// <summary>
        /// Called from Unity's new input system. Sets the status of the right mouse button,
        /// whether it is being held down or otherwise.
        /// </summary>
        /// <param name="context">The context of the input event.</param>
        public void OnRightMouseButtonDown(InputAction.CallbackContext context)
        {
            rightMouseActive = context.ReadValueAsButton();
        }
    }
}