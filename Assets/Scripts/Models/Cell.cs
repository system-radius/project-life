using UnityEngine;

namespace LifeModel
{
    /// <summary>
    /// A cell interface to be placed in the game board.
    /// Requires all implementing cells to have position,
    /// both world and board-relative.
    /// </summary>
    public abstract class Cell
    {
        /// <summary>
        /// Represents the position on the board.
        /// </summary>
        public Vector2Int BoardPosition { get; set; }

        /// <summary>
        /// Represents the world position.
        /// </summary>
        public Vector3 WorldPosition { get; set; }

        /*
        public Cell(Vector2Int boardPosition, Vector3 worldPosition)
        {
            BoardPosition = boardPosition;
            WorldPosition = worldPosition;
        }
        /**/
    }
}
