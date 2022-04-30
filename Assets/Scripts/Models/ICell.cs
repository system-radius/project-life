using UnityEngine;

namespace LifeModel
{
    /// <summary>
    /// A cell interface to be placed in the game board.
    /// Requires all implementing cells to have position,
    /// both world and board-relative.
    /// </summary>
    public interface ICell<T>
    {
        /// <summary>
        /// Represents the position on the board.
        /// </summary>
        Vector2Int BoardPosition { get; set; }

        /// <summary>
        /// Represents the position of this cell with regards to the world.
        /// </summary>
        Vector3 WorldPosition { get; set; }

        /// <summary>
        /// The value of this cell.
        /// </summary>
        T Value { get; set; }
    }
}
