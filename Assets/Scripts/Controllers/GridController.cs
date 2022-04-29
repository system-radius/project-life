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

        /// <summary>
        /// An event to be fired whenever there is something happening with the cells.
        /// </summary>
        [SerializeField] private Vector3Action cellEvent = null;

        /// <summary>
        /// The size of the grid to be created.
        /// </summary>
        [SerializeField] private Vector2IntData bounds = null;

        /// <summary>
        /// The lower-left position of the board to be generated.
        /// </summary>
        [SerializeField] private Vector3Data origin = null;

        /// <summary>
        /// The size of each cell. Cells are considered to be square,
        /// so this serves as the width and height.
        /// </summary>
        [SerializeField] private NumericData cellSize = null;

        /// <summary>
        /// A reference to the grid being manipulated. It contains LifeCell objects,
        /// and returns/requires bool for getting and setting values, respectively.
        /// </summary>
        private GameBoard<LifeCell, bool> grid;

        private void Start()
        {
            Restart();
        }

        private void Restart()
        {
            grid = new GameBoard<LifeCell, bool>(cellEvent, bounds.value, origin.value, cellSize.value);
        }
    }
}