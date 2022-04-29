using UnityEngine;
using UnityEngine.InputSystem;
using EventMessages;
using LifeModel;

namespace LifeController
{
    /// <summary>
    /// A controller that has primary access to the game board.
    /// Its main responsibility is to create the board, and apply
    /// the updates to it.
    /// 
    /// This is the main entry point into the game of life.
    /// </summary>
    public class GridController : MonoBehaviour
    {
        [Header("Events")]
        /// <summary>
        /// An event to be fired whenever there is something happening with the cells.
        /// </summary>
        [SerializeField] private Vector3Action cellEvent = null;

        /// <summary>
        /// An event to be received for changing a cell's value in the grid.
        /// </summary>
        [SerializeField] private Vector3Action setCellValueEvent = null;

        /// <summary>
        /// An event to set the camera position. Only used to center the camera.
        /// </summary>
        [SerializeField] private Vector3Action moveCamera = null;


        [Header("Data")]
        /// <summary>
        /// The size of the grid to be created.
        /// </summary>
        [SerializeField] private Vector2IntData sizeData = null;

        /// <summary>
        /// The lower-left position of the board to be generated.
        /// </summary>
        [SerializeField] private Vector3Data originData = null;

        /// <summary>
        /// The amount of time to pass before performing an update to the grid.
        /// </summary>
        [SerializeField] private NumericData updateTime = null;

        /// <summary>
        /// A reference to the grid being manipulated. It contains LifeCell objects,
        /// and returns/requires bool for getting and setting values, respectively.
        /// </summary>
        private GameBoard<LifeCell, bool> grid;

        /// <summary>
        /// Built-in function, used to register methods to events.
        /// </summary>
        private void OnEnable()
        {
            setCellValueEvent.listeners += SetCellValue;
        }

        /// <summary>
        /// Built-in function, used to unregister methods to events.
        /// </summary>
        private void OnDisable()
        {
            setCellValueEvent.listeners -= SetCellValue;
        }

        private void Start()
        {
            Restart();
        }

        /// <summary>
        /// Called to restart the game board being displayed.
        /// Creates the board from scratch.
        /// </summary>
        private void Restart()
        {
            Vector2Int size = sizeData.value;
            Vector3 origin = originData.value;
            grid = new GameBoard<LifeCell, bool>(cellEvent, size, origin);

            RandomizeBoard();

            // Re-center the camera.
            int halfSizeX = size.x / 2;
            int halfSizeY = size.y / 2;
            Vector3 center = grid.GetWorldPosition(halfSizeX, halfSizeY) - (Vector3.one * 0.5f);
            center.z = size.x > size.y ? halfSizeY : halfSizeX;
            moveCamera?.Raise(center);
        }

        /// <summary>
        /// Randomizes the board.
        /// </summary>
        private void RandomizeBoard()
        {
            for (int x = 0; x < grid.Bounds.x; x++)
            {
                for (int y = 0; y < grid.Bounds.y; y++)
                {
                    grid.SetValue(x, y, Random.Range(0, 2) % 2 == 0);
                }
            }
        }

        private void SetCellValue(Vector3 vector)
        {
            bool value = vector.z == 1;
            vector.z = 0;
            grid.SetValue(vector, value);
        }
    }
}