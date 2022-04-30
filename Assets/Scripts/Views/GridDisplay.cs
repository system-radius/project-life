using UnityEngine;
using EventMessages;

namespace LifeView
{
    /// <summary>
    /// A display component that creates the game object instances for the cells.
    /// This also handles the color changes, either received from the cells themselves
    /// or from the color settings.
    /// </summary>
    public class GridDisplay : MonoBehaviour
    {
        /// <summary>
        /// The cell prefabricated object to be created.
        /// </summary>
        [SerializeField] private GameObject cellPrefab;

        /// <summary>
        /// The cell event to be monitored.
        /// </summary>
        [SerializeField] private Vector3Action cellEvent;

        /// <summary>
        /// The set color data event received on change of color settings.
        /// </summary>
        [SerializeField] private Vector3Action setColorDataAlive;

        /// <summary>
        /// The set color data event received on change of color settings.
        /// </summary>
        [SerializeField] private Vector3Action setColorDataDead;

        /// <summary>
        /// The current set color for the alive cells.
        /// </summary>
        private Color aliveColor;

        /// <summary>
        /// The current set color for the alive cells.
        /// </summary>
        private Color deadColor;

        /// <summary>
        /// Built-in function, used to register methods to events.
        /// </summary>
        private void OnEnable()
        {
            cellEvent.listeners += ReceiveCellEvent;
            setColorDataAlive.listeners += SetAliveColor;
            setColorDataDead.listeners += SetDeadColor;
        }

        /// <summary>
        /// Built-in function, used to unregister methods to events.
        /// </summary>
        private void OnDisable()
        {
            cellEvent.listeners -= ReceiveCellEvent;
            setColorDataAlive.listeners -= SetAliveColor;
            setColorDataDead.listeners -= SetDeadColor;
        }

        /// <summary>
        /// The registered method called whenever this class receives a cell event.
        /// It will attempt to do one of two things on the position that is given
        /// based on the existence of an object on the said position.
        /// </summary>
        /// <param name="vector">The position that needs to be checked.</param>
        private void ReceiveCellEvent(Vector3 vector)
        {
            // Check for a collider on the provided position.
            Collider2D collider = Physics2D.OverlapCircle(vector, 0.5f);
            if (collider == null)
            {
                // If there is no collider, then the event is meant to tell this display
                // that it needs to create a cell display instance on the given position.
                Instantiate(cellPrefab, vector, Quaternion.identity, transform);
            }
            else
            {
                // If there is already a collider, the color based on its state will just
                // be checked and set (the z-coordinate of the vector represents the state).
                collider.gameObject.GetComponent<SpriteRenderer>().color = vector.z == 0 ? deadColor : aliveColor;
            }
        }

        /// <summary>
        /// The registered method called for setting the color of the alive cells.
        /// </summary>
        /// <param name="vector">The color parameters. X is red, Y is green, Z is blue.</param>
        private void SetAliveColor(Vector3 vector)
        {
            aliveColor = new Color(vector.x, vector.y, vector.z);
        }

        /// <summary>
        /// The registered method called for setting the color of the dead cells.
        /// </summary>
        /// <param name="vector">The color parameters. X is red, Y is green, Z is blue.</param>
        private void SetDeadColor(Vector3 vector)
        {
            deadColor = new Color(vector.x, vector.y, vector.z);
        }
    }
}