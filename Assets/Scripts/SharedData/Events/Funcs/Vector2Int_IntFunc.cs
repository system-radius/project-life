using UnityEngine;

namespace EventMessages
{
    /// <summary>
    /// A func event type that is able to return an integer
    /// given a vector 2.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/Func/Vector2, int return")]
    public class Vector2Int_IntFunc : ParameterizedFunc<Vector2, int>
    {
    }
}