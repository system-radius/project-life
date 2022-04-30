using System;
using UnityEngine;

namespace EventMessages
{
    /// <summary>
    /// An action event with no parameters.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/Action/Action Event")]
    public class ActionEvent : ScriptableObject
    {
        public event Action listeners;

        public void Raise()
        {
            if (listeners != null)
            {
                listeners();
            }
        }
    }
}