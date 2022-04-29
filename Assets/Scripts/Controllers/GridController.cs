using System.Collections;
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
        [Header("Fired Events")]
        /// <summary>
        /// An event to be fired whenever there is something happening with the cells.
        /// </summary>
        [SerializeField] private Vector3Action cellEvent = null;

        /// <summary>
        /// An event to be fired to broadcast the change in the number of alive cells.
        /// </summary>
        [SerializeField] private NumericAction changeLifeValue = null;

        /// <summary>
        /// An event to be fired to broadcast the change in the generation count.
        /// </summary>
        [SerializeField] private NumericAction changeGenerationValue = null;

        /// <summary>
        /// An event to set the camera position. Only used to center the camera.
        /// </summary>
        [SerializeField] private Vector3Action moveCamera = null;

        [Header("Received events")]
        /// <summary>
        /// An event to be received for changing a cell's value in the grid.
        /// </summary>
        [SerializeField] private Vector3Action setCellValueEvent = null;

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
        private LifeGameBoard grid;

        private bool[,] container;

        private Coroutine activeCoroutine = null;

        /// <summary>
        /// Built-in function, used to register methods to events.
        /// </summary>
        private void OnEnable()
        {
            setCellValueEvent.listeners += SetCellValue;
            setSizeX.listeners += SetSizeX;
            setSizeY.listeners += SetSizeY;
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
            if (activeCoroutine != null) StopCoroutine(activeCoroutine);

            Vector2Int size = sizeData.value;
            Vector3 origin = originData.value;
            grid = new LifeGameBoard(cellEvent, size, origin);
            container = new bool[size.x, size.y];

            setSizeX?.Raise(size.x);
            setSizeY?.Raise(size.y);

            grid.Randomize();

            // Re-center the camera.
            int halfSizeX = size.x / 2;
            int halfSizeY = size.y / 2;
            Vector3 center = grid.GetWorldPosition(halfSizeX, halfSizeY) - (Vector3.one * 0.5f);
            center.z = size.x > size.y ? halfSizeY : halfSizeX;
            moveCamera?.Raise(center);

            // Start the coroutine.
            activeCoroutine = StartCoroutine(CheckGeneration());
        }

        private IEnumerator CheckGeneration()
        {
            while (true)
            {
                grid.UpdateGeneration(container);

                changeLifeValue?.Raise(grid.AliveCells);
                changeGenerationValue?.Raise(grid.Generations);
                yield return new WaitForSecondsRealtime(updateTime.value);
            }
        }

        private void SetCellValue(Vector3 vector)
        {
            bool value = vector.z == 1;
            vector.z = 0;
            grid.SetValue(vector, value);
        }

        private void SetSizeX(float f)
        {
            sizeData.value.x = Mathf.FloorToInt(f);
        }

        private void SetSizeY(float f)
        {
            sizeData.value.y = Mathf.FloorToInt(f);
        }
    }
}