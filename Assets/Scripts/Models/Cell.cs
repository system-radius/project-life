using UnityEngine;

namespace LifeModel
{
    /// <summary>
    /// A cell interface to be placed in the game board.
    /// Requires all implementing cells to have position,
    /// both world and board-relative.
    /// </summary>
    public abstract class Cell<T>
    {
        /// <summary>
        /// Represents the position on the board.
        /// </summary>
        public Vector2Int BoardPosition { get; set; }

        /// <summary>
        /// Represents the position of this cell with regards to the world.
        /// </summary>
        public Vector3 WorldPosition { get; set; }

        /// <summary>
        /// The value of this cell.
        /// </summary>
        public abstract T Value { get; set; }
    }
}
