using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
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
        /// An event to be received when applying the new size settings to the board.
        /// </summary>
        [SerializeField] private ActionEvent restartBoardEvent = null;

        /// <summary>
        /// An event to be received when applying randomization to the board.
        /// </summary>
        [SerializeField] private ActionEvent randomizeSimulationEvent = null;

        /// <summary>
        /// The current set color for the alive cells.
        /// </summary>
        private Color aliveColor;

        /// <summary>
        /// The current set color for the alive cells.
        /// </summary>
        private Color deadColor;

        private Dictionary<Vector2, SpriteRenderer> activeCells;

        private Dictionary<Vector2, SpriteRenderer> inactiveCells;

        private List<SpriteRenderer> allCells;

        /// <summary>
        /// Built-in function, used to register methods to events.
        /// </summary>
        private void OnEnable()
        {
            cellEvent.listeners += ReceiveCellEvent;
            setColorDataAlive.listeners += SetAliveColor;
            setColorDataDead.listeners += SetDeadColor;
            restartBoardEvent.listeners += Restart;
            randomizeSimulationEvent.listeners += Restart;

            activeCells = new Dictionary<Vector2, SpriteRenderer>();
            inactiveCells = new Dictionary<Vector2, SpriteRenderer>();
            allCells = new List<SpriteRenderer>();
        }

        /// <summary>
        /// Built-in function, used to unregister methods to events.
        /// </summary>
        private void OnDisable()
        {
            cellEvent.listeners -= ReceiveCellEvent;
            setColorDataAlive.listeners -= SetAliveColor;
            setColorDataDead.listeners -= SetDeadColor;
            restartBoardEvent.listeners -= Restart;
            randomizeSimulationEvent.listeners -= Restart;
        }

        private void Restart()
        {
            foreach (SpriteRenderer cell in allCells)
            {
                cell.color = new Color(0, 0, 0, 0);
            }

            activeCells.Clear();
            inactiveCells.Clear();
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
            SpriteRenderer cell;
            if (collider == null)
            {
                // If there is no collider, then the event is meant to tell this display
                // that it needs to create a cell display instance on the given position.
               cell = Instantiate(cellPrefab, vector, Quaternion.identity, transform).GetComponent<SpriteRenderer>();
                allCells.Add(cell);
            } else {
                cell = collider.GetComponent<SpriteRenderer>();
            }

            // If there is already a collider, the color based on its state will just
            // be checked and set (the z-coordinate of the vector represents the state).
            if (vector.z == 0)
            {
                // The cell is set to be dead.
                if (!inactiveCells.ContainsKey(vector)) inactiveCells.Add(vector, cell);
                activeCells.Remove(vector);
                cell.color = deadColor;
            }
            else
            {
                // The cell is set to be alive.
                if (!activeCells.ContainsKey(vector)) activeCells.Add(vector, cell);
                inactiveCells.Remove(vector);
                cell.color = aliveColor;
            }
        }

        /// <summary>
        /// The registered method called for setting the color of the alive cells.
        /// </summary>
        /// <param name="vector">The color parameters. X is red, Y is green, Z is blue.</param>
        private void SetAliveColor(Vector3 vector)
        {
            aliveColor = new Color(vector.x, vector.y, vector.z);
            foreach (Vector2 key in activeCells.Keys)
            {
                activeCells[key].color = aliveColor;
            }
        }

        /// <summary>
        /// The registered method called for setting the color of the dead cells.
        /// </summary>
        /// <param name="vector">The color parameters. X is red, Y is green, Z is blue.</param>
        private void SetDeadColor(Vector3 vector)
        {
            deadColor = new Color(vector.x, vector.y, vector.z);
            foreach (Vector2 key in inactiveCells.Keys)
            {
                inactiveCells[key].color = aliveColor;
            }
        }
    }
}