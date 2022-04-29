using System;
using UnityEngine;

namespace EventMessages
{
    /// <summary>
    /// An event scriptable object that accepts a parameter
    /// and will return some value.
    /// </summary>
    /// <typeparam name="ParamType">The parameter type to be accepted.</typeparam>
    /// <typeparam name="ReturnType">The return type.</typeparam>
    public abstract class ParameterizedFunc<ParamType, ReturnType> : ScriptableObject
    {
        public event Func<ParamType, ReturnType> listeners;

        public ReturnType Raise(ParamType arg)
        {
            if (listeners != null)
            {
                return listeners(arg);
            }

            return default(ReturnType);
        }
    }
}