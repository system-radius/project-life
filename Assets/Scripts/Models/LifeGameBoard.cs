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
        /// An event instance fired whenever there is something happening on a cell.
        /// </summary>
        private ParameterizedAction<Vector3> internalCellEvent = null;

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

        public LifeGameBoard(ParameterizedAction<Vector3> cellEvent, Vector2Int bounds, bool randomize = false, Vector3 origin = default, float cellSize = 1) : base(bounds, origin, cellSize)
        {
            AliveCells = 0;
            Generations = 1;
            container = new bool[Bounds.x, Bounds.y];
            internalCellEvent = cellEvent;

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
                    LifeCell obj = new LifeCell();
                    obj.BoardPosition = new Vector2Int(x, y);
                    obj.WorldPosition = GetWorldPosition(x, y);
                    board[x, y] = obj;

                    bool state = randomize ? Random.Range(0, 2) % 2 == 0 : false;
                    SetValue(x, y, state);

                    AliveCells += state ? 1 : 0;
                }
            }
        }

        /// <summary>
        /// Overrides the SetValue from the game board. This override allows for
        /// an event to be raised. Said event does not particularly care who or
        /// what receives it.
        /// </summary>
        /// <param name="x">The X-coordinate.</param>
        /// <param name="y">The Y-coordinate.</param>
        /// <param name="value">The value to be set.</param>
        public override void SetValue(int x, int y, bool value)
        {
            base.SetValue(x, y, value);
            if (x < 0 || x >= Bounds.x || y < 0 || y >= Bounds.y) return;

            LifeCell cell = board[x, y];
            cell.WorldPosition = new Vector3(cell.WorldPosition.x, cell.WorldPosition.y, cell.Value ? 1 : 0);

            internalCellEvent?.Raise(cell.WorldPosition);
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