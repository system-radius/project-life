using UnityEngine;
using EventMessages;

namespace LifeModel
{
    /// <summary>
    /// A representation of the board that contains the cells. Currently designed for 2D.
    /// </summary>
    /// <typeparam name="T">The type to be held by the board.</typeparam>
    /// <typeparam name="CellType">The cell type.</typeparam>
    public class GameBoard<T, CellType> where T : Cell<CellType>, new()
    {

        /// <summary>
        /// An event instance fired whenever there is something happening on a cell.
        /// </summary>
        private ParameterizedAction<Vector3> cellEvent;

        /// <summary>
        /// Contains the bounding values for this grid.
        /// The bounds can only be set during the creation of the game board.
        /// </summary>
        public Vector2Int Bounds { get; private set; }

        /// <summary>
        /// The integer representation of this grid.
        /// </summary>
        private T[,] board;

        /// <summary>
        /// The size of one cell in the grid.
        /// </summary>
        private float cellSize;

        /// <summary>
        /// The origin screen point from which the board will start.
        /// </summary>
        private Vector3 origin;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="cellEvent">Required event which will be fired whenever there is something happening on the cell.</param>
        /// <param name="bounds">Require size of the grid.</param>
        /// <param name="origin">Optional origin, to adjust the placement of the board itself.</param>
        /// <param name="cellSize">Optional cell size, to adjust the size of individual cells.</param>
        /// 
        public GameBoard(ParameterizedAction<Vector3> cellEvent, Vector2Int bounds, Vector3 origin = default(Vector3), float cellSize = 1f)
        {
            Bounds = bounds;
            board = new T[Bounds.x, Bounds.y];

            this.cellEvent = cellEvent;
            this.origin = origin;
            this.cellSize = cellSize <= 0 ? 1f : cellSize;

            CreateBoard();
        }

        private void CreateBoard()
        {
            for (int x = 0; x < Bounds.x; x++)
            {
                for (int y = 0; y < Bounds.y; y++)
                {
                    T obj = new T();
                    obj.BoardPosition = new Vector2Int(x, y);
                    obj.WorldPosition = GetWorldPosition(x, y);
                    board[x, y] = obj;

                    cellEvent?.Raise(obj.WorldPosition);
                }
            }
        }

        /// <summary>
        /// Set the value of a cell in the board using X and Y integer coordinates.
        /// The value will not be set if the coordinates lie outside the board
        /// bounds (-1 or greater than bounding values).
        /// </summary>
        /// <param name="x">X-coordinate.</param>
        /// <param name="y">Y-coordinate.</param>
        /// <param name="value">The value to be set.</param>
        public virtual void SetValue(int x, int y, CellType value)
        {
            if (x < 0 || x >= Bounds.x || y < 0 || y >= Bounds.y) return;

            T cell = board[x, y];
            cell.Value = value;
            cell.WorldPosition = new Vector3(cell.WorldPosition.x, cell.WorldPosition.y, value.Equals(default(CellType)) ? 0 : 1);
            cellEvent?.Raise(cell.WorldPosition);
        }

        /// <summary>
        /// Set the value of a cell in the board using the world position. The said world position
        /// is converted into x and y integer values that may be used to access cells on the board.
        /// </summary>
        /// <param name="worldPosition">A vector 3 noting the world position.</param>
        /// <param name="value">The value to be set.</param>
        public void SetValue(Vector3 worldPosition, CellType value)
        {
            Vector2Int boardPosition = GetBoardPosition(worldPosition);
            SetValue(boardPosition.x, boardPosition.y, value);
        }

        /// <summary>
        /// Get the value of a cell in the board using X and Y integer coordinates.
        /// Returns -1 by default if the coordinates are invalid.
        /// </summary>
        /// <param name="x">X-coordinate.</param>
        /// <param name="y">Y-coordinate.</param>
        /// <returns>Returns the value found on board[x][y], or the default if the coordinates are invalid.</returns>
        public virtual CellType GetValue(int x, int y)
        {
            if (x < 0 || x >= Bounds.x || y < 0 || y >= Bounds.y) return default(CellType);

            return board[x, y].Value;
        }

        /// <summary>
        /// Get the value of a cell in the board using the world position. The said world position
        /// is converted into x and y integer values that may be used to access cells on the board.
        /// </summary>
        /// <param name="worldPosition">A vector 3 noting the world position.</param>
        /// <returns>The value on the cell referred by the world position, or the default if the coordinates are invalid.</returns>
        public CellType GetValue(Vector3 worldPosition)
        {
            Vector2Int coord = GetBoardPosition(worldPosition);
            return GetValue(coord.x, coord.y);
        }

        /// <summary>
        /// Convert the X and Y integers into a vector 3 that notes the position in the world.
        /// This returns the position supposed to be on the center of the cell.
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <returns>The converted world position for the given x-y values.</returns>
        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, y) * cellSize + origin;
        }

        /// <summary>
        /// Convert the world position into values that may be used to access the board.
        /// </summary>
        /// <param name="worldPosition">The world position.</param>
        /// <returns>An integer vector 2 that contains the X and Y coordinates that may be used to access the board.</returns>
        public Vector2Int GetBoardPosition(Vector3 worldPosition)
        {
            int x = Mathf.RoundToInt((worldPosition - origin).x / cellSize);
            int y = Mathf.RoundToInt((worldPosition - origin).y / cellSize);

            return new Vector2Int(x, y);
        }

        /// <summary>
        /// A utility method for retrieving the grid values and saving them
        /// into another container.
        /// </summary>
        /// <param name="container">Another 2D array that will contain the current grid values.</param>
        public void RetrieveCurrentGridValues(CellType[,] container)
        {
            for (int x = 0; x < Bounds.x; x++)
            {
                for (int y = 0; y < Bounds.y; y++)
                {
                    container[x, y] = GetValue(x, y);
                }
            }
        }
    }

}