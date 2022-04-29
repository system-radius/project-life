using EventMessages;
using UnityEngine;

namespace LifeModel
{
    public class LifeGameBoard : GameBoard<LifeCell, bool>
    {
        public LifeGameBoard(ParameterizedAction<Vector3> cellEvent, Vector2Int bounds, Vector3 origin = default, float cellSize = 1) : base(cellEvent, bounds, origin, cellSize)
        {
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
                    SetValue(x, y, Random.Range(0, 2) % 2 == 0);
                }
            }
        }

        /// <summary>
        /// Updates the entire board to move on to the next generation.
        /// </summary>
        /// <param name="container">An array container to hold the values while updating.</param>
        public void UpdateGeneration(bool[,] container)
        {
            RetrieveCurrentGridValues(container);
            UpdateCellStates(container);
        }

        /// <summary>
        /// Updates the state of the cells in the grid based on the values
        /// from the given container.
        /// </summary>
        /// <param name="container">A 2D array that contains a snapshot of the grid values.</param>
        public void UpdateCellStates(bool[,] container)
        {
            for (int x = 0; x < Bounds.x; x++)
            {
                for (int y = 0; y < Bounds.y; y++)
                {
                    int n = CountNeighbours(container, x, y);
                    SetValue(x, y, GetCellState(n, GetValue(x, y)));
                }
            }
        }

        public int CountNeighbours(bool[,] container, int positionX, int positionY) 
        {
            int neighbours = 0;
            for (int x = -1; x <= 1; x++)
            {

                int neighbourX = x + positionX;
                if (neighbourX < 0 || neighbourX >= Bounds.x)
                    continue; // Skip invalid indices.

                for (int y = -1; y <= 1; y++)
                {
                    int neighbourY = y + positionY;
                    if (neighbourY < 0 || neighbourY >= Bounds.y)
                        continue; // Skip invalid indices.
                    if (x == 0 && y == 0) continue; // Skip self.
                    neighbours += container[neighbourX, neighbourY] ? 1 : 0;
                }
            }

            return neighbours;
        }

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