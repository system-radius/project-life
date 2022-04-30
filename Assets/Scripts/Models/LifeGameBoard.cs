using System.Collections.Generic;
using EventMessages;
using UnityEngine;

namespace LifeModel
{
    /// <summary>
    /// A subclass of the game board. Implements additional methods
    /// necessary for the generation updates for the game of life.
    /// </summary>
    public class LifeGameBoard : GameBoard<LifeCell>
    {

        /// <summary>
        /// An event instance fired whenever there is something happening on a cell.
        /// </summary>
        private ParameterizedAction<Vector3> internalCellEvent = null;

        /// <summary>
        /// A container for the current state of the board so that generation updates
        /// can be done without destroying data.
        /// </summary>
        private LifeCell[,] container;

        /// <summary>
        /// The number of the cells that are currently alive.
        /// </summary>
        public int AliveCells { get; private set; }

        /// <summary>
        /// The number of generations since the beginning of this game board.
        /// </summary>
        public int Generations { get; private set; }

        private List<LifeCell> inactiveCells;

        public LifeGameBoard(ParameterizedAction<Vector3> cellEvent, Vector2Int bounds, bool randomize = false, Vector3 origin = default, float cellSize = 1) : base(bounds, origin, cellSize)
        {
            AliveCells = 0;
            Generations = 1;
            container = new LifeCell[Bounds.x, Bounds.y];
            internalCellEvent = cellEvent;

            inactiveCells = new List<LifeCell>();

            CreateBoard(randomize);
        }

        /// <summary>
        /// Creates a new board.
        /// </summary>
        /// <param name="randomize">Whether to perform randomization on the values or not.</param>
        public override void CreateBoard(bool randomize)
        {
            for (int x = 0; x < Bounds.x; x++)
            {
                for (int y = 0; y < Bounds.y; y++)
                {
                    bool state = randomize ? Random.Range(0, 2) % 2 == 0 : false;
                    SetCellState(x, y, state);

                }
            }
        }

        /// <summary>
        /// A function that allows for switching the value of a cell.
        /// Switching the value means creating a cell instance or removing it.
        /// 
        /// If the new state is true and there is no cell on the position,
        /// a new instance will be created (or retrieved from the list of 
        /// inactive cells.
        /// 
        /// If the new state is false and there is a cell on the position,
        /// it is removed and added to the list of inactive cells.
        /// 
        /// All other instances, the state is preserved.
        /// </summary>
        /// <param name="x">The X-coordinate.</param>
        /// <param name="y">The Y-coordinate.</param>
        /// <param name="newState">The new state for the cell.</param>
        public void SetCellState(int x, int y, bool newState)
        {
            if (x < 0 || x >= Bounds.x || y < 0 || y >= Bounds.y) return;

            LifeCell cell = GetValue(x, y);
            Vector3 worldPosition = GetWorldPosition(x, y);

            // The first case where the state is set to true, and there is no active cell.
            if (newState && cell == null)
            {
                // If there are still inactive cells, simply retrieve one of those.
                if (inactiveCells.Count > 0)
                {
                    cell = inactiveCells[0];
                    inactiveCells.RemoveAt(0);
                }
                // Otherwise, create a new instance.
                else
                {
                    cell = new LifeCell();
                }

                // Finally, set the position details of the cell.
                cell.BoardPosition = new Vector2Int(x, y);
                cell.WorldPosition = worldPosition;
                worldPosition.z = 1;
            }

            // The second case where the state is set to false, and there is an active cell.
            else if (!newState && cell != null)
            {
                inactiveCells.Add(cell);
                cell = null;
                worldPosition.z = 0;
            }

            // All other cases not handled above. Simply preserve the current state.
            else
            {
                worldPosition.z = cell != null ? 1 : 0;
            }

            // Raise an event that is related on the cell's world position.
            internalCellEvent?.Raise(worldPosition);

            // Set the value to the board.
            SetValue(x, y, cell);
        }

