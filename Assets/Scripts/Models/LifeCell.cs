using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LifeModel
{

    /// <summary>
    /// A cell model, represents an entry in the grid.
    /// Simply contains a state, which represents
    /// whether the cell is alive or otherwise.
    /// 
    /// Also able to check on the neighbors and determine
    /// whether it should live or die.
    /// </summary>
    public class LifeCell : ICell
    {
        public Vector2Int BoardPosition { get; set; }
        public Vector3 WorldPosition { get; set; }
    }
}