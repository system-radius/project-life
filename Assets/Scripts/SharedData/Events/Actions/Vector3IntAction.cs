using UnityEngine;

namespace EventMessages
{
    /// <summary>
    /// A parameterized action that accepts an integer vector 3 as parameter.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/Action/Integer Vector 3 action")]
    public class Vector3IntAction : ParameterizedAction<Vector3Int>
    {
    }
}