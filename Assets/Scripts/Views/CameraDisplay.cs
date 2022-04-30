using UnityEngine;
using EventMessages;

namespace LifeView
{

    /// <summary>
    /// A display class that mainly handles the camera transform.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraDisplay : MonoBehaviour
    {
        /// <summary>
        /// An event to be received whenever the camera needs to be moved.
        /// </summary>
        [SerializeField] private Vector3Action moveCamera;

        /// <summary>
        /// An instance of the camera component. Mainly used for altering
        /// the size.
        /// </summary>
        private Camera cam;

        /// <summary>
        /// Built-in function, used to register methods to events.
        /// </summary>
        private void OnEnable()
        {
            moveCamera.listeners += MoveCamera;
        }

        /// <summary>
        /// Built-in function, used to unregister methods to events.
        /// </summary>
        private void OnDisable()
        {
            moveCamera.listeners -= MoveCamera;
        }

        private void Start()
        {
            cam = GetComponent<Camera>();
        }

        /// <summary>
        /// The registered method to be called on invocation of the move camera event.
        /// </summary>
        /// <param name="vector">The position that the camera needs to look at.</param>
        private void MoveCamera(Vector3 vector)
        {
            // Set the X and Y position of the camera to look at the new position.
            // The Z position remains constant for any assignment.
            Vector3 position = new Vector3(vector.x, vector.y, transform.position.z);
            transform.position = position;

            // The z-coordinate of the given vector represents the view size of the camera.
            cam.orthographicSize = vector.z;
        }
    }
}