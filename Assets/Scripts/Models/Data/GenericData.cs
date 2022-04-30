using UnityEngine;

namespace LifeModel
{
    /// <summary>
    /// A generic scriptable object that contains a value.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    public abstract class GenericData<T> : ScriptableObject
    {
        public T value;
    }
}