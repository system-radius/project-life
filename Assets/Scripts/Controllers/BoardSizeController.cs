using UnityEngine;
using EventMessages;

namespace LifeController
{
    /// <summary>
    /// A controller that mainly receives the events related
    /// to the changes regarding the size of the grid/board.
    /// </summary>
    public class BoardSizeController : MonoBehaviour
    {
        [Header("Received events")]
        /// <summary>
        /// An event to be received for changing the size of the grid with regards to X-axis.
        /// </summary>
        [SerializeField] private NumericAction setSizeX = null;

        /// <summary>
        /// An event to be received for changing the size of the grid with regards to Y-axis.
        /// </summary>
        [SerializeField] private NumericAction setSizeY = null;

        [Header("Data")]
        /// <summary>
        /// The size of the grid to be created.
        /// </summary>
        [SerializeField] private Vector2IntData sizeData = null;

        /// <summary>
        /// Built-in function, used to register methods to events.
        /// </summary>
        private void OnEnable()
        {
            setSizeX.listeners += SetSizeX;
            setSizeY.listeners += SetSizeY;
        }

        /// <summary>
        /// Built-in function, used to unregister methods to events.
        /// </summary>
        private void OnDisable()
        {
            setSizeX.listeners -= SetSizeX;
            setSizeY.listeners -= SetSizeY;
        }

        private void Start()
        {
            // Fire the events for whoever requires the initial values.
            setSizeX?.Raise(sizeData.value.x);
            setSizeY?.Raise(sizeData.value.y);
        }

        /// <summary>
        /// Set the size of the grid with regards to the X-Axis.
        /// </summary>
        /// <param name="f">The value to be set.</param>
        private void SetSizeX(float f)
        {
            sizeData.value.x = Mathf.FloorToInt(f);
        }

        /// <summary>
        /// Set the size of the grid with regards to the Y-Axis.
        /// </summary>
        /// <param name="f">The value to be set.</param>
        private void SetSizeY(float f)
        {
            sizeData.value.y = Mathf.FloorToInt(f);
        }
    }
}