        /// <summary>
        /// A wrapper for the SetCellState function so it may receive the world position directly.
        /// </summary>
        /// <param name="worldPosition">The vector 3 to be converted to board position.</param>
        /// <param name="newState">The new state to be set.</param>
        public void SetCellState(Vector3 worldPosition, bool newState)
        {
            Vector2Int position = GetBoardPosition(worldPosition);
            SetCellState(position.x, position.y, newState);
        }

        /// <summary>
        /// A method that can force events on set value.
        /// </summary>
        public void SetCellSelfValue()
        {
            for (int x = 0; x < Bounds.x; x++)
            {
                for (int y = 0; y < Bounds.y; y++)
                {
                    SetValue(x, y, GetValue(x, y));
                }
            }
        }

        /// <summary>
        /// Updates the entire board to move on to the next generation.
        /// </summary>
        /// /// <param name="enableTorus">Whether the board can do a wrap around search for neighbours or otherwise.</param>
        public int UpdateGeneration(bool enableTorus = false)
        {
            RetrieveCurrentGridValues(container);
            UpdateCellStates(container, enableTorus);
            Generations++;

            return Generations;
        }

        /// <summary>
        /// Updates the state of the cells in the grid based on the values
        /// from the given container.
        /// </summary>
        /// <param name="container">A 2D array that contains a snapshot of the grid values.</param>
        /// <param name="enableTorus">Whether the board can do a wrap around search for neighbours or otherwise.</param>
        public void UpdateCellStates(LifeCell[,] container, bool enableTorus = false)
        {
            AliveCells = 0;
            for (int x = 0; x < Bounds.x; x++)
            {
                for (int y = 0; y < Bounds.y; y++)
                {
                    int n = CountNeighbours(container, x, y, enableTorus);

                    bool state = GetCellState(n, GetValue(x, y) != null);
                    AliveCells += state ? 1 : 0;
                    //SetValue(x, y, state);
                    SetCellState(x, y, state);
                }
            }
        }

        /// <summary>
        /// Attempt to count the neighbours of the cell identified by a position XY. This is done
        /// in this model as it requires checking for the bounds.
        /// </summary>
        /// <param name="container">A 2D array that contains the states of the cell prior to changes.</param>
        /// <param name="positionX">The X-coordinate of the cell to be checked.</param>
        /// <param name="positionY">The Y-coordinate of the cell to be checked.</param>
        /// <param name="enableTorus">Whether the board can do a wrap around search for neighbours or otherwise.</param>
        /// <returns>The total number of "alive" neighbours.</returns>
        public int CountNeighbours(LifeCell[,] container, int positionX, int positionY, bool enableTorus = false)
        {
            int neighbours = 0;
            for (int x = -1; x <= 1; x++)
            {

                int neighbourX = enableTorus ? (x + positionX + Bounds.x) % Bounds.x : x + positionX;
                if (neighbourX < 0 || neighbourX >= Bounds.x)
                    continue; // Skip invalid indices.

                for (int y = -1; y <= 1; y++)
                {
                    int neighbourY = enableTorus ? (y + positionY + Bounds.y) % Bounds.y : y + positionY;
                    if (neighbourY < 0 || neighbourY >= Bounds.y)
                        continue; // Skip invalid indices.
                    if (x == 0 && y == 0) continue; // Skip self.
                    neighbours += container[neighbourX, neighbourY] != null ? 1 : 0;
                }
            }

            return neighbours;
        }

        /// <summary>
        /// Apply the rules of the Game of Life based on the retrieved number of neighbours.
        /// </summary>
        /// <param name="n">The number of neighbours.</param>
        /// <param name="currentState">The current state of the cell.</param>
        /// <returns>The new state of the cell, alive or otherwise.</returns>
        public bool GetCellState(int n, bool currentState)
        {
            bool state = currentState;
            if (n == 3)
            {
                state = true;
            }
            else if (n < 2 || n > 3)
            {
                state = false;
            }

            return state;
        }
    }
}