using UnityEngine;

namespace EventMessages
{
    /// <summary>
    /// A parameterized action that accepts a numeric value.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/Action/Numeric action")]
    public class NumericAction : ParameterizedAction<float>
    {
    }
}