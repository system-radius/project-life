using System;
using UnityEngine;

namespace EventMessages
{
    /// <summary>
    /// An event scriptable object that allows for the
    /// inclusion of arguments.
    /// </summary>
    /// <typeparam name="T">Any type that will be used as parameter.</typeparam>
    public abstract class ParameterizedAction<T> : ScriptableObject
    {
        public event Action<T> listeners;

        public void Raise(T arg)
        {
            if (listeners != null)
            {
                listeners(arg);
            }
        }
    }
}