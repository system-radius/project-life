using System.Collections;
using UnityEngine;
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
        /// An event to be fired when attempting to center the camera. 
        /// This also allows for the camera to be moved to the center whenever
        /// the grid is re-created for whatever reason.
        /// </summary>
        [SerializeField] private ActionEvent centerCamera = null;

        [Header("Received events")]
        /// <summary>
        /// An event to be received for changing a cell's value in the grid.
        /// </summary>
        [SerializeField] private Vector3Action setCellValueEvent = null;

        /// <summary>
        /// An event to be received when applying the new simulation speed.
        /// </summary>
        [SerializeField] private NumericAction setSimulationSpeed = null;

        /// <summary>
        /// An event to be received when applying the new size settings to the board.
        /// </summary>
        [SerializeField] private ActionEvent restartBoard = null;

        /// <summary>
        /// An event to be received when applying randomization to the board.
        /// </summary>
        [SerializeField] private ActionEvent randomizeSimulation = null;

        /// <summary>
        /// An event to be received when pausing/playing the simulation.
        /// </summary>
        [SerializeField] private ActionEvent pauseSimulation = null;

        /// <summary>
        /// An event that may be received when there is a color change.
        /// Forces the cells to "reset" their values.
        /// </summary>
        [SerializeField] private Vector3Action forceResetAlive = null;

        /// <summary>
        /// An event that may be received when there is a color change.
        /// Forces the cells to "reset" their values.
        /// </summary>
        [SerializeField] private Vector3Action forceResetDead = null;

        [Header("Data")]
        /// <summary>
        /// The size of the grid to be created.
        /// </summary>
        [SerializeField] private Vector2IntData sizeData = null;

        /// <summary>
        /// The amount of time to pass before performing an update to the grid.
        /// </summary>
        [SerializeField] private NumericData updateTime = null;

        /// <summary>
        /// Whether playing the simulation or not.
        /// </summary>
        [SerializeField] private BooleanData playingSimulation = null;

        /// <summary>
        /// A reference to the grid being manipulated. It contains LifeCell objects,
        /// and returns/requires bool for getting and setting values, respectively.
        /// </summary>
        private LifeGameBoard grid;

        /// <summary>
        /// A reference to the current coroutine that runs the generation updates.
        /// </summary>
        private Coroutine activeCoroutine = null;

        /// <summary>
        /// Built-in function, used to register methods to events.
        /// </summary>
        private void OnEnable()
        {
            setCellValueEvent.listeners += SetCellValue;
            setSimulationSpeed.listeners += SetSimulationSpeed;
            restartBoard.listeners += Restart;
            randomizeSimulation.listeners += RandomizeSimulation;
            pauseSimulation.listeners += PauseSimulation;

            forceResetAlive.listeners += ForceResetValue;
            forceResetDead.listeners += ForceResetValue;
        }

        /// <summary>
        /// Built-in function, used to unregister methods to events.
        /// </summary>
        private void OnDisable()
        {
            setCellValueEvent.listeners -= SetCellValue;
            setSimulationSpeed.listeners -= SetSimulationSpeed;
            restartBoard.listeners -= Restart;
            randomizeSimulation.listeners -= RandomizeSimulation;
            pauseSimulation.listeners -= PauseSimulation;

            forceResetAlive.listeners -= ForceResetValue;
            forceResetDead.listeners -= ForceResetValue;
        }

        private void Start()
        {
            RandomizeSimulation();
            setSimulationSpeed?.Raise(updateTime.value);
        }

        /// <summary>
        /// Called to restart the game board being displayed.
        /// Creates the board from scratch.
        /// </summary>
        private void Restart()
        {
            ResetBoard();
        }

        /// <summary>
        /// Called to restart the board and randomize its contents.
        /// Creates the board from scratch.
        /// </summary>
        private void RandomizeSimulation()
        {
            ResetBoard(true);
        }

        /// <summary>
        /// Reset the board. Building it with the new data for size.
        /// </summary>
        /// <param name="randomize">An optional parameter to randomize the contents.</param>
        private void ResetBoard(bool randomize = false)
        {
            if (activeCoroutine != null) StopCoroutine(activeCoroutine);
            // When starting a new simulation, pause it first to allow viewing of the initial state.
            playingSimulation.value = false;

            Vector2Int size = sizeData.value;
            // Create a new instance of the game board.
            grid = new LifeGameBoard(cellEvent, size);

            // Raise the following events to reset the held values of other concerned objects.
            changeLifeValue?.Raise(grid.AliveCells);
            changeGenerationValue?.Raise(grid.Generations);

            // Randomize the grid if applicable.
            if (randomize) grid.Randomize();

            // Raise the event to attempt move the camera to the center.
            centerCamera?.Raise();

            // Start the coroutine.
            activeCoroutine = StartCoroutine(CheckGeneration());
        }

        /// <summary>
        /// Called when pausing/playing the simulation.
        /// </summary>
        private void PauseSimulation()
        {
            playingSimulation.value = !playingSimulation.value;
        }

        /// <summary>
        /// The coroutine that runs the generation update on the grid.
        /// Running this helps with the calculation for the wait time without extra variables.
        /// </summary>
        /// <returns>A reference to the coroutine.</returns>
        private IEnumerator CheckGeneration()
        {
            while (true)
            {
                if (playingSimulation.value)
                {
                    grid.UpdateGeneration();

                    changeLifeValue?.Raise(grid.AliveCells);
                    changeGenerationValue?.Raise(grid.Generations);
                    yield return new WaitForSecondsRealtime(updateTime.value);
                }
                yield return null;
            }
        }

        /// <summary>
        /// A method executed when receiving "cellEvents".
        /// </summary>
        /// <param name="vector">The vector containing the coordinate of the cell, and the value to be set.</param>
        private void SetCellValue(Vector3 vector)
        {
            bool value = vector.z == 1;
            vector.z = 0;
            grid.SetValue(vector, value);
        }

        /// <summary>
        /// A method executed when receiving "setSimulationSpeed" event.
        /// </summary>
        /// <param name="f">The new speed parameter.</param>
        private void SetSimulationSpeed(float f)
        {
            updateTime.value = f;
        }

        /// <summary>
        /// Allows for forcibly reseting the values on the cells,
        /// so that set cell event may be triggered.
        /// </summary>
        /// <param name="vector">An unused parameter.</param>
        private void ForceResetValue(Vector3 vector)
        {
            if (grid != null) grid.SetCellSelfValue();
        }
    }
}