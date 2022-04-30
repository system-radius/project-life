using EventMessages;
using UnityEngine;

namespace LifeModel
{
    /// <summary>
    /// A subclass of the game board. Implements additional methods
    /// necessary for the generation updates for the game of life.
    /// </summary>
    public class LifeGameBoard : GameBoard<LifeCell, bool>
    {

        /// <summary>
        /// A container for the current state of the board so that generation updates
        /// can be done without destroying data.
        /// </summary>
        private bool[,] container;

        /// <summary>
        /// The number of the cells that are currently alive.
        /// </summary>
        public int AliveCells { get; private set; }

        /// <summary>
        /// The number of generations since the beginning of this game board.
        /// </summary>
        public int Generations { get; private set; }

        public LifeGameBoard(ParameterizedAction<Vector3> cellEvent, Vector2Int bounds, Vector3 origin = default, float cellSize = 1) : base(cellEvent, bounds, origin, cellSize)
        {
            AliveCells = 0;
            Generations = 1;
            container = new bool[Bounds.x, Bounds.y];
        }

        /// <summary>
        /// Randomizes the board.
        /// </summary>
        public void Randomize()
        {
            for (int x = 0; x < Bounds.x; x++)
            {
                for (int y = 0; y < Bounds.y; y++)
                {
                    bool state = Random.Range(0, 2) % 2 == 0;
                    SetValue(x, y, state);

                    AliveCells += state ? 1 : 0;
                }
            }
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
        public void UpdateCellStates(bool[,] container, bool enableTorus = false)
        {
            AliveCells = 0;
            for (int x = 0; x < Bounds.x; x++)
            {
                for (int y = 0; y < Bounds.y; y++)
                {
                    int n = CountNeighbours(container, x, y, enableTorus);

                    bool state = GetCellState(n, GetValue(x, y));
                    AliveCells += state ? 1 : 0;
                    SetValue(x, y, state);
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
        public int CountNeighbours(bool[,] container, int positionX, int positionY, bool enableTorus = false)
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
                    neighbours += container[neighbourX, neighbourY] ? 1 : 0;
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