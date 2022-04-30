using System;
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
        /// An event to be fired whenever there is something heppening with the cells.
        /// </summary>
        [SerializeField] private Vector3Action cellEvent = null;

        /// <summary>
        /// An event to be fired to broadcast the change in the number of alive cells.
        /// </summary>
        [SerializeField] private NumericAction changeLifeValueEvent = null;

        /// <summary>
        /// An event to be fired to broadcast the change in the generation count.
        /// </summary>
        [SerializeField] private NumericAction changeGenerationValueEvent = null;

        /// <summary>
        /// An event to be fired when attempting to center the camera. 
        /// This also allows for the camera to be moved to the center whenever
        /// the grid is re-created for whatever reason.
        /// </summary>
        [SerializeField] private ActionEvent centerCameraEvent = null;

        [Header("Received events")]
        /// <summary>
        /// An event to be receieved whenever there is something new with the cells.
        /// </summary>
        [SerializeField] private Vector3Action internalCellEvent = null;

        /// <summary>
        /// An event to be received for changing a cell's value in the grid.
        /// </summary>
        [SerializeField] private Vector3Action setCellValueEvent = null;

        /// <summary>
        /// An event to be received when applying the new simulation speed.
        /// </summary>
        [SerializeField] private NumericAction setSimulationSpeedEvent = null;

        /// <summary>
        /// An event to be received when applying the new size settings to the board.
        /// </summary>
        [SerializeField] private ActionEvent restartBoardEvent = null;

        /// <summary>
        /// An event to be received when applying randomization to the board.
        /// </summary>
        [SerializeField] private ActionEvent randomizeSimulationEvent = null;

        /// <summary>
        /// An event to be received when pausing/playing the simulation.
        /// </summary>
        [SerializeField] private ActionEvent pauseSimulationEvent = null;

        /// <summary>
        /// An event to be received when enabling/disabling the torus capability of the board.
        /// </summary>
        [SerializeField] private BooleanAction enableTorusEvent = null;

        /// <summary>
        /// An event that may be received when there is a color change.
        /// Forces the cells to "reset" their values.
        /// </summary>
        [SerializeField] private Vector3Action forceResetAliveEvent = null;

        /// <summary>
        /// An event that may be received when there is a color change.
        /// Forces the cells to "reset" their values.
        /// </summary>
        [SerializeField] private Vector3Action forceResetDeadEvent = null;

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
        /// Whether the grid can wrap around itself when checking for neighbours.
        /// </summary>
        [SerializeField] private BooleanData boardWrap = null;

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

            internalCellEvent.listeners += ChangedCellValue;
            setCellValueEvent.listeners += SetCellValue;
            setSimulationSpeedEvent.listeners += SetSimulationSpeed;
            restartBoardEvent.listeners += Restart;
            randomizeSimulationEvent.listeners += RandomizeSimulation;
            pauseSimulationEvent.listeners += PauseSimulation;
            enableTorusEvent.listeners += EnableTorus;

            forceResetAliveEvent.listeners += ForceResetValue;
            forceResetDeadEvent.listeners += ForceResetValue;
        }

        /// <summary>
        /// Built-in function, used to unregister methods to events.
        /// </summary>
        private void OnDisable()
        {
            internalCellEvent.listeners -= ChangedCellValue;
            setCellValueEvent.listeners -= SetCellValue;
            setSimulationSpeedEvent.listeners -= SetSimulationSpeed;
            restartBoardEvent.listeners -= Restart;
            randomizeSimulationEvent.listeners -= RandomizeSimulation;
            pauseSimulationEvent.listeners -= PauseSimulation;
            enableTorusEvent.listeners -= EnableTorus;

            forceResetAliveEvent.listeners -= ForceResetValue;
            forceResetDeadEvent.listeners -= ForceResetValue;
        }

        private void Start()
        {
            RandomizeSimulation();
            setSimulationSpeedEvent?.Raise(updateTime.value);
            enableTorusEvent?.Raise(boardWrap.value);
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
            // When starting a new simulation, pause it first to allow viewing of the initial state.
            playingSimulation.value = false;

            Vector2Int size = sizeData.value;
            // Create a new instance of the game board.
            grid = new LifeGameBoard(internalCellEvent, size, randomize);

            // Raise the following events to reset the held values of other concerned objects.
            changeLifeValueEvent?.Raise(grid.AliveCells);
            changeGenerationValueEvent?.Raise(grid.Generations);

            // Raise the event to attempt move the camera to the center.
            centerCameraEvent?.Raise();
            SetSimulationSpeed(updateTime.value);
        }

        /// <summary>
        /// Called when pausing/playing the simulation.
        /// </summary>
        private void PauseSimulation()
        {
            playingSimulation.value = !playingSimulation.value;
        }

        /// <summary>
        /// Called when enabling/disabling the wrap around functionality.
        /// </summary>
        /// <param name="state">Whether the board can do a wrap around search for neighbours or otherwise.</param>
        private void EnableTorus(bool state)
        {
            boardWrap.value = state;
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
                    grid.UpdateGeneration(boardWrap.value);

                    changeLifeValueEvent?.Raise(grid.AliveCells);
                    changeGenerationValueEvent?.Raise(grid.Generations);
                    yield return new WaitForSecondsRealtime(updateTime.value);
                }
                yield return null;
            }
        }

        /// <summary>
        /// A method executed when receiving "internalCellEvent".
        /// This simply propagates the events for whichever outside 
        /// class that would like to receive it.
        /// </summary>
        /// <param name="vector">The vector containing the coordinate of the cell, and the value to be set.</param>
        private void ChangedCellValue(Vector3 vector)
        {
            cellEvent?.Raise(vector);
        }

        /// <summary>
        /// A method executed when receiving "setCellEvent".
        /// This allows for setting the cell value that is in the grid.
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
            if (activeCoroutine != null) StopCoroutine(activeCoroutine);
            updateTime.value = f;

            // Start the coroutine.
            activeCoroutine = StartCoroutine(CheckGeneration());
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