using UnityEngine;
using EventMessages;
using LifeModel;

namespace LifeController
{
    /// <summary>
    /// A controller that mainly deals with the camera.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Fired Events")]
        /// <summary>
        /// An event fired to set the camera position.
        /// </summary>
        [SerializeField] private Vector3Action moveCamera = null;

        [Header("Received Events")]
        /// <summary>
        /// An event received to set the camera position to the center.
        /// </summary>
        [SerializeField] private ActionEvent centerCamera = null;

        [Header("Data")]
        /// <summary>
        /// The size of the grid.
        /// </summary>
        [SerializeField] private Vector2IntData sizeData = null;

        /// <summary>
        /// Built-in function, used to register methods to events.
        /// </summary>
        private void OnEnable()
        {
            centerCamera.listeners += CenterCamera;
        }

        /// <summary>
        /// Built-in function, used to unregister methods to events.
        /// </summary>
        private void OnDisable()
        {
            centerCamera.listeners -= CenterCamera;
        }

        /// <summary>
        /// Set the camera to be on the center of the created grid.
        /// </summary>
        private void CenterCamera()
        {
            Vector2Int size = sizeData.value;

            // Re-center the camera.
            int halfSizeX = size.x / 2;
            int halfSizeY = size.y / 2;

            // Retrieve the center part of the board, and attempt to have the camera focus on that.
            Vector3 center = new Vector3(halfSizeX, halfSizeY) - (Vector3.one * 0.5f);

            // Then modify the camera size to properly show the entirety of the board.
            center.z = size.x > size.y ? halfSizeY : halfSizeX;

            // Raise an event for moving the camera.
            moveCamera?.Raise(center);
        }
    }
}