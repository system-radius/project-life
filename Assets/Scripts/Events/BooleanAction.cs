using UnityEngine;

namespace EventMessages
{
    /// <summary>
    /// A parameterized action that accepts a boolean value.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/Action/Boolean Action")]
    public class BooleanAction : ParameterizedAction<bool>
    {
    }
}